using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationManager
{
    public partial class Show
    {

        public List<Category> GetCategories(bool includeFull = true, Reservation res = null)
        {
            List<Category> li = new List<Category>();
            foreach (PriceList p in PriceLists)
                if(includeFull || CalcFreePlacesByCat(p.Category,res) > 0)
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
                //if (freePlacesString == "")
                freePlacesString = "";
                foreach (Category cat in GetCategories())
                    freePlacesString += cat.Name.Substring(3) + "/" + CalcFreePlacesByCat(cat) + "  ";

                return freePlacesString;
            }
        }

        public string GetFreePlacesStr(Reservation r = null)
        {

            freePlacesString = "";
            foreach (Category cat in GetCategories())
                freePlacesString += cat.Name.Substring(3) + "/" + CalcFreePlacesByCat(cat, r) + "  ";

            return freePlacesString;
        }


        public int FreePlacesTotal
        {
            get
            {
                var freePlacesTotal = 0;
                foreach (Category c in GetCategories())
                    freePlacesTotal += (int)CalcFreePlacesByCat(c);

                return freePlacesTotal;
            }
        }

        public int GetFreePlacesTotal(Reservation r = null)
        {

            var freePlacesTotal = 0;
            foreach (Category c in GetCategories())
                freePlacesTotal += (int)CalcFreePlacesByCat(c, r);

            return freePlacesTotal;
        }


        public int ReservationsCount(int IdCat)
        {
            return (from r in Reservations where r.IdCat == IdCat select r).Count();
        }

        /*private int dCatId = -1;
        private int dNum = 0;

        public void DiscountResPlaces(int catId, int num)
        {
            dCatId = catId;
            dNum = num;
        }*/


        public decimal CalcFreePlacesByCat(Category cat, Reservation res = null)
        {
            decimal placesUsed = (from r in Reservations where r.IdCat == cat.Id && r.IdShow == Id && r != res select r.Number).Sum();

            var freePlaces = (cat.PlacesNumber - placesUsed);
            return freePlaces;
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
                prices = "";
                //if (prices == "")
                {
                    foreach (PriceList pl in PriceLists)
                        prices += pl.Category.Name.Substring(3) + "/" + pl.Price.ToString("G29") + "  ";

                }

                return prices;
            }
        }

    }
}
