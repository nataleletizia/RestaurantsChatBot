using ChatBot.Controllers;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ChatBot
{
    class GoogleGeoCodeResponse
    {
        [JsonProperty("html_attributions")]
        public List<object> html_attributions { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("results")]
        public Results[] results { get; set; }
    }
    
    class IpInfo
    {

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("loc")]
        public string Loc { get; set; }

        [JsonProperty("org")]
        public string Org { get; set; }

        [JsonProperty("postal")]
        public string Postal { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }

    public class GeoLocatorHandler : DelegatingHandler
    {
        public List<Results> GetMatchingRestaurants(Location cog, RestaurantCriteria restaurantCriteria)
        {
            List<Results> matchingRestaurants = new List<Results>();

            var gmapsKey = "<gmapskey>";

            var lat = cog.Latitude.ToString().Replace(",", ".");
            var lng = cog.Longitude.ToString().Replace(",", ".");

            var userKeywords = restaurantCriteria.counter>0 ? restaurantCriteria.mykeywords:String.Empty;

            var address = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location="+lat+","+lng+"&radius=5000&type=restaurant&key="+gmapsKey+"&sensor=false&opennow"+ userKeywords;

            // just a restaurant rendering option: 

            var result = new System.Net.WebClient().DownloadString(address);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var jsonObject = jss.Deserialize<GoogleGeoCodeResponse>(result);
            /*var location = new Location();
            location.Latitude = googleGeoCodeResponse.Results[0].geometry.location.lat;
            location.Longitude = googleGeoCodeResponse.Results[0].geometry.location.lng;*/

            //var jsonObject = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(result);

            var distMatrixGMapsKey = "<distmatrixgmapskey>";

            foreach (var item in jsonObject.results)
            {
                var destLat = item.geometry.location.lat.ToString().Replace(",", ".");

                var destLng = item.geometry.location.lng.ToString().Replace(",", ".");
                var customerDeparturePoint = restaurantCriteria.userLocation.Name;

                address = "https://maps.googleapis.com/maps/api/distancematrix/json?origins="+customerDeparturePoint+"&destinations=" + destLat + "," + destLng + "&key="+distMatrixGMapsKey;

                result = new System.Net.WebClient().DownloadString(address);
                var distJSonObject = jss.Deserialize<GMapsDistanceMatrix>(result);
                item.distance = distJSonObject.rows[0].elements[0].distance.text;
                matchingRestaurants.Add(item);
            }

            return matchingRestaurants;
        }

        public Location GEOCodeAddress(String Address)
        {
            var address = String.Format("http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false", Address.Replace(" ", "+"));
            var result = new System.Net.WebClient().DownloadString(address);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var googleGeoCodeResponse = jss.Deserialize<GoogleGeoCodeResponse>(result);
            var location = new Location();
            location.Latitude = googleGeoCodeResponse.results[0].geometry.location.lat;
            location.Longitude = googleGeoCodeResponse.results[0].geometry.location.lng;

            return location;
        }

        public Location GetLocation(string address)
        {
            IpInfo ipInfo = new IpInfo();
            try
            {
                string info = new WebClient().DownloadString("http://maps.googleapis.com/maps/api/geocode/json?address="+address+"&sensor=true");
                ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
                RegionInfo myRI1 = new RegionInfo(ipInfo.Country);
                ipInfo.Country = myRI1.EnglishName;
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            var location = new Location();
            location.Latitude = ipInfo.Latitude;
            location.Longitude = ipInfo.Longitude;
            
            return location;
        }

        public string GetIpAddress(/*this*/ HttpRequestMessage requestMessage)
        {
            // Web Hosting
            if (requestMessage.Properties.ContainsKey("MS_HttpContext"))
            {
                return HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null;
            }
            // Self Hosting
            if (requestMessage.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty property =
                    (RemoteEndpointMessageProperty)requestMessage
                        .Properties[RemoteEndpointMessageProperty.Name];
                return property != null ? property.Address : null;
            }
            return null;
        }

        public static string GetIP(/*this*/ HttpRequestMessage requestMessage)
        {
            // Web Hosting
            if (requestMessage.Properties.ContainsKey("MS_HttpContext"))
            {
                return HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null;
            }
            // Self Hosting
            if (requestMessage.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty property =
                    (RemoteEndpointMessageProperty)requestMessage
                        .Properties[RemoteEndpointMessageProperty.Name];
                return property != null ? property.Address : null;
            }
            return null;
        }

        public static bool AllowIP(/*this*/ HttpRequestMessage request)
        {
            var whiteListedIPs = ConfigurationManager.AppSettings["WhiteListedIPAddresses"];
            if (!string.IsNullOrEmpty(whiteListedIPs))
            {
                var whiteListIPList = whiteListedIPs.Split(',').ToList();
                var ipAddressString = GetIP(request);
                var ipAddress = IPAddress.Parse(ipAddressString);
                var isInwhiteListIPList =
                    whiteListIPList
                        .Where(a => a.Trim()
                            .Equals(ipAddressString, StringComparison.InvariantCultureIgnoreCase))
                        .Any();
                return isInwhiteListIPList;
            }
            return true;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (AllowIP(request))
            {
                return await base.SendAsync(request, cancellationToken);
            }
            return request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not authorized to view/access this resource");
        }
    }

    public class rows
    {
        public elements[] elements { get; set; }
    }

    public class elements
        {
        public distance distance { get; set; }
        public duration duration { get; set; }
        public string status { get; set; }
    }

    public class distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class GMapsDistanceMatrix
    {
        public string[] destination_addresses { get; set; }

        public string[] origin_addresses { get; set; }

       public rows[] rows { get; set; }

    public string status { get; set; }
}
}