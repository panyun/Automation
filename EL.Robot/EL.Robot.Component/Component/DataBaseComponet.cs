using EL.Async;
using MySql.Data.MySqlClient;
using System.Data.OracleClient;
using System.Data.SqlClient;

namespace EL.Robot.Component
{
    public class DataBaseComponet: BaseComponent
    {
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var ip = self.CurrentNode.GetParamterString("ip").ToLower();
            var type = self.CurrentNode.GetParamterString("type").ToLower();
            var port = self.CurrentNode.GetParamterString("port").ToLower();
            var dbName = self.CurrentNode.GetParamterString("dbName");
            var user = self.CurrentNode.GetParamterString("user");
            var passworld = self.CurrentNode.GetParamterString("passworld");
            if (type.ToLower() == "mysql")
            {
                var mysql = new MySqlConnection($@"server=""{ip}:{port}"";user={user};password={passworld};database={dbName}");
                mysql.Open();
                mysql.Close();
                self.Out = mysql;
                self.Value = true;
            }
            if (type.ToLower() == "sqlserver")
            {
                var sql = new SqlConnection($@"Data Source={ip}:{port};Initial Catalog={dbName};User ID={user};Password={passworld}");
                sql.Open();
                sql.Close();
                self.Out = sql;
            }
            if (type.ToLower() == "oracle")
            {
                string connectionString = string.Concat(
                @"Data Source=",
                @"    (DESCRIPTION=",
                @"        (ADDRESS_LIST=",
                @"            (ADDRESS=",
                @"                (PROTOCOL=TCP)",
               $@"                (HOST={ip})",
                $@"                (PORT={port})",
                @"            )",
                @"        )",
                @"        (CONNECT_DATA=",
                $@"            (SERVICE_NAME={dbName})",
                @"        )",
                @"    );",
                @"Persist Security Info=True;",
                $@"User Id={user};",
                $@"Password={passworld}"
                );
                // Create a new OracleConnection object with the connection string
                OracleConnection connection = new OracleConnection(connectionString);
                connection.Open();
                connection.Close();
                self.Out = connection;
            }
            self.Value = true;
            return self;
        }
    }
}
