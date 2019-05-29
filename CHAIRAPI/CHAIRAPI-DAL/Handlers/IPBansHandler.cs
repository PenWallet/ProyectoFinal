using CHAIRAPI_Entities.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class IPBansHandler
    {
        /// <summary>
        /// Method which will save the IPBan in the database
        /// </summary>
        /// <param name="ipBan">The IPBan to be saved</param>
        /// <returns>1 if saved successfully; 0 if the IP is already banned; -1 other errors</returns>
        public static int saveNewIPBan(IPBan ipBan)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "INSERT INTO IPBans(IP, banReason, untilDate) VALUES (@IP, @banReason, @untilDate)";

                //Create parameters
                command.Parameters.Add("@IP", SqlDbType.VarChar).Value = ipBan.IP;
                command.Parameters.Add("@banReason", SqlDbType.VarChar).Value = ipBan.banReason;
                command.Parameters.Add("@untilDate", SqlDbType.DateTime).Value = ipBan.untilDate;

                //Get connection
                sqlConnection = connection.getConnection();

                //Give the connection to the command
                command.Connection = sqlConnection;

                //Execute query
                affectedRows = command.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {

                if (ex.Number == 2627) //Duplicate PRIMARY KEY Exception Number
                    affectedRows = 0;
                else
                    affectedRows = -1; //Instead of throwing exception, change affectedRows to -1
            }
            catch (Exception)
            {
                affectedRows = -1; //Instead of throwing exception, change affectedRows to -1
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return affectedRows;
        }

        /// <summary>
        /// Method which will save the changes made to the user in the database
        /// </summary>
        /// <param name="user">The user to be updated</param>
        /// <returns>1 if updated successfully; 0 if no IP was found; -1 otherwise</returns>
        public static int updateIPBan(IPBan ipBan)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE IPBans SET banReason = @banReason, untilDate = @untilDate WHERE IP = @IP";

                //Create parameters
                command.Parameters.Add("@IP", SqlDbType.VarChar).Value = ipBan.IP;
                command.Parameters.Add("@banReason", SqlDbType.VarChar).Value = ipBan.banReason;
                command.Parameters.Add("@untilDate", SqlDbType.DateTime).Value = ipBan.untilDate;

                //Get connection
                sqlConnection = connection.getConnection();

                //Give the connection to the command
                command.Connection = sqlConnection;

                //Execute query
                affectedRows = command.ExecuteNonQuery();

            }
            catch (SqlException)
            {
                affectedRows = -1; //Instead of throwing exception, change affectedRows to -1
            }
            catch (Exception)
            {
                affectedRows = -1; //Instead of throwing exception, change affectedRows to -1
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return affectedRows;
        }

        /// <summary>
        /// Method which will search the database for the IPBan with the specified IP
        /// </summary>
        /// <param name="IP">The IP of the IPBan to be searched</param>
        /// <returns>The IPBan with all its information if it was found, false otherwise</returns>
        public static IPBan searchIPBanByIP(string IP)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            IPBan ipBan = null;


            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT IP, banReason, untilDate FROM IPBans WHERE IP = @IP";

                //Set the parameter
                command.Parameters.Add("@IP", SqlDbType.VarChar).Value = IP;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Read the result and assign values
                    reader.Read();
                    ipBan = new IPBan();
                    ipBan.IP = (string)reader["IP"];
                    ipBan.banReason = (string)reader["banReason"];
                    ipBan.untilDate = (DateTime)reader["untilDate"];
                }

            }
            catch (SqlException) { ipBan = null; }
            catch (Exception) { ipBan = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return ipBan;
        }
    }
}
