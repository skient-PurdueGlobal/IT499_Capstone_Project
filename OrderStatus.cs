using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace IT499_Capstone_Project
{
    public class OrderStatus
    {
        #region Class Variables
        static DatabaseAccess dbAccess;
        public string dbStatus;
        public appDataSet ds;
        #endregion

        #region Class Constructors
        public OrderStatus()
        {
            //Initialize and load database.
            dbAccess = new DatabaseAccess();
            dbStatus = dbAccess.dbStatus;
            ds = dbAccess.Load_Database();
            if (ds == null)
                dbStatus = "Database has not loaded correctly!!!";
            else dbStatus = "Connected Successfully.";
        }
        #endregion

        #region Class Methods
        public string Load_Orders(string email)
        {
            //LINQ query to merge datatables for viewing.
            var query = from tbl_1 in ds.order_details.AsEnumerable()
                        join tbl_2 in ds.user.AsEnumerable() on (int)tbl_1["user_id"] equals (int)tbl_2["id"]
                        where tbl_2.email == email
                        select new
                        {
                            OrderID = tbl_1["id"],
                            TotalPrice = tbl_1["total_price"],
                            HasShipped = tbl_1["has_shipped"],
                            Email = tbl_2["email"]
                        };

            //Save query to datatable.
            DataTable dt = new DataTable();
            DataRow dr = null;

            dt.Columns.Add("OrderID");
            dt.Columns.Add("TotalPrice");
            dt.Columns.Add("HasShipped");
            dt.Columns.Add("Email");

            foreach (var rowObj in query)
            {
                dr = dt.NewRow();
                dt.Rows.Add(rowObj.OrderID, rowObj.TotalPrice, rowObj.HasShipped, rowObj.Email);
            }

            return Serialize_Data(dt);
        }
        public string Serialize_Data(DataTable dt)
        {
            string jsonString = "";

            jsonString = JsonConvert.SerializeObject(dt);

            return jsonString;
        }
        public static bool Cancel_Order(int order)
        {
            bool isCanceled = false;



            return isCanceled;
        }
        #endregion
    }
}
