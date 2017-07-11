namespace ChatBot
{
    public class geometry
    {
        public string location_type { get; set; }
        public location location { get; set; }
    }

    public class photos
    {
        public int height { get; set; }
        public string[] html_attributions { get; set; }
        public string photo_reference { get; set; }
        public int width { get; set; }
    }

    public class location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class address_component
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }

    class viewport
    {
        public northeast northeast { get; set; }
        public southwest southwest { get; set; }
    }

    class bounds
    {
        public northeast northeast { get; set; }
    }

    class northeast
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    class southwest
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class Results
    {
        public string vicinity { get; set; }

        public string formatted_address { get; set; }
        public geometry geometry { get; set; }
        public string[] types { get; set; }
        public address_component[] address_components { get; set; }
        public string name { get; set; }
        public string distance { get; set; }
        public double rating { get; set; }
        public photos[] photos { get; set; }
    }
}