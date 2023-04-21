using EL.Async;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EL.Sqlite
{
    public class SqliteComponent : Entity
    {
        public static SqliteComponent Instance;
        public string Directory = $"{System.IO.Directory.GetCurrentDirectory()}\\SQLite";
        public string DBName = $"WinXinMsg.db";

        public string FilePath => $"{Directory}\\{DBName}";
        public SqliteConnectionStringBuilder SqliteConnectionStringBuilder { get; set; }
        public SqliteConnection SqliteConnection { get; set; }
        public SqliteComponent()
        {
            Instance = this;
        }
    }
    public class SqliteComponentByDbNameAwake : AwakeSystem<SqliteComponent, string>
    {
        public override void Awake(SqliteComponent self, string dbName)
        {
            self.DBName = dbName;
            self.CreateConnection();
        }
    }
    public class SqliteComponentAwake : AwakeSystem<SqliteComponent>
    {
        public override void Awake(SqliteComponent self)
        {
            self.CreateConnection();
        }
    }
    public static class SqliteComponentSystem
    {
        /// <summary>
        /// 创建一个数据库文件。 这只是创建一个零字节的文件，
        /// SQLite正确打开文件后，它将变成数据库。 
        /// </summary>
        /// <param name="databaseFileName">需要创建的文件</param>
        public static bool IsExist(this SqliteComponent self,string tableName)
        {
            if (!File.Exists(self.FilePath)) return false;
            var list = self.Query<dynamic>($"select * from sqlite_master where name = '{tableName}';");
            return list != null && list.Count > 0;
        }
        public static void CreateDataFile(this SqliteComponent self, string dbName)
        {
            if (!string.IsNullOrWhiteSpace(dbName))
                self.DBName = dbName;

            if (File.Exists(self.FilePath))
                File.Copy(self.FilePath, self.FilePath + IdGenerater.Instance.GenerateInstanceId() + "_Temp");

            if (!Directory.Exists(self.Directory))
                Directory.CreateDirectory(self.Directory);

            using (var file = File.Open(self.FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                file.Close();
            }
            self.SqliteConnectionStringBuilder = new SqliteConnectionStringBuilder($"Data Source={self.FilePath};")
            {
                Mode = SqliteOpenMode.ReadWriteCreate
            };
        }

        internal static void CreateConnection(this SqliteComponent self)
        {
            if (self.SqliteConnectionStringBuilder == null)
                self.SqliteConnectionStringBuilder = new SqliteConnectionStringBuilder($"Data Source={self.FilePath};");
        }
        public static void CreateTable(this SqliteComponent self, string creatTableCmd)
        {
            self.CreateConnection();
 

            using (var connection = new SqliteConnection(self.SqliteConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var tran = connection.BeginTransaction();
                var command = connection.CreateCommand();
                command.Transaction = tran;
                command.CommandText = creatTableCmd;
                command.ExecuteNonQuery();
                tran.Commit();
            }

        }
        public static async ELTask<int> ExecuteNonQueryAsync(this SqliteComponent self, string cmd)
        {
            self.CreateConnection();
            int result = default;
            using (var connection = new SqliteConnection(self.SqliteConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = cmd;
                try
                {
                    result = await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

            }
            return result;
        }
        public static async ELTask<int> BulkExecuteNonQueryAsync(this SqliteComponent self, List<string> cmds)
        {
            self.CreateConnection();
            int result = default;
            using (var connection = new SqliteConnection(self.SqliteConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var tran = connection.BeginTransaction();
                var command = connection.CreateCommand();
                foreach (var item in cmds)
                {
                    command.CommandText = item;
                    try
                    {
                        result += await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
                tran.Commit();
            }
            return result;
        }
        public static void BulkExecute(this SqliteComponent self, Func<SqliteCommand, List<Task<int>>> action)
        {
            self.CreateConnection();
            using (var connection = new SqliteConnection(self.SqliteConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var tran = connection.BeginTransaction();
                var command = connection.CreateCommand();
                command.Transaction = tran;
                var result = action(command);
                tran.Commit();
            }
        }
        public static void BulkExecute(this SqliteComponent self, Func<SqliteCommand, List<Task<int>>> action, string connectionString)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tran = connection.BeginTransaction();
                var command = connection.CreateCommand();
                command.Transaction = tran;
                var result = action(command);
                tran.Commit();
            }
        }
        /// <summary>
        /// 返回一个实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static List<T> Query<T>(this SqliteComponent self, string cmd) where T : new()
        {
            self.CreateConnection();
            SqliteDataReader sqliteDataReader = null;
            using (var connection = new SqliteConnection(self.SqliteConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = cmd;
                try
                {
                    sqliteDataReader = command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return default;
                }
                return sqliteDataReader.ToList<T>();
            }
        }
        /// <summary>
        /// 返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static T Find<T>(this SqliteComponent self, string cmd) where T : new()
        {
            self.CreateConnection();
            SqliteDataReader sqliteDataReader = null;
            using (var connection = new SqliteConnection(self.SqliteConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = cmd;
                try
                {
                    sqliteDataReader = command.ExecuteReader();
                    sqliteDataReader.Read();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return default;
                }
                return sqliteDataReader.To<T>();
            }
        }
        /// <summary>
        /// 获取一个唯一值
        /// </summary>
        public static T Scalar<T>(this SqliteComponent self, string cmd)
        {
            self.CreateConnection();
            SqliteDataReader sqliteDataReader = null;
            using (var connection = new SqliteConnection(self.SqliteConnectionStringBuilder.ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = cmd;
                try
                {
                    var v = command.ExecuteScalar();
                    return (T)Convert.ChangeType(v, typeof(T));
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return default;
                }
            }
        }
    }
    public static class SqlDataReaderEx
    {
        /// <summary>
        /// 属性反射信息缓存 key:类型的hashCode,value属性信息
        /// </summary>
        private static Dictionary<int, Dictionary<string, PropertyInfo>> propInfoCache = new Dictionary<int, Dictionary<string, PropertyInfo>>();

        /// <summary>
        /// 将SqlDataReader转成T类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T To<T>(this SqliteDataReader reader)
          where T : new()
        {
            if (reader == null || reader.HasRows == false) return default(T);

            var res = new T();
            var propInfos = GetFieldnameFromCache<T>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var n = reader.GetName(i).ToLower();
                if (propInfos.ContainsKey(n))
                {
                    PropertyInfo prop = propInfos[n];
                    var IsValueType = prop.PropertyType.IsValueType;
                    object defaultValue = null;//引用类型或可空值类型的默认值
                    if (IsValueType)
                    {
                        if ((!prop.PropertyType.IsGenericType)
                            || (prop.PropertyType.IsGenericType && !prop.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
                        {
                            defaultValue = 0;//非空值类型的默认值
                        }
                    }
                    var val = Convert.ChangeType(reader.GetValue(i), prop.PropertyType);
                    prop.SetValue(res, (Convert.IsDBNull(val) ? defaultValue : val), null);
                }
            }

            return res;
        }

        private static Dictionary<string, PropertyInfo> GetFieldnameFromCache<T>()
        {
            Dictionary<string, PropertyInfo> res = null;
            var hashCode = typeof(T).GetHashCode();
            if (!propInfoCache.ContainsKey(hashCode))
            {
                propInfoCache.Add(hashCode, GetFieldname<T>());
            }
            res = propInfoCache[hashCode];
            return res;
        }

        /// <summary>
        /// 获取一个类型的对应数据表的字段信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Dictionary<string, PropertyInfo> GetFieldname<T>()
        {
            var res = new Dictionary<string, PropertyInfo>();
            var props = typeof(T).GetProperties();
            foreach (PropertyInfo item in props)
            {
                res.Add(item.GetFiledName(), item);
            }
            return res;
        }



        /// <summary>
        /// 将SqlDataReader转成List<T>类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this SqliteDataReader reader)
            where T : new()
        {
            if (reader == null || reader.HasRows == false) return null;
            var res = new List<T>();
            while (reader.Read())
            {
                res.Add(reader.To<T>());
            }
            return res;
        }

        /// <summary>
        /// 获取该属性对应到数据表中的字段名称
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static string GetFiledName(this PropertyInfo propInfo)
        {
            var fieldname = propInfo.Name;
            var attr = propInfo.GetCustomAttributes(false);
            foreach (var a in attr)
            {
                if (a is DataFieldAttribute)
                {
                    fieldname = (a as DataFieldAttribute).Name;
                    break;
                }
            }
            return fieldname.ToLower();
        }
    }

    public class DataFieldAttribute : Attribute
    {
        public DataFieldAttribute()
        {

        }
        public DataFieldAttribute(string name)
        {
            m_name = name;
        }
        private string m_name = null;

        public string Name { get { return m_name; } set { m_name = value; } }

    }
}
//#if NET47 || NET48  
//                DirectorySecurity directorySecurity = new DirectorySecurity();
//                directorySecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
//                Directory.CreateDirectory(self.Directory, directorySecurity);
//#endif
//#if NET6_0
//       Directory.CreateDirectory(self.Directory);
//#endif