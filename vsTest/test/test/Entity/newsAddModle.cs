using System;
using System.Data;
using System.Text;
using System.Data.OracleClient;

namespace test.Entity
{
    public class newsAddModle
    {
        private test.Framwork.Data.DBOracelHelper mDbHelp = null;

        public newsAddModle(test.Framwork.Data.DBOracelHelper help)
        {
            mDbHelp = help;
        }

        /// <summary>
        /// 新闻录入页增加一条数据
        /// </summary>
        public bool Add(test.Entity.newsAdd model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into atest_news(");
            strSql.Append("NEWS_TITLE,NEWS_TYPE,NEWS_SUMMARY,NEWS_CONTENT)");
            strSql.Append(" values (");
            strSql.Append(":NEWS_TITLE,:NEWS_TYPE,:NEWS_SUMMARY,:NEWS_CONTENT)");
            OracleParameter[] parameters = {
					new OracleParameter(":NEWS_TITLE", OracleType.VarChar,50),
					new OracleParameter(":NEWS_TYPE", OracleType.VarChar,50),
					new OracleParameter(":NEWS_SUMMARY", OracleType.VarChar,50),
					new OracleParameter(":NEWS_CONTENT", OracleType.VarChar,2000)};
            parameters[0].Value = model.NEWS_TITLE;
            parameters[1].Value = model.NEWS_TYPE;
            parameters[2].Value = model.NEWS_SUMMARY;
            parameters[3].Value = model.NEWS_CONTENT;
            

            int rows = mDbHelp.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}