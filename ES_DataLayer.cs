using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SQLite;
using System.Collections;
using ES_FORMS;
using MySql.Data.MySqlClient;

namespace ES_DataLayer
{
    public class Pub
    {
        /// <summary>
        /// MID窗口指針
        /// </summary>
        public static Form midform = null;

        private static string _appPath = null;
        /// <summary>
        /// 應用程式目錄
        /// </summary>
        public static string AppPath
        {
            get
            {
                if (_appPath == null)
                {
                    _appPath = System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
                }
                return _appPath;
            }
        }
    }
    
    /// <summary>
    /// 資料操作介面
    /// </summary>
    interface iESData
    {
        void Open_Conn();
        void Close_Conn();
        DbDataReader Reader(string sql);
        DbDataReader Reader_ShowTables();
        int Exec(String sql);
        String ViewTableTxt(String TableName);
        DbCommand GetCommand(String cmdsql);
        DbCommandBuilder GetDbCommandBuilder(DbDataAdapter da);
        SQLiteCommand GetSQLiteCommand(String cmdsql, SQLiteParameter[] paras);
        OdbcCommand GetOdbcCommand(String cmdsql, OdbcParameter[] paras);
        MySqlCommand GetMyCommand(string cmdsql, MySqlParameter[] paras);
        String ShowTablesSQL();
    }
    /// <summary>
    /// 資料操作
    /// </summary>
    public class ESData : iESData
    {
        private static ESData _instance = null;
        

        /// <summary>
        ///中英欄位對照 
        /// </summary>
        public Hashtable dict = null;
        /// <summary>
        /// 構造
        /// </summary>
        protected ESData()
        {
            dict = new Hashtable();
            dict.Add("itemno", "物品編號");
            dict.Add("item", "產品");
            dict.Add("model", "型號");
        }
        /// <summary>
        /// 可選資料類別
        /// </summary>
        /// <param name="t">1 sqlite,2 mysql.net,3 odbc_mysql</param>
        public static void SetDB(int t)
        {
            switch (t)
            {
                case 1:
                    _instance = new SQLite_ESData();  //本地db
                    break;
                case 2:
                    _instance = new MySQL_ESData(); //250server
                    _instance.Open_Conn();

                    break;
                case 3:
                    _instance = new MySQL_ODBC_ESData();
                    break;
            }
            
        }
        public static void SetDB(int t, String dbname)
        {
            switch (t)
            {
                case 1:
                    _instance = new SQLite_ESData(dbname);  //本地db
                    break;
                case 2:
                    _instance = new MySQL_ESData(dbname); //250server
                    _instance.Open_Conn();

                    break;
                case 3:
                    _instance = new MySQL_ODBC_ESData(dbname);
                    break;
            }
        }

