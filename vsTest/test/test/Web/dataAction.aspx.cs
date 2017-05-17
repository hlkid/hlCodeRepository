using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using test.Framwork.IO;
using System.IO;
using System.Data;
using System.Collections.Specialized;
using System.Text;

namespace test.Web
{
    public partial class dataAction : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request.QueryString["action"];
            string parma = Request.QueryString["parma"];
            string username = Request.QueryString["username"];
            string role = Request.QueryString["role"];
            string password = Request.QueryString["userpassword"];
            string phonenum = Request.QueryString["phonenum"];
            string checkuser = Request.QueryString["checkuser"];

            //新闻录入参数
            string newstitle = Request.QueryString["newstitle"];
            string newstype = Request.QueryString["newstype"];
            string newssummary = Request.QueryString["newssummary"];
            string newscontent = Request.QueryString["newscontent"];
            string newsimage = Request.QueryString["newsimage"];
            
            int iphonenum = 0;
            if (!int.TryParse(phonenum, out iphonenum))
                iphonenum = 1;

            
            if (!string.IsNullOrEmpty(action))
            {
                switch (action)
                {
                    case "login":
                        Response.Write(login(username,password));
                        break;
                    case "register"://注册
                        Response.Write(register(username, password, iphonenum));
                        break;
                    case "usermange"://用户管理
                        Response.Write(usermange(username));
                        break;
                    case "deluser"://用户管理
                        Response.Write(deluser(username));
                        break;
                    case "userupdata"://更新用户资料
                        Response.Write(userupdata(username,password,role,iphonenum));
                        break;
                    case "check"://检查
                        Response.Write(check(checkuser));
                        break;
                    case "newsadd"://新闻录入
                        Response.Write(newsadd(newstitle, newstype, newssummary, newscontent));
                        break;
                    case "newshow"://新闻展示
                        Response.Write(newshow(newstype));
                        break;
                    
                }
            }
            Response.End(); //停止Response后续写入动作，保证Response内只有我们写入内容
        }
        

        //登陆页面
        private string login(string username,string password)
        {
            string result = "{\"state\":\"fail\"}";
            DataTable dt = null;
            Bll.select bll = new Bll.select();

            dt = bll.login(username,password);

            if (dt != null && dt.Rows.Count > 0)
                result = Framwork.Data.JsonHelper.SerializeObject(dt);


            return result;
        }

        //用户管理页面
        private string usermange(string username)
        {
            string result = "{\"state\":\"fail\"}";
            DataTable dt = null;
            Bll.select bll = new Bll.select();

            dt = bll.usermange(username);

            if (dt != null && dt.Rows.Count > 0)
                result = Framwork.Data.JsonHelper.SerializeObject(dt);


            return result;
        }

        //删除用户
        private string deluser(string username)
        {
            string result = "{\"state\":\"fail\"}";
            Bll.delelt bll = new Bll.delelt();
            if (bll.deluser(username))
                result = "[{\"state\":\"success\"}]";
                
            //result = "[{'state':'fail'}]";
            return result;
        }

        //更新用户资料
        private string userupdata(string username,string password,string role,int iphonenum)
        {
            string result = "{\"state\":\"fail\"}";
            Bll.updata bll = new Bll.updata();
            if (bll.userupdata(username,password,role,iphonenum))
                result = "[{\"state\":\"success\"}]";
                
            //result = "[{'state':'fail'}]";
            return result;
        }

        //注册页面
        private string register(string username, string password, int iphonenum)
        {
            string result = "{\"state\":\"fail\"}";
            //数据入库
            Bll.insert bll = new Bll.insert();
            Entity.UserAdd mod = new Entity.UserAdd();

            mod.USER = username;
            mod.PASS = password;
            mod.PHONE = iphonenum;

            if (bll.userInsert(mod))
                result = "[{\"state\":\"success\"}]";

            return result;
        }

        //检查用户名
        private string check(string checkuser)
        {
            string result = "{\"state\":\"fail\"}";
            DataTable dt = null;
            Bll.select bll = new Bll.select();

            dt = bll.checkuser(checkuser);

            if (dt != null && dt.Rows.Count > 0)
                result = Framwork.Data.JsonHelper.SerializeObject(dt);
            

            return result;
        }

        //新闻录入页面
        private string newsadd(string newstitle,string newstype,string newssummary,string newscontent)
        {
            string result = "{\"state\":\"fail\"}";


            //数据入库
            Bll.insert bll = new Bll.insert();
            Entity.newsAdd mod = new Entity.newsAdd();

            mod.NEWS_TITLE = newstitle;
            mod.NEWS_TYPE = newstype;
            mod.NEWS_SUMMARY = newssummary;
            mod.NEWS_CONTENT = newscontent;
            
            if (bll.newsAdd(mod))
                result = "[{\"state\":\"success\"}]";

            return result;
        }

        //显示新闻
        private string newshow(string newstype)
        {
            string result = "{\"state\":\"fail\"}";
            DataTable dt = null;
            Bll.select bll = new Bll.select();

            dt = bll.newsCheck(newstype);

            if (dt != null && dt.Rows.Count > 0)
                result = Framwork.Data.JsonHelper.SerializeObject(dt);


            return result;
        }
        
    }
}