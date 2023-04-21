using EL.CollectWebPages.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Common
{
    //public class SqliteHelper
    //{
    //    private string dbName;
    //    public SqliteHelper(string dbName = "test.db")
    //    {
    //        this.dbName = dbName;
    //    }
    //    public int ExecuteNonQuery(string sql, List<System.Data.SQLite.SQLiteParameter> parameters = null)
    //    {
    //        var line = 0;
    //        try
    //        {
    //            using SQLiteConnection cn = new SQLiteConnection("data source=" + this.dbName);
    //            cn.Open();
    //            if (cn.State ==  System.Data.ConnectionState.Open)
    //            {
    //                using SQLiteCommand cmd = new SQLiteCommand();
    //                cmd.Connection = cn;
    //                cmd.CommandText = sql;
    //                if (parameters?.Any() == true)
    //                {
    //                    cmd.Parameters.Clear();
    //                    cmd.Parameters.AddRange(parameters.ToArray());
    //                }
    //                line = cmd.ExecuteNonQuery();
    //            }
    //            cn.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //        }
    //        return line;
    //    }
    //    public SQLiteDataReader GetExecuteReader(string sql, List<System.Data.SQLite.SQLiteParameter> parameters = null)
    //    {
    //        SQLiteDataReader sQLiteDataReader = null;
    //        try
    //        {
    //            using SQLiteConnection cn = new SQLiteConnection("data source=" + this.dbName);
    //            cn.Open();
    //            if (cn.State == System.Data.ConnectionState.Open)
    //            {
    //                using SQLiteCommand cmd = new SQLiteCommand();
    //                cmd.Connection = cn;
    //                cmd.CommandText = sql;
    //                if (parameters?.Any() == true)
    //                {
    //                    cmd.Parameters.Clear();
    //                    cmd.Parameters.AddRange(parameters.ToArray());
    //                }
    //                sQLiteDataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
    //            }
    //        }
    //        catch (Exception)
    //        {
    //        }
    //        return sQLiteDataReader;
    //    }
    //    public static SqliteHelper GetSqlite(string dbName = "Test.db")
    //    {
    //        return new SqliteHelper(dbName);
    //    }
    //}
}
