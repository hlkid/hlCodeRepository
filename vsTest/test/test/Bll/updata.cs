using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using test.Framwork.Data;

namespace test.Bll
{
    public class updata
    {
        public bool userupdata(string username, string password, string role, int iphonenum)
        {
            bool result = false;
            DBOracelHelper help = null;
            string sql = "";
            try
            {
                help = new DBOracelHelper("HlkidConnection");
                help.ConnectDB();

                sql = "update atest_user t";
                sql = sql + " set username='"+username+"',";
                sql = sql + " pass='"+password+"',";
                sql = sql + " role='" + role + "',";
                sql = sql + " phone_num=" + iphonenum;
                sql = sql + " where t.username='"+username+"'";
                help.ExecuteSql(sql);
                help.ExecuteSql("commit");
                    
                result = true;
                
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