using ChatBot.Serialization;
using ChatBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace ChatBot.Controllers
{
    public class MessagesController : ApiController
	{
		static string cateringBudget = string.Empty;
		static string chineseRestaurant = string.Empty;
		static string frenchRestaurant = string.Empty;
		static string italianRestaurant = string.Empty;
		static string malteseTraditional = string.Empty;
		static string mediterraneanRestaurant = string.Empty;
		static string orientalRestaurant = string.Empty;
		static string goodFishRestaurant = string.Empty;
		static string wideChoiceOfWinesRestaurant = string.Empty;
		static string spicyFoodRestaurant = string.Empty;
		static string tableAllocation = string.Empty;
		static Location place = new Location(), userLocation=new Location();

		private static int counter = 0;
		private static bool greetingMsgsSent = false;

		private static bool showUpMatchingRestaurants = false;

		static string DecodeEncodedNonAsciiCharacters(string value)
		{
			return Regex.Replace(
				value,
				@"\\u(?<Value>[a-zA-Z0-9]{4})",
				m => {
					return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
				});
		}

		public async Task<Message> Post([FromBody]Message message)
		{
			//Message msg = new Message();
			//msg.Type = "Message";
			//msg.Created = DateTime.Now;
			//msg.ConversationId = message.ConversationId;
			//msg.Id = "123456";
			//msg.Text = message.Text;

			//return await Task.FromResult<Message>(msg);

			return await Response(message);
		}

		private static async Task<Message> Response(Message message)
		{
			Message replyMessage = new Message();
			var response = await Luis.GetResponse(message.Text);
			var compositeMessage = string.Empty;
			List<Results> matchingRestaurants = new List<Results>();

			if (greetingMsgsSent == false)
			{
				replyMessage.Text = $"Dear customer, I am gonna help you find the best choice for a restaurant." + "</br></br>" +
										"What are you in the mood for ?" + "</br>" +
										"How many people are you going to be ?" + "</br>" +
										"What is your price range ?" + "</br>" + "</br>" +
										"THANK YOU FOR YOUR ANSWERS.";
					await Conversation.SendAsync(replyMessage, () => new Dialogs.RootDialog());
					greetingMsgsSent = true;
					return replyMessage;
			}
			if (response != null)
			{
				var intent = new Intent();
				var entity = new Entity();

				var query = response.query;

				foreach (var item in response.entities)
				{
					switch (item.type)
					{
						case "ConfirmChoicesAndGetSuggestions":
							if (counter > 0 && !string.IsNullOrEmpty(tableAllocation) && !string.IsNullOrEmpty(userLocation.Name) &&
								!string.IsNullOrEmpty(place.Name))
							{
								showUpMatchingRestaurants = true;
								GeoLocatorHandler geoLocatorHandler2 = new GeoLocatorHandler();
								matchingRestaurants = geoLocatorHandler2.GetMatchingRestaurants(place, new RestaurantCriteria(cateringBudget,
									chineseRestaurant,
									frenchRestaurant, italianRestaurant, malteseTraditional, mediterraneanRestaurant,
									orientalRestaurant,
									goodFishRestaurant, wideChoiceOfWinesRestaurant, spicyFoodRestaurant,
									tableAllocation, place, userLocation, counter));
							}
							break;
						case "ClearUpPreferences":
							cateringBudget = string.Empty;
							chineseRestaurant = string.Empty;
							frenchRestaurant = string.Empty;
							italianRestaurant = string.Empty;
							malteseTraditional = string.Empty;
							mediterraneanRestaurant = string.Empty;
							orientalRestaurant = string.Empty;
							goodFishRestaurant = string.Empty;
							wideChoiceOfWinesRestaurant = string.Empty;
							spicyFoodRestaurant = string.Empty;
							tableAllocation = string.Empty;
							place = new Location();
							counter = 0;
							showUpMatchingRestaurants = false;
							break;
						case "User location":
							var geoLocatorHandler = new GeoLocatorHandler();
							var tmp2 = geoLocatorHandler.GEOCodeAddress(item.entity);
							userLocation.Latitude = tmp2.Latitude;
							userLocation.Longitude = tmp2.Longitude;
							userLocation.Name = item.entity;
							break;
						case "Place":
							var location = new GeoLocatorHandler();
							var tmp = location.GEOCodeAddress(item.entity);
							place.Latitude = tmp.Latitude;
							place.Longitude = tmp.Longitude;
							place.Name = item.entity;
							break;
						case "Catering budget":
							cateringBudget = item.entity;
							counter++;
							break;
						case "Chinese restaurant":
							chineseRestaurant = item.entity;
							counter++;
							break;
						case "French restaurant":
							frenchRestaurant = item.entity;
							counter++;
							break;
						case "Italian restaurant":
							italianRestaurant = item.entity;
							counter++;
							break;
						case "Maltese traditional":
							malteseTraditional = item.entity;
							counter++;
							break;
						case "Mediterranean restaurant":
							mediterraneanRestaurant = item.entity;
							counter++;
							break;
						case "Oriental restaurant":
							orientalRestaurant = item.entity;
							counter++;
							break;
						case "Restaurant which serves fresh fish":
							goodFishRestaurant = item.entity;
							counter++;
							break;
						case "Restaurant with a wide choice of wines":
							wideChoiceOfWinesRestaurant = item.entity;
							counter++;
							break;
						case "Spicy food restaurant":
							spicyFoodRestaurant = item.entity;
							counter++;
							break;
						case "Table allocation":
							tableAllocation = item.entity;
							break;
					}
				}

				if (showUpMatchingRestaurants == true)
				{
					compositeMessage = "Follows the matching restaurant(s) according to your preferences: </br>";
					foreach(var item in matchingRestaurants)
					{
						var distance = item.distance.Replace(",", ".");
						var photo = DecodeEncodedNonAsciiCharacters(item.photos[0].html_attributions[0]);
						var vicinity = string.IsNullOrEmpty(item.vicinity) ? string.Empty : " nearby "+ Encoding.UTF8.GetString(Encoding.Default.GetBytes(item.vicinity));

						byte[] bytes = Encoding.Default.GetBytes(item.name);
						var restaurantName = Encoding.UTF8.GetString(bytes);

						compositeMessage += restaurantName + ", "+distance+", "+item.rating.ToString().Replace(",", ".") + ", "+ "<a href='https://www.google.com/maps/search/" + restaurantName + vicinity + "'>Map</a></br>";
					}
				}
				else { 
				compositeMessage = "Follows criteria you choose for the restaurant: </br>";
				if (!string.IsNullOrEmpty(place.Name))
				{
					compositeMessage += "location and neighbourhood area) " + place.Name + "</br>";
				}
				if (!string.IsNullOrEmpty(cateringBudget))
				{
					compositeMessage += "budget) " + cateringBudget+"</br>";
				}
				if (!string.IsNullOrEmpty(chineseRestaurant))
				{
					compositeMessage += "restaurant type) " + chineseRestaurant + "</br>";
				}
				if (!string.IsNullOrEmpty(frenchRestaurant))
				{
					compositeMessage += "restaurant type) {frenchRestaurant}</br>";
				}
				if (!string.IsNullOrEmpty(italianRestaurant))
				{
					compositeMessage += "restaurant type) " + italianRestaurant + "</br>";
				}
				if (!string.IsNullOrEmpty(malteseTraditional))
				{
					compositeMessage += "restaurant type) " + malteseTraditional + "</br>";
				}
				if (!string.IsNullOrEmpty(mediterraneanRestaurant))
				{
					compositeMessage += "restaurant type) " + mediterraneanRestaurant + "</br>";
				}
				if (!string.IsNullOrEmpty(orientalRestaurant))
				{
					compositeMessage += "restaurant type) " + orientalRestaurant + "</br>";
				}
				if (!string.IsNullOrEmpty(goodFishRestaurant))
				{
					compositeMessage += "restaurant type) " + goodFishRestaurant + "</br>";
				}
				if (!string.IsNullOrEmpty(wideChoiceOfWinesRestaurant))
				{
					compositeMessage += "restaurant type) " + wideChoiceOfWinesRestaurant + "</br>";
				}
				if (!string.IsNullOrEmpty(spicyFoodRestaurant))
				{
					compositeMessage += "restaurant type) " + spicyFoodRestaurant + "</br>";
				}
				if (!string.IsNullOrEmpty(tableAllocation))
				{
					compositeMessage += "restaurant table allocation) " + tableAllocation;
				}
				}
				if (counter == 0)
					replyMessage = message.CreateReplyMessage("I do not understand what you say. Search on <a href='http://www.google.com'>Google</a>");
				else replyMessage = message.CreateReplyMessage(compositeMessage+".");
			}
			return replyMessage;
		}
	}
}
