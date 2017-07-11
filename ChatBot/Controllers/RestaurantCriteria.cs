using Microsoft.Bot.Connector;

namespace ChatBot.Controllers
{
	public class RestaurantCriteria
	{
		public string cateringBudget = string.Empty;
		public string chineseRestaurant = string.Empty;
		public string frenchRestaurant = string.Empty;
		public string italianRestaurant = string.Empty;
		public string malteseTraditional = string.Empty;
		public string mediterraneanRestaurant = string.Empty;
		public string orientalRestaurant = string.Empty;
		public string goodFishRestaurant = string.Empty;
		public string wideChoiceOfWinesRestaurant = string.Empty;
		public string spicyFoodRestaurant = string.Empty;
		public string tableAllocation = string.Empty;
		public Location place = new Location();
		public Location userLocation = new Location();

		//public string stringPrice = string.Empty;
		public string mykeywords=string.Empty;

		public bool initialized = false;
		public int counter = 0;
		private bool tableReservationImplemented = false;

		public RestaurantCriteria()
		{
			
		}

		public RestaurantCriteria(string cateringBudget, string chineseRestaurant, string frenchRestaurant, string italianRestaurant, string malteseTraditional, string mediterraneanRestaurant,
			string orientalRestaurant, string goodFishRestaurant, string wideChoiceOfWinesRestaurant, string spicyFoodRestaurant, string tableAllocation, Location place, Location userLocation, int counter)
		{
			this.cateringBudget = cateringBudget;
			this.chineseRestaurant = chineseRestaurant;
			this.frenchRestaurant = frenchRestaurant;
			this.italianRestaurant = italianRestaurant;
			this.malteseTraditional = malteseTraditional;
			this.mediterraneanRestaurant = mediterraneanRestaurant;
			this.orientalRestaurant = orientalRestaurant;
			this.goodFishRestaurant = goodFishRestaurant;
			this.wideChoiceOfWinesRestaurant = wideChoiceOfWinesRestaurant;
			this.spicyFoodRestaurant = spicyFoodRestaurant;
			this.tableAllocation = tableAllocation;
			this.place = place; this.userLocation = userLocation;

			this.counter = counter;

			// arbitrary criteria for restaurant rating
			/*if (!string.IsNullOrEmpty(cateringBudget))
			{
				if (cateringBudget.Contains("free meal"))
					stringPrice = "&maxprice=0";
				else if (cateringBudget.Contains("cheapest"))
					stringPrice = "&maxprice=1";
				else if(cateringBudget.Contains("cheap"))
					stringPrice = "&maxprice=2";
				else if (cateringBudget.Contains("do not want to spend much"))
					stringPrice = "&maxprice=3";
				else if (cateringBudget.Contains("best") || cateringBudget.Contains("most expensive")) 
					stringPrice = "&minprice=3";
			} not in use in google maps */

			
			if(counter>0) {
				mykeywords = "&keyword=";
				if(!string.IsNullOrEmpty(chineseRestaurant) && chineseRestaurant.Contains("chinese"))
					mykeywords += "(chinese)";
				if(!string.IsNullOrEmpty(frenchRestaurant) && frenchRestaurant.Contains("french")) { 
					if(!string.IsNullOrEmpty(chineseRestaurant))
						mykeywords += " AND ";
					mykeywords += "(french)";
				}
				if (!string.IsNullOrEmpty(italianRestaurant) && italianRestaurant.Contains("italian"))
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant))
						mykeywords += " AND ";
					mykeywords += "(italian)";
				}
				if (!string.IsNullOrEmpty(malteseTraditional) && malteseTraditional.Contains("maltese"))
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant) || !string.IsNullOrEmpty(italianRestaurant))
						mykeywords += " AND ";
					mykeywords += "(maltese)";
				}
				if (!string.IsNullOrEmpty(mediterraneanRestaurant) && mediterraneanRestaurant.Contains("mediterranean"))
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant) || 
						!string.IsNullOrEmpty(italianRestaurant) || !string.IsNullOrEmpty(malteseTraditional))
						mykeywords += " AND ";
					mykeywords += "(mediterranean)";
				}
				if (!string.IsNullOrEmpty(orientalRestaurant) && orientalRestaurant.Contains("oriental"))
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant) ||
						!string.IsNullOrEmpty(italianRestaurant) || !string.IsNullOrEmpty(malteseTraditional)
						|| !string.IsNullOrEmpty(mediterraneanRestaurant))
						mykeywords += " AND ";
					mykeywords += "(oriental)";
				}
				if (!string.IsNullOrEmpty(goodFishRestaurant) && goodFishRestaurant.Contains("fish"))
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant) ||
						!string.IsNullOrEmpty(italianRestaurant) || !string.IsNullOrEmpty(malteseTraditional)
						|| !string.IsNullOrEmpty(mediterraneanRestaurant) || !string.IsNullOrEmpty(orientalRestaurant))
						mykeywords += " AND ";
					mykeywords += "(fish)";
				}
				if (!string.IsNullOrEmpty(wideChoiceOfWinesRestaurant) && wideChoiceOfWinesRestaurant.Contains("wine"))
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant) ||
						!string.IsNullOrEmpty(italianRestaurant) || !string.IsNullOrEmpty(malteseTraditional)
						|| !string.IsNullOrEmpty(mediterraneanRestaurant) || !string.IsNullOrEmpty(orientalRestaurant)
						|| !string.IsNullOrEmpty(goodFishRestaurant))
						mykeywords += " AND ";
					mykeywords += "(wine)";
				}
				if (!string.IsNullOrEmpty(spicyFoodRestaurant) && spicyFoodRestaurant.Contains("spicy"))
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant) ||
						!string.IsNullOrEmpty(italianRestaurant) || !string.IsNullOrEmpty(malteseTraditional)
						|| !string.IsNullOrEmpty(mediterraneanRestaurant) || !string.IsNullOrEmpty(orientalRestaurant)
						|| !string.IsNullOrEmpty(goodFishRestaurant) || !string.IsNullOrEmpty(wideChoiceOfWinesRestaurant))
						mykeywords += " AND ";
					mykeywords += "(spicy)";
				}
				if (!string.IsNullOrEmpty(tableAllocation) && tableReservationImplemented)
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant) ||
						!string.IsNullOrEmpty(italianRestaurant) || !string.IsNullOrEmpty(malteseTraditional)
						|| !string.IsNullOrEmpty(mediterraneanRestaurant) || !string.IsNullOrEmpty(orientalRestaurant)
						|| !string.IsNullOrEmpty(goodFishRestaurant) || !string.IsNullOrEmpty(wideChoiceOfWinesRestaurant)
						|| !string.IsNullOrEmpty(spicyFoodRestaurant))
						mykeywords += " AND ";
					mykeywords += "(book for "+ tableAllocation + ")";
				}
				if (!string.IsNullOrEmpty(cateringBudget) && !cateringBudget.Contains("no price limits"))
				{
					if (!string.IsNullOrEmpty(chineseRestaurant) || !string.IsNullOrEmpty(frenchRestaurant) ||
						!string.IsNullOrEmpty(italianRestaurant) || !string.IsNullOrEmpty(malteseTraditional)
						|| !string.IsNullOrEmpty(mediterraneanRestaurant) || !string.IsNullOrEmpty(orientalRestaurant)
						|| !string.IsNullOrEmpty(goodFishRestaurant) || !string.IsNullOrEmpty(wideChoiceOfWinesRestaurant)
						|| !string.IsNullOrEmpty(spicyFoodRestaurant) || !string.IsNullOrEmpty(tableAllocation))
						mykeywords += " AND ";

					if (cateringBudget.Contains("free") || cateringBudget.Contains("gratis"))
						mykeywords += "(free)";
					else if(cateringBudget.Contains("cheap")|| cateringBudget.Contains("low cost")|| cateringBudget.Contains("affordable") || cateringBudget.Contains("do not want to spend much"))
						mykeywords += "(cheap)";
					else if (cateringBudget.Contains("average") || cateringBudget.Contains("not too expensive") || cateringBudget.Contains("reasonable price"))
						mykeywords += "(average)";
					else if (cateringBudget.Contains("expensive") || cateringBudget.Contains("best choice") || cateringBudget.Contains("costly"))
						mykeywords += "(expensive)";
				}
			}
			initialized = true;
		}
	}
}