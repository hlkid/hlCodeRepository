using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
//using System.Data.OleDb;
//using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Configuration;
using System.Collections.Generic;

namespace test.Framwork.Data
{
    /// <summary>
    /// Copyright (C) 2004-2008 
    /// 数据访问基础类(基于SQLServer)
    /// </summary>
    public class DBOracelHelper
    {
        protected string mConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        protected int cTimeOut = 30000;//Convert.ToInt32(ConfigurationSettings.AppSettings["ConnectTimeOut"]);
        protected string mConnectKey = "";

        //数据库连接字符串(web.config来配置)
        //<add key="ConnectionString" value="server=127.0.0.1;database=DATABASE;uid=sa;pwd=" /> 
        //protected string connectionString = ConfigurationSettings.AppSettings["ConnectionString"];
        /// <summary>
        /// 全局数据连接
        /// </summary>
        protected OracleConnection mConn = null;
        /// <summary>
        /// 全局事务处理
        /// </summary>
        protected OracleTransaction mTran = null;

        #region 属性
        /// <summary>
        /// 全局数据连接
        /// </summary>
        public OracleConnection Connection
        {
            get
            {
                return mConn;
            }
            set
            {
                mConn = value;
            }
        }
        #endregion

        public DBOracelHelper()
        {
            mConnectKey = "DefaultConnection";
            mConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            cTimeOut = 30000;
        }

        public DBOracelHelper(string connectkey)
        {
            mConnectKey = connectkey;
            mConnectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[connectkey].ToString();
            cTimeOut = 30000;
        }
        public int TimeOut
        {
            get { return cTimeOut; }
        }

