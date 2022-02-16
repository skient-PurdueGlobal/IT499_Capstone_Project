using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace IT499_Capstone_Project
{
    public class ShoppingCart
    {
        #region Class Variables
        static DatabaseAccess dbAccess;
        public string dbStatus;
        public appDataSet ds;
        #endregion

        #region Class Constructors
        public ShoppingCart()
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
        public DataTable Load_Inventory()
        {
            //LINQ query to merge datatables for viewing.
            var query = from tbl_1 in ds.products.AsEnumerable()
                        select new
                        {
                            InvProdID = tbl_1["id"],
                            ProdName = tbl_1["product_name"],
                            Description = tbl_1["description"],
                            UnitPrice = tbl_1["price"],
                            Quantity = 0,
                            Cost = 0
                        };

            //Save query to datatable
            DataTable dt = new DataTable();
            DataRow dr = null;

            dt.Columns.Add("InvProdID");
            dt.Columns.Add("ProdName");
            dt.Columns.Add("Description");
            dt.Columns.Add("UnitPrice");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Cost");


            foreach (var rowObj in query)
            {
                dr = dt.NewRow();
                dt.Rows.Add(rowObj.InvProdID, rowObj.ProdName, rowObj.Description, rowObj.UnitPrice, rowObj.Quantity, rowObj.Cost);
            }

            return dt;
        }
        public string Load_Cart(string email)
        {

            //Get Datatable for all inventory.
            DataTable inv = new DataTable();
            inv = Load_Inventory();

            //LINQ query to merge datatables for viewing.
            var query = from tbl_1 in ds.products.AsEnumerable()
                        join tbl_2 in ds.cart_item.AsEnumerable() on (int)tbl_1["id"] equals (int)tbl_2["product_id"]
                        join tbl_3 in ds.saved_carts.AsEnumerable() on (int)tbl_2["session_id"] equals (int)tbl_3["id"]
                        join tbl_4 in ds.user.AsEnumerable() on (int)tbl_3["user_id"] equals (int)tbl_4["id"]
                        where tbl_4.email == email
                        select new
                        {
                            ProductID = tbl_1["id"],
                            ProdName = tbl_1["product_name"],
                            Description = tbl_1["description"],
                            UnitPrice = tbl_1["price"],
                            Quantity = tbl_2["quantity"],
                            DateSaved = tbl_3["date_saved"]
                        };

            //Save query to datatable
            DataTable dt = new DataTable();
            DataRow dr = null;

            dt.Columns.Add("ProductID");
            dt.Columns.Add("ProdName");
            dt.Columns.Add("Description");
            dt.Columns.Add("UnitPrice");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("DateSaved");

            foreach (var rowObj in query)
            {
                dr = dt.NewRow();
                dt.Rows.Add(rowObj.ProductID, rowObj.ProdName, rowObj.Description, rowObj.UnitPrice, rowObj.Quantity, rowObj.DateSaved);
            }

            if (dt.Rows.Count == 0)
                return Serialize_Data(inv);
            else
            {
                //Compare tables and add inventory items to cart table if needed.
                foreach (DataRow r in inv.Rows)
                {
                    bool isFound = false;
                    string date = "";

                    foreach (DataRow c in dt.Rows)
                    {
                        if (r["InvProdID"] == c["ProductID"])
                        {
                            isFound = true;
                            date = c["DateSaved"].ToString();
                            break;
                        }
                    }

                    if (!isFound)
                    {
                        dr = dt.NewRow();
                        dt.Rows.Add(r["InvProdID"], r["ProdName"], r["Description"], r["UnitPrice"], r["Quantity"], date);
                    }
                }

                return Serialize_Data(dt);
            }
        }
        public string Serialize_Data(DataTable dt)
        {
            string jsonString = "";

            jsonString = JsonConvert.SerializeObject(dt);

            return jsonString;
        }
        public string Save_Cart(string email, string id, string quantity)
        {
            string saveStatus = "";
            bool isSaved = false;

            try
            {
                //Get User ID Number
                string userID = "";
                foreach (DataRow r in ds.user)
                {
                    if (r["email"].ToString() == email)
                        userID = r["id"].ToString();   
                }
                if (userID == "")
                    saveStatus = "User not found.  ";
                else
                {
                    //Create Saved Cart
                    appDataSet.saved_cartsRow cartRow = ds.saved_carts.Newsaved_cartsRow();
                    cartRow.id = ds.saved_carts.Rows.Count + 1;
                    cartRow.user_id = Convert.ToInt32(userID);
                    cartRow.date_saved = DateTime.Now;
                    ds.saved_carts.Rows.Add(cartRow);
                    isSaved = dbAccess.Update_Database(ds);

                    //Add Cart Items
                    foreach (appDataSet.productsRow r in ds.products.Rows)
                    {
                        if (r.id == Convert.ToInt32(id))
                        {
                            appDataSet.cart_itemRow itemRow = ds.cart_item.Newcart_itemRow();
                            itemRow.id = ds.cart_item.Rows.Count + 1;
                            itemRow.session_id = cartRow.id;
                            itemRow.product_id = Convert.ToInt32(id);
                            itemRow.quantity = Convert.ToInt32(quantity);
                            ds.cart_item.Rows.Add(itemRow);
                            isSaved = dbAccess.Update_Database(ds);
                        }
                            
                    }
                }

                saveStatus += "Shopping cart was successfully updated.";
            }
            catch (Exception ex)
            {
                saveStatus = "Shopping cart was not updated!!";
            }

            return saveStatus;
        }
        public string Update_Cart(string email, string id, string quantity)
        {
            string saveStatus = "";
            bool isSaved = false;

            try
            {
                //Get User ID Number
                string userID = "";
                foreach (DataRow r in ds.user)
                {
                    if (r["email"].ToString() == email)
                        userID = r["id"].ToString();
                }
                if (userID == "")
                    saveStatus = "User not found.  ";
                else
                {
                    //Get Saved Cart
                    string cartID = "";
                    int cartIndex = 0;
                    foreach (DataRow r in ds.saved_carts)
                    {
                        if (r["user_id"].ToString() == userID)
                        {
                            cartID = r["id"].ToString();
                            break;
                        }
                        cartIndex++;
                    }
                    ds.saved_carts[cartIndex]["date_saved"] = DateTime.Now;

                    if (cartID == "")
                        saveStatus = "Cart not found.  ";
                    else
                    {
                        //Get Cart Items to Update
                        int itemIndex = 0;
                        bool itemExists = false;
                        foreach (DataRow r in ds.cart_item)
                        {
                            if (r["session_id"].ToString() == cartID && r["product_id"].ToString() == id)
                            {
                                itemExists = true;
                                break;
                            }
                            itemIndex++;
                        }
                        if(itemExists)
                            ds.cart_item[itemIndex]["quantity"] = Convert.ToInt32(quantity);
                        else
                        {
                            appDataSet.cart_itemRow itemRow = ds.cart_item.Newcart_itemRow();
                            itemRow.id = ds.cart_item.Rows.Count + 1;
                            itemRow.session_id = Convert.ToInt32(cartID);
                            itemRow.product_id = Convert.ToInt32(id);
                            itemRow.quantity = Convert.ToInt32(quantity);
                            ds.cart_item.Rows.Add(itemRow);
                        }
                        isSaved = dbAccess.Update_Database(ds);
                    }
                }

                saveStatus += "Shopping cart was successfully updated.";
            }
            catch (Exception ex)
            {
                saveStatus = "Shopping cart was not updated!!";
            }

            return saveStatus;
        }
        #endregion
    }
}
