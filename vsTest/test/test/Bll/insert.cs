using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using test.Framwork.Data;

namespace test.Bll
{
    public class insert
    {
        //添加用户
        public bool userInsert(Entity.UserAdd mod)
        {
            bool result = false;

            DBOracelHelper help = null;
            //数据入库
            try
            {
                help = new DBOracelHelper("HlkidConnection");
                help.ConnectDB();
                Entity.useraddModle dal = new Entity.useraddModle(help);
                result = dal.Add(mod);
            }
            catch
            {
            }
            finally
            {
                help.DisConnectDB();
            }
            return result;
        }


        //添加新闻
        public bool newsAdd(Entity.newsAdd mod)
        {
            bool result = false;

            DBOracelHelper help = null;
            //数据入库
            try
            {
                help = new DBOracelHelper("HlkidConnection");
                help.ConnectDB();
                Entity.newsAddModle dal = new Entity.newsAddModle(help);
                result = dal.Add(mod);
            }
            catch
            {
            }
            finally
            {
                help.DisConnectDB();
            }
            return result;
        }

    }
}