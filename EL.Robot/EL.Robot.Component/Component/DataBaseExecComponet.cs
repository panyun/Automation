using EL.Async;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;

namespace EL.Robot.Component
{
    public class DataBaseExecComponet : BaseComponent
    {
		public DataBaseExecComponet()
		{
			Config.Category = Category.基础函数;
		}
		public override Config GetConfig()
		{
			if (Config.IsInit) return Config;
			Config.DisplayName = "数据库执行";
			return base.GetConfig();
		}
		[Obsolete]
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await base.Main(self);
            var con = self.CurrentNode.GetParamterValue("Content");
            var queryString = self.CurrentNode.GetParamterString("queryString").ToLower();
            if (con is MySqlConnection mySqlConnection)
            {
                try
                {
                    mySqlConnection.Open();
                    var cmd = mySqlConnection.CreateCommand();
                    cmd.CommandText = queryString;
                    self.Out = cmd.ExecuteNonQuery();
                    mySqlConnection.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    mySqlConnection.Close();
                }
            }
            if (con is SqlConnection sqlConnection)
            {
                try
                {
                    sqlConnection.Open();
                    var cmd = sqlConnection.CreateCommand();
                    cmd.CommandText = queryString;
                    self.Out = cmd.ExecuteNonQuery();
                    sqlConnection.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            
            }
            if (con is OracleConnection oracleConnection)
            {
                try
                {
                    oracleConnection.Open();
                    var cmd = oracleConnection.CreateCommand();
                    cmd.CommandText = queryString;
                    self.Out = cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    oracleConnection.Close();
                }
            }
            self.Value = true;
            return self;
        }
    }
}
