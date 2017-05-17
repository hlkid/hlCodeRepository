using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using test.Framwork.Data;

namespace test.Bll
{
    
    public class delelt
    {
        
        public bool deluser(string username)
        {
            bool result = false;
            DBOracelHelper help = null;
            try
            {
                help = new DBOracelHelper("HlkidConnection");
                help.ConnectDB();
                Entity.Deluser dal = new Entity.Deluser(help);
                result = dal.deluser(username);
            }
            catch
            { }
            finally
            {
                help.DisConnectDB();
            }
            return result;
        }
    }
}