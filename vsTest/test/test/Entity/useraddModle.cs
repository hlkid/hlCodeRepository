using System;
using System.Data;
using System.Text;
using System.Data.OracleClient;

namespace test.Entity
{
    public class useraddModle
    {
        private test.Framwork.Data.DBOracelHelper mDbHelp = null;

        public useraddModle(test.Framwork.Data.DBOracelHelper help)
        {
            mDbHelp = help;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(test.Entity.UserAdd model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into atest_user(");
            strSql.Append("USERNAME,PASS,ROLE,PHONE_NUM)");
            strSql.Append(" values (");
            strSql.Append(":USERNAME,:PASS,:ROLE,:PHONE_NUM)");
            OracleParameter[] parameters = {
					new OracleParameter(":USERNAME", OracleType.VarChar,255),
					new OracleParameter(":PASS", OracleType.VarChar,50),
					new OracleParameter(":ROLE", OracleType.VarChar,50),
					new OracleParameter(":PHONE_NUM", OracleType.Number)};
            parameters[0].Value = model.USER;
            parameters[1].Value = model.PASS;
            parameters[2].Value = model.ROLE;
            parameters[3].Value = model.PHONE;
            

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