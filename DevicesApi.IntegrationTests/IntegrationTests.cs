using DevicesApi.Contracts.Requests;
using DevicesApi.Domain;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using System.Net.Http;

namespace DevicesApi.IntegrationTests
{
    public class IntegrationTests
    {
        protected readonly string ConnectionString;
        protected readonly SqlConnection Connection;
        protected readonly SqlDataAdapter DataAdapter;
        protected readonly HttpClient HttpClient;

        protected IntegrationTests()
        {
            HttpClient = new HttpClient();
            ConnectionString = "Server=db;Database=DevicesApi;User=sa;Password=Pedro*12345";
            Connection = new SqlConnection(ConnectionString);
            DataAdapter = new SqlDataAdapter();
        }

        protected bool ExecuteNonQuery(string commandString)
        {
            int updatedRows = 0;
            SqlCommand command = new SqlCommand(commandString, Connection);
            try
            {
                Connection.Open();
                DataAdapter.InsertCommand = command;
                updatedRows = DataAdapter.InsertCommand.ExecuteNonQuery();
            }
            catch (System.Exception)
            {
                return false;
            }
            finally
            {
                Connection.Close();
            }

            return updatedRows > 0 ;
        }

        protected Device ExecuteDevicesQuery(string commandString)
        {
            SqlCommand command = new SqlCommand(commandString, Connection);
            try
            {
                Connection.Open();
                DataAdapter.InsertCommand = command;
                SqlDataReader sqlDataReader = DataAdapter.InsertCommand.ExecuteReader();
                if(sqlDataReader.Read())
                {
                    return new Device
                    {
                        Device_id = (int)sqlDataReader["Device_id"],
                        Name = (string)sqlDataReader["Name"],
                        Location = (string)sqlDataReader["Location"]
                    };
                }

                return null;
            }
            catch (System.Exception)
            {
                return null;
            }
            finally
            {
                Connection.Close();
            }
        }
    }
}
