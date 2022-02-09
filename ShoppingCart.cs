using System;
using System.Collections.Generic;
using System.Text;

namespace IT499_Capstone_Project
{
    public class ShoppingCart
    {
        #region Class Variables
        static DatabaseAccess dbAccess;
        public string dbStatus;
        public appDataSet ds;
        #endregion

        #region Constructors
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
    }
}
