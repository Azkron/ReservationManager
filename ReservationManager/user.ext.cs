using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationManager
{
    public partial class User
    {
        private static bool rightsInitialized = false;
        private static Dictionary<Table, Right> adminRights = new Dictionary<Table, Right>();
        private static Dictionary<Table, Right> sellerRights = new Dictionary<Table, Right>();

        public User()
        {

        }

        public Right GetRights(Table table) // GetRights(Table.bla)
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

            adminRights[Table.SHOW] = Right.ALL;
            adminRights[Table.CLIENT] = Right.READ;
            adminRights[Table.RESERVATION] = Right.READ;
            adminRights[Table.CATEGORY] = Right.READ;
            adminRights[Table.PRICE] = Right.ALL;

            sellerRights[Table.SHOW] = Right.READ;
            sellerRights[Table.CLIENT] = Right.ALL;
            sellerRights[Table.RESERVATION] = Right.ALL;
            sellerRights[Table.CATEGORY] = Right.READ;
            sellerRights[Table.PRICE] = Right.READ;
        }
    }
}
