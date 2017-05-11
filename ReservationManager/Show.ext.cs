using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationManager
{
    public partial class Show
    {

        private List<Category> GetCategories()
        {
            List<Category> li = new List<Category>();
            foreach (PriceList p in PriceLists)
                li.Add(p.Category);

            return li;
        }

        private string freePlaces = "";
        public string FreePlaces
        {
            get
            {
                if (freePlaces == "")
                    foreach(Category cat in GetCategories())
                        freePlaces += cat.Name.Substring(3) + "/" + CalcFreePlacesByCat(cat) + "  ";

                return freePlaces;
            }
        }


        private decimal CalcFreePlacesByCat(Category cat)
        {
            decimal placesUsed = (from r in Reservations where r.IdCat == cat.Id select r.Number).Sum();

            return cat.PlacesNumber - placesUsed;
        }

        private string prices = "";
        public string Prices
        {
            get
            {
                if (prices == "")
                {
                    foreach (PriceList pl in PriceLists)
                        prices += pl.Category.Name.Substring(3) + "/" + pl.Price.ToString("G29") + "  ";
                    
                }

                return prices;
            }
        }

    }
}