        /// <summary>
        /// 向数据库中更新一个 DataTable
        /// 本函数没有任何错误处理，调用模块必须对其进行处理
        /// </summary>
        /// <param name="dt">包含数据的DataTable</param>
        /// <param name="select_sql">一个select 结构的sql语句</param>
        public void UpdateDataTable(DataTable dt, string select_sql)
        {
            OracleCommand comm = mConn.CreateCommand();
            comm.CommandText = select_sql;
            OracleDataAdapter adapter = new OracleDataAdapter();
            adapter.SelectCommand = comm;

            OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
            adapter.Update(dt);
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns>成功返回true，否则返回false</returns>
        public bool ConnectDB()
        {
            try
            {
                mConn = new OracleConnection(mConnectionString);
                mConn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, params OracleParameter[] cmdParms)
        {//循环处理前台SQL参数
            //前台实体的属性为空不能插入数据库
            //此处把其改为数据库默认的NULL
            foreach (OracleParameter OracleParameter in cmdParms)
            {
                if (OracleParameter.Value == null)
                {
                    OracleParameter.Value = DBNull.Value;
                }
            }
            using (OracleConnection connection = new OracleConnection(mConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    // cmd.CommandTimeout = cTimeOut;
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 断开数据库连接
        /// </summary>
        /// <returns>无论是否执行断开成功，都返回TRUE</returns>
        public bool DisConnectDB()
        {
            try
            {
                mConn.Close();
            }
            catch
            {

            }
            return true;
        }

        /// <summary>
        /// 显示开始事务
        /// </summary>
        /// <returns>成功返回true，否则返回false</returns>
        public bool BeginTransaction()
        {
            if (mConn.State != ConnectionState.Open)
                return false;
            try
            {
                mTran = mConn.BeginTransaction();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 执行提交事务,如果失败会直接调用事务回滚
        /// </summary>
        /// <returns>成功返回true，否则返回false</returns>
        public bool Commit()
        {
            if ((mTran == null) || (mTran.Connection == null))
                return false;
            try
            {
                mTran.Commit();
                mTran = null;
                return true;
            }
            catch
            {
                try
                {
                    mTran.Rollback();
                    mTran = null;
                }
                catch
                { }
                return false;
            }
        }

        /// <summary>
        /// 显示回滚事务
        /// </summary>
        /// <returns>成功返回true，否则返回false</returns>
        public bool Rollback()
        {
            if (mTran == null)
                return false;
            try
            {
                mTran.Rollback();
                mTran = null;
                return true;
            }
            catch
            {
                return false;
            }
        }



        #region    返回最大的ID
        /// <summary>
        /// 返回最大的ID
        /// </summary>
        /// <param name="FieldName">ID</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        #endregion

        #region    返回最大的ID
        /// <summary>
        /// 返回最大的ID
        /// </summary>
        /// <param name="FieldName">ID</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public int GetMaxID(string FieldName, string TableName, string strWhere)
        {
            string strsql = "select max(RegNum) from (select " + FieldName + " as RegNum from " + TableName;
            if (!String.IsNullOrEmpty(strWhere))
            {
                strsql += " where " + strWhere;
            }
            strsql += ") as t where isnumeric(t.RegNum) = 1";
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        #endregion

        #region    返回数据行数
        /// <summary>
        /// 返回数据行数
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public int GetRowCount(string TableName, string where)
        {
            string strsql = "select count(*) from " + TableName;
            if (!String.IsNullOrEmpty(where))
            {
                strsql += " where " + where;
            }
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        #endregion

        #region    返回数据行数
        /// <summary>
        /// 返回数据行数
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public int GetRowCount(string coungString)
        {
            object obj = GetSingle(coungString);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        #endregion

        #region  是否存在该记录
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public bool Exists(string strSql, params OracleParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion




        #region 执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString)
        {
            using (OracleCommand cmd = new OracleCommand(SQLString, mConn))
            {
                // cmd.CommandTimeout = cTimeOut;
                try
                {
                    if (mConn.State == ConnectionState.Closed)
                    {
                        mConn.Open();
                    }
                    if ((mTran != null) && (mTran.Connection != null))
                        cmd.Transaction = mTran;
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }

            }

        }
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="fs">image字段的buf</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, byte[] fs)
        {
            using (OracleCommand cmd = new OracleCommand(SQLString, mConn))
            {
                //  cmd.CommandTimeout = cTimeOut;
                OracleParameter myParameter = new OracleParameter("@fs", OracleType.Blob);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    if (mConn.State == ConnectionState.Closed)
                    {
                        mConn.Open();
                    }
                    if ((mTran != null) && (mTran.Connection != null))
                        cmd.Transaction = mTran;
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }

            }

        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params OracleParameter[] cmdParms)
        {
            //循环处理前台SQL参数
            //前台实体的属性为空不能插入数据库
            //此处把其改为数据库默认的NULL
            foreach (OracleParameter OracleParameter in cmdParms)
            {
                if (OracleParameter.Value == null)
                {
                    OracleParameter.Value = DBNull.Value;
                }
            }
            using (OracleCommand cmd = new OracleCommand())
            {
                // cmd.CommandTimeout = cTimeOut;
                try
                {
                    PrepareCommand(cmd, mConn, null, SQLString, cmdParms);
                    if ((mTran != null) && (mTran.Connection != null))
                        cmd.Transaction = mTran;
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

        }


        /// <summary>
        /// 执行SQL返回结果集
        /// </summary>
        /// <param name="SQLStringList"></param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteSqlRe(string SQLString)
        {

            OracleCommand cmd = new OracleCommand();
            // cmd.CommandTimeout = cTimeOut;
            cmd.Connection = mConn;
            //OracleTransaction tx = mConn.BeginTransaction();
            //cmd.Transaction = tx;
            //string table = "table";
            OracleDataAdapter adapter = new OracleDataAdapter();
            DataSet set = new DataSet();

            if (mConn.State == ConnectionState.Closed)
            {
                mConn.Open();
            }

            cmd.CommandText = SQLString;

            adapter.SelectCommand = cmd;
            adapter.Fill(set);
            return set;

        }


        /// <summary>
        /// 执行多条SQL并返回DataSet
        /// </summary>
        /// <param name="SQLStringList"></param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteManySql(ArrayList SQLStringList)
        {

            OracleCommand cmd = new OracleCommand();
            // cmd.CommandTimeout = cTimeOut;
            cmd.Connection = mConn;
            OracleTransaction tx = mConn.BeginTransaction();
            cmd.Transaction = tx;
            string table = "table";
            OracleDataAdapter adapter = new OracleDataAdapter();
            DataSet set = new DataSet();
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();
                }
                for (int n = 0; n < SQLStringList.Count; n++)
                {
                    string strsql = SQLStringList[n].ToString();
                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        //cmd.ExecuteNonQuery();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(set, table + n);
                    }
                }
                tx.Commit();
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                tx.Rollback();
                throw E;
            }
            finally
            {
                cmd.Dispose();
            }
            return set;

        }

        /// <summary>
        /// 执行多条SQL并返回DataSet
        /// </summary>
        /// <param name="TableNameAndSQLStringList">HashTable的每一个元素，键名是表名，键值是SQL语句</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteManySql(Hashtable TableNameAndSQLStringList)
        {

            OracleCommand cmd = new OracleCommand();
            // cmd.CommandTimeout = cTimeOut;
            cmd.Connection = mConn;
            OracleTransaction tx = mConn.BeginTransaction();
            cmd.Transaction = tx;

            OracleDataAdapter adapter = new OracleDataAdapter();
            DataSet set = new DataSet();
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();
                }
                foreach (DictionaryEntry deSql in TableNameAndSQLStringList)
                {
                    string strTableName = deSql.Key.ToString();
                    string strsql = deSql.Value.ToString();

                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        adapter.SelectCommand = cmd;
                        adapter.Fill(set, strTableName);
                    }
                }
                tx.Commit();
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                tx.Rollback();
                throw E;
            }
            finally
            {
                cmd.Dispose();
            }
            return set;

        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param> 
        public void ExecuteSqlTran(ArrayList SQLStringList)
        {
            OracleCommand cmd = new OracleCommand();
            // cmd.CommandTimeout = cTimeOut;
            cmd.Connection = mConn;
            OracleTransaction tx = mConn.BeginTransaction();
            cmd.Transaction = tx;
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();
                }
                for (int n = 0; n < SQLStringList.Count; n++)
                {
                    string strsql = SQLStringList[n].ToString();
                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                tx.Rollback();
                throw E;
            }
            finally
            {
                cmd.Dispose();
            }

        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, string content)
        {

            OracleCommand cmd = new OracleCommand(SQLString, mConn);
            // cmd.CommandTimeout = cTimeOut;
            OracleParameter myParameter = new OracleParameter("@content", OracleType.VarChar);
            myParameter.Value = content;
            cmd.Parameters.Add(myParameter);
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();
                }
                if ((mTran != null) && (mTran.Connection != null))
                    cmd.Transaction = mTran;
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                throw E;
            }
            finally
            {
                cmd.Dispose();
            }

        }
        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {

            OracleCommand cmd = new OracleCommand(strSQL, mConn);
            // cmd.CommandTimeout = cTimeOut;
            OracleParameter myParameter = new OracleParameter("@fs", OracleType.Blob);
            myParameter.Value = fs;
            cmd.Parameters.Add(myParameter);
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();
                }
                if ((mTran != null) && (mTran.Connection != null))
                    cmd.Transaction = mTran;
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                throw E;
            }
            finally
            {
                cmd.Dispose();
            }

        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString)
        {
            using (OracleCommand cmd = new OracleCommand(SQLString, mConn))
            {
                cmd.CommandTimeout = cTimeOut;
                try
                {
                    if (mConn.State == ConnectionState.Closed)
                    {
                        mConn.Open();
                    }
                    if ((mTran != null) && (mTran.Connection != null))
                        cmd.Transaction = mTran;
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }

            }

        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <param name="fs">fs参数的buf</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, byte[] fs)
        {
            using (OracleCommand cmd = new OracleCommand(SQLString, mConn))
            {
                //cmd.CommandTimeout = cTimeOut;

                OracleParameter myParameter = new OracleParameter("@fs", OracleType.Blob);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);

                try
                {
                    if (mConn.State == ConnectionState.Closed)
                    {
                        mConn.Open();
                    }
                    if ((mTran != null) && (mTran.Connection != null))
                        cmd.Transaction = mTran;
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }

            }

        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <param name="transaction">回滚事务</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, OracleTransaction transaction)
        {
            using (OracleCommand cmd = new OracleCommand(SQLString, mConn, transaction))
            {
                // cmd.CommandTimeout = cTimeOut;
                try
                {
                    if (mConn.State == ConnectionState.Closed)
                    {
                        return null;
                    }
                    if ((mTran != null) && (mTran.Connection != null))
                        cmd.Transaction = mTran;
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }

            }

        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <param name="transaction">回滚事务</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, OracleTransaction transaction, byte[] fs)
        {
            using (OracleCommand cmd = new OracleCommand(SQLString, mConn, transaction))
            {
                // cmd.CommandTimeout = cTimeOut;
                OracleParameter myParameter = new OracleParameter("@fs", OracleType.Blob);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    if (mConn.State == ConnectionState.Closed)
                    {
                        return null;
                    }
                    if ((mTran != null) && (mTran.Connection != null))
                        cmd.Transaction = mTran;
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }

            }

        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet Query(string SQLString, params OracleParameter[] parameters)
        {
            DataSet ds = new DataSet();
            OracleCommand comm = null;
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();

                }
                OracleDataAdapter command = new OracleDataAdapter(SQLString, mConn);
                comm = command.SelectCommand;
                command.SelectCommand.CommandTimeout = cTimeOut;
                command.SelectCommand.Parameters.AddRange(parameters);
                command.Fill(ds, "ds");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (comm != null)
                {
                    comm.Parameters.Clear();
                }
                throw ex;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Parameters.Clear();
                }
            }

            return ds;
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString)
        {

            DataSet ds = new DataSet();
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();
                }

                OracleDataAdapter command = new OracleDataAdapter(SQLString, mConn);
                command.SelectCommand.CommandTimeout = cTimeOut;
                command.Fill(ds, "ds");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }

            return ds;

        }

        public DataTable QueryDataTable(string SQLString, params OracleParameter[] parameters)
        {
            DataTable dt;
            DataSet ds = new DataSet();
            OracleCommand comm = null;
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();

                }
                OracleDataAdapter command = new OracleDataAdapter(SQLString, mConn);
                comm = command.SelectCommand;
                command.SelectCommand.CommandTimeout = cTimeOut;
                command.SelectCommand.Parameters.AddRange(parameters);
                command.Fill(ds, "ds");
                dt = ds.Tables[0];
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (comm != null)
                {
                    comm.Parameters.Clear();
                }
                throw ex;
            }
            finally
            {
                if (comm != null)
                {
                    comm.Parameters.Clear();
                }
            }

            return dt;
        }

        /// <summary>
        /// 返回DataTable对象
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public DataTable QueryDataTable(string SQLString)
        {

            DataTable dt;
            DataSet ds = new DataSet();
            try
            {
                if (mConn.State == ConnectionState.Closed)
                {
                    mConn.Open();

                }
                OracleDataAdapter command = new OracleDataAdapter(SQLString, mConn);
                command.SelectCommand.CommandTimeout = cTimeOut;
                command.Fill(ds, "ds");
                dt = ds.Tables[0];
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }

            return dt;

        }

        #endregion

        /// <summary>
        /// 简单防sql注入程序
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Sqlstring(string str)
        {
            str = str.Replace("&", "&amp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt");
            str = str.Replace("'", "''");
            str = str.Replace("*", "");
            str = str.Replace("\n", "<br/>");
            str = str.Replace("\r\n", "<br/>");
            str = str.Replace("%", "''%''");
            str = str.Replace("_", "''_''");
            str = str.Replace("select", "");
            str = str.Replace("insert", "");
            str = str.Replace("update", "");
            str = str.Replace("delete", "");
            str = str.Replace("create", "");
            str = str.Replace("drop", "");
            str = str.Replace("delcare", "");
            str = str.Replace("--", "");
            if (str.Trim().ToString() == "")
            {
                str = "";
            }

            return str;
        }

        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        /*
        */
        /// <summary>
        /// 分页获取数据列表
        /// IsCount=0 返回记录总数,否则，返回记录集
        /// 分页存储过程UP_GetRecordByPage
        /// </summary>
        public DataSet GetList(string tableName, string colName, int pageCountSize, int PageIndex, string strWhere, bool orderTypeDESC)
        {
            //这里，为了不更改动软生成的代码太多部分，
            //将原来的第一个参数PageSize 作为一个标志使用
            //如果PageSize=0 ，即参数IsCount=0
            //则为计算总数
            //在传递给存储过程时，@IsCount=1
            //否则，就接收到了PageSize 的值


            //得到记录总数，如果保存统一的一个 PageSize，则可以从全局设置中直接取。
            int PageSize = pageCountSize;

            OracleParameter[] parameters = {
                new OracleParameter("@tblName",  OracleType.VarChar, 4000),
                new OracleParameter("@fldName", OracleType.VarChar, 1000),
                new OracleParameter("@orderName", OracleType.VarChar, 255),
                new OracleParameter("@PageSize", OracleType.Int32),
                new OracleParameter("@PageIndex", OracleType.Int32),
                new OracleParameter("@IsReCount", OracleType.Byte),
                new OracleParameter("@OrderType", OracleType.Byte),
                new OracleParameter("@strWhere", OracleType.VarChar,1000),
                };

            parameters[0].Value = tableName;
            parameters[1].Value = "";
            parameters[2].Value = colName;
            parameters[3].Value = PageSize;
            parameters[4].Value = PageIndex;
            parameters[5].Value = 1;     //@IsReCount （非）返回总数，返回记录集
            parameters[6].Value = orderTypeDESC;
            parameters[7].Value = strWhere;

            return RunProcedure("SP_GetPageList", parameters, "ds");
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            if (mConn == null || mConn.State == ConnectionState.Closed || mConn.State == ConnectionState.Broken)
            {
                ConnectDB();
            }

            DataSet dataSet = new DataSet();

            OracleDataAdapter sqlDA = new OracleDataAdapter();
            // sqlDA.SelectCommand 为null 值 条用CommandTimeout 报未将对象引用错误
            // sqlDA.SelectCommand.CommandTimeout = cTimeOut;
            sqlDA.SelectCommand = BuildQueryCommand(mConn, storedProcName, parameters);
            sqlDA.Fill(dataSet, tableName);
            return dataSet;

        }



        /// <summary>
        /// 构建 OracleCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private OracleCommand BuildQueryCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);
            //command.CommandTimeout = cTimeOut;
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // 检查未分配值的输出参数,将其分配以DBNull.Value.
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            return command;
        }

    }
}