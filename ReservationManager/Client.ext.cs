using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationManager
{
    public partial class Client
    {

		public string FullName
        {
            get
            {
				return LastName + " " + FirstName;
            }
        }

        private int totalReservations = -1;

		public int TotalReservations
        {
            get
            {
				if(totalReservations == -1)
                    totalReservations = (from r in Reservations select r.Number).Sum();

                return totalReservations;
            }
        }
    }
}
