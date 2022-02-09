using System;
using System.Linq;

namespace IT499_Capstone_Project
{
    public class DatabaseAccess
    {
        #region Class Variables
        public string dbStatus = "";
        #endregion

        #region Class Constructors
        public DatabaseAccess() 
        {
            //Update database directory path.
            string path = Environment.CurrentDirectory;
            path = path.Replace("\\bin\\Debug\\netcoreapp3.1", "");
            path = path.Replace("IT499_Test_App", "IT499_Capstone_Project");
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            //dbStatus = ("*****" + AppDomain.CurrentDomain.GetData("DataDirectory") + Environment.NewLine);
        }
        #endregion

        #region Class Methods
        public appDataSet Load_Database()
        {
            appDataSet ds = new appDataSet();

            //Define table adapters.
            appDataSetTableAdapters.userTableAdapter userTA = new appDataSetTableAdapters.userTableAdapter();
            appDataSetTableAdapters.productsTableAdapter productsTA = new appDataSetTableAdapters.productsTableAdapter();
            appDataSetTableAdapters.order_detailsTableAdapter ordDetailsTA = new appDataSetTableAdapters.order_detailsTableAdapter();
            appDataSetTableAdapters.order_itemsTableAdapter ordItemsTA = new appDataSetTableAdapters.order_itemsTableAdapter();
            appDataSetTableAdapters.saved_cartsTableAdapter savedCartTA = new appDataSetTableAdapters.saved_cartsTableAdapter();
            appDataSetTableAdapters.cart_itemTableAdapter cartItemsTA = new appDataSetTableAdapters.cart_itemTableAdapter();

            //Fill dataset.
            userTA.Fill(ds.user);
            productsTA.Fill(ds.products);
            ordDetailsTA.Fill(ds.order_details);
            ordItemsTA.Fill(ds.order_items);
            savedCartTA.Fill(ds.saved_carts);
            cartItemsTA.Fill(ds.cart_item);

            return ds;
        }
        #endregion

    }
}
