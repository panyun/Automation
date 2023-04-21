using Dapper;
using EL.Async;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace EL.Sqlite
{
    public class ElSqliteComponent : Entity
    {
        public string FilePath => $"SQLite\\SQLite.db";
        public SqliteConnectionStringBuilder sb;
        public ElSqliteComponent()
        {
            sb = new SqliteConnectionStringBuilder();
            sb.DataSource = FilePath;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        public int ExcuteWithEtity(string sql, object entity)
        {
            using (var conn = new SqliteConnection(sb.ToString()))
            {
                return conn.Execute(sql, entity);
            }
        }

        /// <summary>
        /// 执行sql语句(包含删除)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Excute(string sql)
        {
            using (var conn = new SqliteConnection(sb.ToString()))
            {
                return conn.Execute(sql);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> GetEntitys<T>(string sql)
        {
            Debug.WriteLine(sql);
            using (var conn = new SqliteConnection(sb.ToString()))
            {
                return conn.Query<T>(sql).ToList();
            }
        }

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T GetEntity<T>(string sql)
        {
            using (var conn = new SqliteConnection(sb.ToString()))
            {
                return conn.QueryFirstOrDefault<T>(sql);
            }
        }

        /// <summary>
        /// 判断指定数据是否存在
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int GetEntityNumber(string sql)
        {
            using (var conn = new SqliteConnection(sb.ToString()))
            {
                return conn.QueryFirstOrDefault<int>(sql);
            }
        }
    }
    public class ElSqliteComponentAwake : AwakeSystem<ElSqliteComponent>
    {
        public override void Awake(ElSqliteComponent self)
        {
        }
    }
}