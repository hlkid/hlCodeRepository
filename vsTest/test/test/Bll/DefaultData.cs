using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using test.Framwork.Data;

namespace test.Bll
{
    public class DefaultData
    {
        /// <summary>
        /// 执行查询语句返回数据表对象
        /// </summary>
        /// <param name="sqlstr">执行的SQL语句</param>
        /// <returns>数据集</returns>
        public static DataTable QueryData(string sqlstr)
        {
            DataTable result = null;

            DBOracelHelper help = null;
            try
            {
                help = new DBOracelHelper("HlkidConnection");
                help.ConnectDB();
                result = help.QueryDataTable(sqlstr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                help.DisConnectDB();
            }
            return result;
        }
        
    }
}