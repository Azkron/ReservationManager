using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationManager
{
    public partial class Reservation
    {
        public string TabHeader { get { return "<" + TabHeaderPseudo + ">"; } }

        public string TabHeaderPseudo
        {
            get
            {
                if (Client != null)
                    return Client.FullName + " reservation";
                else
                    return "new reservation";
            }
        }

    }
}