        /// <summary>
        /// 唯一實列化
        /// </summary>
        public static ESData GetInst
        {
            get
            {
                if (_instance == null)
                {
                    //_instance = new SQLite_ESData();  //本地db
                    // _instance = new MySQL_ODBC_ESData();
                    _instance = new MySQL_ESData(); //250server
                }
                return _instance;
            }
        }
        protected static System.Data.Common.DbConnection _conn = null;
        protected virtual String GetConn_Txt() { return null; }
        public String Conn_Txt { get { return GetConn_Txt(); } }
        protected virtual DbConnection GetConn()
        {
            return null;
        }
        public DbConnection conn
        {
            get
            {
                return GetConn();
            }
        }
        public void Open_Conn()
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
        }
        public void Close_Conn()
        {
            conn.Close();
            conn.Dispose();
        }
        public virtual System.Data.Common.DbDataReader Reader(string sql)
        {
            return null;
        }
        public virtual System.Data.Common.DbDataReader Reader_ShowTables()
        {
            return null;
        }
        public virtual int Exec(String sql)
        {
            return 0;
        }
        public virtual DbCommand GetCommand(String cmdsql)
        {
            return null;
        }
        public virtual DbCommandBuilder GetDbCommandBuilder(DbDataAdapter da)
        {
            return null;
        }
        public virtual DbDataAdapter GetAdapter()
        {
            return null;
        }
        public virtual DbDataAdapter GetAdapter(DbCommand cmd)
        {
            return null;
        }
        public virtual OdbcCommand GetOdbcCommand(String cmdsqsl, OdbcParameter[] paras)
        {
            return null;
        }
        public virtual SQLiteCommand GetSQLiteCommand(String cmdsqsl, SQLiteParameter[] paras)
        {
            return null;
        }
        public virtual MySqlCommand GetMyCommand(string cmdsql, MySqlParameter[] paras)
        {
            return null;
        }
        public String ViewTableTxt(String TableName)
        {
            ESData.GetInst.Open_Conn();
            String txt = "TabeName:" + TableName + "\n";
            System.Data.Common.DbDataReader dr = ESData.GetInst.Reader(String.Format("select * from {0};", TableName));

            for (int i = 0; i < dr.FieldCount; i++)
            {
                txt += dr.GetName(i) + "\t ";
            }
            txt += "\n";
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        try
                        {
                            txt += dr[i].ToString() + "\t ";
                        }
                        catch
                        {
                            MessageBox.Show(dr[0].ToString());
                        }
                    }
                    txt += "\n";
                }
            }
            dr.Close();
            dr.Dispose();
            return txt;
        }
        public virtual String ShowTablesSQL()
        {
            return null;
        }
        public static string db_host = "127.0.0.1";
        public static string db_user = "u";
        public static string db_pwd = "p";
        public static string db_dbname = "db";

    }
    class MySQL_ESData : ESData
    {
        public MySQL_ESData()
            : base()
        {

        }
        public MySQL_ESData(String host,String dbname,String usr,String pwd)
            : base()
        {
            _conn_txt = String.Format(my_con_str, host, usr, pwd, dbname);
        }
        public MySQL_ESData( String dbname)
            : base()
        {
            _conn_txt = String.Format(my_con_str, db_host, db_user, db_pwd, dbname);
        }
        private static readonly string my_con_str = "server={0};uid={1}; pwd={2}; database={3};pooling=true;Connect Timeout=200;CharSet=utf8;Allow Zero Datetime=true";
        private static string _conn_txt = String.Format(my_con_str,   db_host, db_user,db_pwd,db_dbname);
        private static MySqlConnection myconn = null;
        protected override string GetConn_Txt()
        {
            return _conn_txt;
        }
        protected override DbConnection GetConn()
        {
            if (_conn == null)
            {
                myconn = new MySqlConnection(_conn_txt);
                _conn = myconn;
            }
            return _conn;
        }
        public override DbDataReader Reader(string sql)
        {
            Open_Conn();
            MySqlCommand cmd = new MySqlCommand(sql, myconn);
            return cmd.ExecuteReader();
        }
        public override DbDataReader Reader_ShowTables()
        {
            Open_Conn();
            MySqlCommand cmd = new MySqlCommand("show tables;", myconn);
            return cmd.ExecuteReader();
        }
        public override string ShowTablesSQL()
        {
            //return base.ShowTablesSQL();
            return "show tables";
        }

        public override int Exec(string sql)
        {
            MySqlCommand cmd = new MySqlCommand(sql, myconn);
            int reti = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return reti;
        }
        public override DbCommand GetCommand(string cmdsql)
        {
            return new MySqlCommand(cmdsql, myconn);
        }
        public override DbCommandBuilder GetDbCommandBuilder(DbDataAdapter da)
        {
            if (da is MySqlDataAdapter)
            {
                return new MySqlCommandBuilder((MySqlDataAdapter)da);
            }
            else
            {
                return base.GetDbCommandBuilder(da);
            }
        }
        public override DbDataAdapter GetAdapter()
        {
            return new MySqlDataAdapter();
        }
        public override DbDataAdapter GetAdapter(DbCommand cmd)
        {
            if (cmd is MySqlCommand)
            {
                return new MySqlDataAdapter((MySqlCommand)cmd);
            }
            else { return base.GetAdapter(cmd); }
        }
        public override MySqlCommand GetMyCommand(string cmdsql, MySqlParameter[] paras)
        {
            MySqlCommand cmd = new MySqlCommand(cmdsql, myconn);
            if (paras != null)
                for (int i = 0; i < paras.Length; i++)
                {
                    cmd.Parameters.Add(paras[i]);
                }
            return cmd;
        }
    }
    class MySQL_ODBC_ESData : ESData
    {

         public MySQL_ODBC_ESData()
            : base()
        {

        }
        public MySQL_ODBC_ESData(String host,String dbname,String usr,String pwd)
            : base()
        {
            _conn_txt = String.Format(cnst5_1DriverConnStrFormat, host,dbname, usr, pwd);
        }
        public MySQL_ODBC_ESData(String dbname)
            : base()
        {
            _conn_txt = String.Format(cnst5_1DriverConnStrFormat, db_host, dbname, db_user, db_pwd);
        }
        private static readonly string cnst5_1DriverConnStrFormat = "Driver={{MySQL ODBC 5.1 Driver}};Server={0};Database={1};UID={2};PWD={3};OPTION=67108867";
        private static String DBNAME = db_dbname;
        private static String _conn_txt = string.Format(cnst5_1DriverConnStrFormat, db_host, db_dbname, db_user, db_pwd);
        private static OdbcConnection mysqlconn = null;
        protected override string GetConn_Txt()
        {
            return _conn_txt;
        }
        protected override DbConnection GetConn()
        {
            if (_conn == null)
            {
                mysqlconn = new OdbcConnection(_conn_txt);
                _conn = mysqlconn;
            }
            return _conn;
        }
        public override DbDataReader Reader(string sql)
        {
            Open_Conn();
            OdbcCommand cmd = new OdbcCommand(sql, mysqlconn);
            return cmd.ExecuteReader();
        }
        public override DbDataReader Reader_ShowTables()
        {
            Open_Conn();
            OdbcCommand cmd = new OdbcCommand("show tables", mysqlconn);
            return cmd.ExecuteReader();
        }
        public override int Exec(string sql)
        {
            return (new OdbcCommand(sql, mysqlconn)).ExecuteNonQuery();
        }
        public override DbCommandBuilder GetDbCommandBuilder(DbDataAdapter da)
        {
            if (da is OdbcDataAdapter)
            {
                return new OdbcCommandBuilder((OdbcDataAdapter)da);
            }
            else
            {
                return base.GetDbCommandBuilder(da);
            }
        }
        public override DbCommand GetCommand(string cmdsql)
        {
            return new OdbcCommand(cmdsql, mysqlconn);
        }
        public override DbDataAdapter GetAdapter()
        {
            return new OdbcDataAdapter();
        }
        public override DbDataAdapter GetAdapter(DbCommand cmd)
        {
            if (cmd is OdbcCommand)
            {
                return new OdbcDataAdapter((OdbcCommand)cmd);
            }
            else
                return base.GetAdapter(cmd);
        }
        public override OdbcCommand GetOdbcCommand(string cmdsql, OdbcParameter[] paras)
        {
            OdbcCommand cmd = new OdbcCommand(cmdsql, mysqlconn);
            if (paras != null)
                for (int i = 0; i < paras.Length; i++)
                {
                    cmd.Parameters.Add(paras[i]);
                }
            return cmd;
        }
        public override string ShowTablesSQL()
        {
            //return base.ShowTablesSQL();
            return "show tables";
        }
    }
    public class SQLite_ESData : ESData
    {
        public SQLite_ESData()
            : base()
        {
        }
        public SQLite_ESData(String dbname)
            : base()
        {
            DBNAME = dbname + ".sqlite";
            _conn_txt = string.Format("Data Source=\"{0}\\{1}\"", Pub.AppPath, DBNAME);
        }
        
        private static String DBNAME = "PPYM.sqlite";
        private static String _conn_txt = string.Format("Data Source=\"{0}\\{1}\"", Pub.AppPath, DBNAME);
        private static SQLiteConnection sqliteconn = null;
        protected override string GetConn_Txt()
        {
            return _conn_txt;
        }
        protected override DbConnection GetConn()
        {
            if (_conn == null)
            {
                sqliteconn = new SQLiteConnection(_conn_txt);
                _conn = sqliteconn;
            }
            return _conn;
        }
        public override DbDataReader Reader(string sql)
        {
            Open_Conn();
            SQLiteCommand cmd = new SQLiteCommand(sql, sqliteconn);
            return cmd.ExecuteReader();
        }
        public override DbDataReader Reader_ShowTables()
        {
            Open_Conn();
            SQLiteCommand cmd = new SQLiteCommand("select tbl_name from sqlite_master where type='table' order by tbl_name;", sqliteconn);
            return cmd.ExecuteReader();
        }
        public override int Exec(string sql)
        {
            return (new SQLiteCommand(sql, sqliteconn)).ExecuteNonQuery();
        }
        public override DbCommand GetCommand(string cmdsql)
        {
            return new SQLiteCommand(cmdsql, sqliteconn);
        }
        public override DbCommandBuilder GetDbCommandBuilder(DbDataAdapter da)
        {
            if (da is SQLiteDataAdapter)
            {
                return new SQLiteCommandBuilder((SQLiteDataAdapter)da);
            }
            else
            {
                return base.GetDbCommandBuilder(da);
            }
        }
        public override DbDataAdapter GetAdapter()
        {
            return new SQLiteDataAdapter();
        }
        public override DbDataAdapter GetAdapter(DbCommand cmd)
        {
            if (cmd is SQLiteCommand)
            {
                return new SQLiteDataAdapter((SQLiteCommand)cmd);
            }
            else { return base.GetAdapter(cmd); }
        }

        public override SQLiteCommand GetSQLiteCommand(string cmdsql, SQLiteParameter[] paras)
        {
            SQLiteCommand cmd = new SQLiteCommand(cmdsql, sqliteconn);
            if (paras != null)
                for (int i = 0; i < paras.Length; i++)
                {
                    cmd.Parameters.Add(paras[i]);
                }
            return cmd;
        }
        public override string ShowTablesSQL()
        {
            //return base.ShowTablesSQL();
            return "select tbl_name from sqlite_master where type='table' order by tbl_name;";
        }
    }
    public class esreport
    {
        public static void ShowTable(String sql)
        {
            FormDataGrid_Cmd fg = new FormDataGrid_Cmd(new ES_FORMS.iSqlCmdFormGrid(ESData.GetInst.GetAdapter(),
                ESData.GetInst.GetCommand(sql),
             null,
             null,
             null),
             ESData.GetInst.dict,
             ES_FORMS.BindingListOptions.AllPri);
            fg.MdiParent = Pub.midform;
            fg.Text = "(ReadOnly)"+sql;
            fg.Show();
        }
        public static void ShowTableWithUpdate(String sql)
        {
            DbDataAdapter da =ESData.GetInst.GetAdapter();
            da.SelectCommand = ESData.GetInst.GetCommand(sql);
            ESData.GetInst.GetDbCommandBuilder(da);
            FormDataGrid_Cmd fg = new FormDataGrid_Cmd(new ES_FORMS.iSqlCmdFormGrid(da),
             ESData.GetInst.dict,
             ES_FORMS.BindingListOptions.AllPri);
            fg.MdiParent = Pub.midform;
            fg.Text = "(+RW)"+sql;
            fg.Show();
        }
        public static void ShowTableWithImportxls(String sql)
        {
            DbDataAdapter da = ESData.GetInst.GetAdapter();
            da.SelectCommand = ESData.GetInst.GetCommand(sql);
            ESData.GetInst.GetDbCommandBuilder(da);
            FormDataGrid_Cmd_Importxls fg = new FormDataGrid_Cmd_Importxls(new ES_FORMS.iSqlCmdFormGrid(da),
             ESData.GetInst.dict,
             ES_FORMS.BindingListOptions.AllPri);
            fg.MdiParent = Pub.midform;
            fg.Text = "(+RW)" + sql;
            fg.Show();
        }
        public static void ShowTableWithUpdateAndInsert(String SQL,String insSQL)
        {
            DbDataAdapter da = ESData.GetInst.GetAdapter();
            da.SelectCommand = ESData.GetInst.GetCommand(SQL);
            ESData.GetInst.GetDbCommandBuilder(da);
        
            da.InsertCommand = ESData.GetInst.GetCommand(insSQL);
            FormDataGrid_Cmd fg = new FormDataGrid_Cmd(new ES_FORMS.iSqlCmdFormGrid(da),
             ESData.GetInst.dict,
             ES_FORMS.BindingListOptions.AllPri);
            fg.MdiParent = Pub.midform;
            fg.Text = "(+RW)" + SQL;
            fg.Show();
        }
    }
}
