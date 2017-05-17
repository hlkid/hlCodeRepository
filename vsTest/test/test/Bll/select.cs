using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace test.Bll
{
    public class select
    {
        public DataTable usermange(string username)
        {
            string sql = "";

            sql = sql + "select t.username,t.pass,t.role,t.phone_num from atest_user t where 1=1";
            if (username != "") {
                sql = sql + "and t.username='" + username + "'";
            }
            

            return DefaultData.QueryData(sql);
        }

        //检查用户名
        public DataTable checkuser(string checkuser)
        {
            string sql = "";

            sql = sql + "select * from atest_user where username='" + checkuser + "'";


            return DefaultData.QueryData(sql);
        }

        //显示新闻
        public DataTable newsCheck(string newstype)
        {
            string sql = "";

            sql = sql + "select * from atest_news where NEWS_TYPE='" + newstype + "'";


            return DefaultData.QueryData(sql);
        }

        //用户登陆
        public DataTable login(string username, string password)
        {
            string sql = "";

            sql = sql + "select * from atest_user where username='" + username + "'";
            sql = sql + " and pass='" + password + "'";

            return DefaultData.QueryData(sql);
        }
    }
}