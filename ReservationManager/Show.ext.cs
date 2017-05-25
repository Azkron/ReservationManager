using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationManager
{
    public partial class Show
    {

        public List<Category> GetCategories()
        {
            List<Category> li = new List<Category>();
            foreach (PriceList p in PriceLists)
                li.Add(p.Category);

            return li;
        }

        public void RefreshStrings()
        {
            freePlacesString = "";
            prices = "";
        }

        private string freePlacesString = "";
        public string FreePlacesString
        {
            get
            {
                if (freePlacesString == "")
                    foreach(Category cat in GetCategories())
                        freePlacesString += cat.Name.Substring(3) + "/" + CalcFreePlacesByCat(cat) + "  ";

                return freePlacesString;
            }
        }

        private int freePlacesTotal = -1;
        public int FreePlacesTotal
        {
            get
            {
                if (freePlacesTotal == -1)
                {
                    freePlacesTotal = 0;
                    foreach(Category c in App.Model.Categories)
                        freePlacesTotal += (int)CalcFreePlacesByCat(c);
                }

                return freePlacesTotal;
            }
        }

        public int ReservationsCount(int IdCat)
        {
                return (from r in Reservations where r.IdCat == IdCat select r).Count();
        }



        public decimal CalcFreePlacesByCat(Category cat)
        {
            decimal placesUsed = (from r in Reservations where r.IdCat == cat.Id && r.IdShow == Id select r.Number).Sum();

            return cat.PlacesNumber - placesUsed;
        }
        
        public int? GetPrice(int idCat)
        {
            PriceList priceList = (from p in PriceLists where p.IdCat == idCat select p).FirstOrDefault(null);

            if (priceList == null)
                return null;
            else
                return (int?)priceList.Price;
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
