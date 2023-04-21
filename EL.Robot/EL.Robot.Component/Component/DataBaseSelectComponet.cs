using EL.Async;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;

namespace EL.Robot.Component
{
    public class DataBaseSelectComponet :BaseComponent
    {

        [Obsolete]
        public override async ELTask<INodeContent> Main(INodeContent self)
        {
            await ELTask.CompletedTask;
            var con = self.CurrentNode.GetParamterValue("Content");
            var queryString = self.CurrentNode.GetParamterString("queryString").ToLower();
            DbDataAdapter dbDataAdapter = default;
            if (con is MySqlConnection mySqlConnection)
                dbDataAdapter = new MySqlDataAdapter(queryString, mySqlConnection);
            if (con is SqlConnection sqlConnection)
                dbDataAdapter = new SqlDataAdapter(queryString, sqlConnection);
            if (con is OracleConnection oracleConnection)
                dbDataAdapter = new OracleDataAdapter(queryString, oracleConnection);
            DataSet dataSet = new();
            dbDataAdapter.Fill(dataSet);
            self.Out = dataSet.Tables[0];
            self.Value = true;
            return self;
        }
    }
}
