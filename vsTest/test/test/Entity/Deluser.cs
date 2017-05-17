using System;
using System.Data;
using System.Text;
using System.Data.OracleClient;

namespace test.Entity
{
    public class Deluser
    {
        private test.Framwork.Data.DBOracelHelper mDbHelp = null;

        public Deluser(test.Framwork.Data.DBOracelHelper help)
        {
            mDbHelp = help;
        }

        public bool deluser(string username)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from atest_user ");
            strSql.Append(" where USERNAME=:USERNAME ");
            OracleParameter[] parameters = {
					new OracleParameter(":USERNAME", OracleType.VarChar,50)			};
            parameters[0].Value = username;

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