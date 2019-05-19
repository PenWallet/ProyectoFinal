using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace DAL.Conexion
{
    public class Connection
    {
        public String server { get; set; }
        public String dataBase { get; set; }
        public String user { get; set; }
        public String pass { get; set; }
        
        public Connection()
        {
            this.server = "localhost";
            this.dataBase = "CHAIRDB";
            this.user = "CHAIRMaster";
            this.pass = "chairisbetterthansteam";
         }
        
        public Connection(String server, String database, String user, String pass)
        {
            this.server = server;
            this.dataBase = database;
            this.user = user;
            this.pass = pass;
        }


        //METODOS

        /// <summary>
        /// Method to open a connection to the database
        /// </summary>
        /// <returns>An SqlConnection object connected to the database</returns>
        public SqlConnection getConnection()
        {
            SqlConnection connection = new SqlConnection();

            try
            {
                //connection.ConnectionString = string.Format("server={0};database={1};uid={2};pwd={3};", server, dataBase, user, pass);
                connection.ConnectionString = $"server={server};database={dataBase};uid={user};pwd={pass};MultipleActiveResultSets=true"; //Max Pool Size=300;MultipleActiveResultSets=true
                connection.Open();
            }
            catch (SqlException)
            {
                throw;
            }

            return connection;

        }

        /// <summary>
        /// Close a connection with the database
        /// </summary>
        /// <post>The connection is closed.</post>
        /// <param name="connection">Connection to be closed
        /// </param>
        public void closeConnection(ref SqlConnection connection)
        {
            try
            {
                connection.Close();
                connection.Dispose();
            }
            catch (SqlException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }

}
