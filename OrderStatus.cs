using System;
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
        public DataTable View_Orders(int ordNumber)
        {
            //LINQ query to merge datatables for viewing.
            var query = from tbl_1 in ds.products.AsEnumerable()
                        join tbl_2 in ds.order_items.AsEnumerable() on (int)tbl_1["id"] equals (int)tbl_2["product_id"]
                        join tbl_3 in ds.order_details.AsEnumerable() on (int)tbl_2["order_id"] equals (int)tbl_3["id"]
                        where tbl_3.id == ordNumber
                        select new
                        {
                            ProdName = tbl_1["product_name"],
                            Desc = tbl_1["description"],
                            Price = tbl_1["price"],
                            Quantity = tbl_2["quantity"],
                            TotalPrice = tbl_3["total_price"],
                            Shipped = tbl_3["has_shipped"]
                        };

            //Save query to datatable.
            DataTable dt = new DataTable();
            DataRow dr = null;

            dt.Columns.Add("ProdName");
            dt.Columns.Add("Desc");
            dt.Columns.Add("Price");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("TotalPrice");
            dt.Columns.Add("Shipped");

            foreach (var rowObj in query)
            {
                dr = dt.NewRow();
                dt.Rows.Add(rowObj.ProdName, rowObj.Desc, rowObj.Price, rowObj.Quantity, rowObj.TotalPrice, rowObj.Shipped);
            }

            return dt;
        }
        public string Serialize_Data(DataTable dt)
        {
            string jsonString = "";

            jsonString = JsonConvert.SerializeObject(dt);

            return jsonString;
        }
        public bool Cancel_Order(int order)
        {
            bool isCanceled = false;
            int detailsIndex = 0;
            int itemsIndex = 0;

            //Get table row index for deletion.
            foreach (appDataSet.order_itemsRow r in ds.order_items)
            {
                if (r.order_id == order)
                    break;
                itemsIndex++;
            }
            foreach (appDataSet.order_detailsRow r in ds.order_details)
            {
                if (r.id == order)
                    break;
                detailsIndex++;
            }

            //Remove rows from dataset tables.
            ds.order_items[itemsIndex].Delete();
            isCanceled = dbAccess.Update_Database(ds);
            ds.order_details[detailsIndex].Delete();
            isCanceled = dbAccess.Update_Database(ds);

            return isCanceled;
        }
        #endregion
    }
}
