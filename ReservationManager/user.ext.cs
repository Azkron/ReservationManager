using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace ReservationManager
{
    public partial class User
    {
        private static bool rightsInitialized = false;
        private static Dictionary<Table, Rights> adminRights = new Dictionary<Table, Rights>();
        private static Dictionary<Table, Rights> sellerRights = new Dictionary<Table, Rights>();

        public User()
        {

        }

        public Rights GetRights(Table table) // GetRights(Table.bla)
        {
            if (!rightsInitialized)
                InitRights();

            if (Admin == 1)
                return adminRights[table];
            else
                return sellerRights[table];
        }

        private void InitRights()
        {
            rightsInitialized = true;

            adminRights[Table.SHOW] = Rights.ALL;
            adminRights[Table.CLIENT] = Rights.READ;
            adminRights[Table.RESERVATION] = Rights.READ;
            adminRights[Table.CATEGORY] = Rights.READ;
            adminRights[Table.PRICE] = Rights.ALL;

            sellerRights[Table.SHOW] = Rights.READ;
            sellerRights[Table.CLIENT] = Rights.ALL;
            sellerRights[Table.RESERVATION] = Rights.ALL;
            sellerRights[Table.CATEGORY] = Rights.READ;
            sellerRights[Table.PRICE] = Rights.READ;
        }
    }
}
