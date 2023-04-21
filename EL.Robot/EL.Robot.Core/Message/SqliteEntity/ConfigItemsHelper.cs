using EL.Sqlite;

namespace EL.Robot.Core.SqliteEntity
{
    /// <summary>
    /// 数据库帮助类
    /// </summary>
    public static class ConfigItemsHelper
    {
        public static bool Log_IsAutomaticBackup
        {
            get
            {
                return Get<bool>(nameof(Log_IsAutomaticBackup));
            }
            set
            {
                Set<bool>(nameof(Log_IsAutomaticBackup), value);
            }
        }
        public static int Log_BackupDays
        {
            get
            {
                return Get<int>(nameof(Log_BackupDays));
            }
            set
            {
                Set(nameof(Log_BackupDays), value);
            }
        }
        public static string Log_BackupDirectory
        {
            get
            {
                return Get(nameof(Log_BackupDirectory));
            }
            set
            {
                Set(nameof(Log_BackupDirectory), value);
            }
        }
        public static bool Log_IsScreenRecording
        {
            get
            {
                return Get<bool>(nameof(Log_IsScreenRecording));
            }
            set
            {
                Set<bool>(nameof(Log_IsScreenRecording), value);
            }
        }
        public static bool IsExist
        {
            get
            {
                var sql = Boot.GetComponent<RobotComponent>().GetComponent<SqliteComponent>();
                if (!File.Exists(sql.FilePath)) return false;
                var list = sqliteComponent.Query<dynamic>($"select * from sqlite_master where name = '{nameof(ConfigItems)}';");
                return list != null && list.Count > 0;
            }
        }
        static readonly SqliteComponent sqliteComponent = Boot.GetComponent<RobotComponent>().GetComponent<SqliteComponent>();
        public static string Get(string key)
        {
            var ConfigItems = sqliteComponent.Find<ConfigItems>($"select id,key,value from configitems where key ='{key}';");
            return ConfigItems?.Value?.ToString();
        }
        public static T Get<T>(string key)
        {
            var ConfigItems = sqliteComponent.Find<ConfigItems>($"select id,key,value from configitems where key ='{key}';");
            if (ConfigItems != null)
            {
                return (T)Convert.ChangeType(ConfigItems.Value, typeof(T));
            }
            return default;
        }
        public static bool Set(string key, string data)
        {
            return sqliteComponent.ExecuteNonQueryAsync($"update configitems set value='{data}' WHERE key='{key}';").GetResult() > 0;
        }
        public static bool Set<T>(string key, T data)
        {
            return sqliteComponent.ExecuteNonQueryAsync($"update configitems set value='{Convert.ToString(data)}' WHERE key='{key}';").GetResult() > 0;
        }
    }

}
