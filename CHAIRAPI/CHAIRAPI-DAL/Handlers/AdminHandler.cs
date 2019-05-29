using CHAIRAPI_Entities.Complex;
using CHAIRAPI_Entities.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class AdminHandler
    {
        /// <summary>
        /// Method which will search the database for all relationships and return all of the games which users have in their libraries and are being played
        /// </summary>
        /// <returns>A list with all the games if they're found, null otherwiser</returns>
        public static List<GameBeingPlayed> getGamesBeingPlayed()
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            GameBeingPlayed game = null;
            List<GameBeingPlayed> list = new List<GameBeingPlayed>();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT name, numberOfPlayers, numberOfPlayersPlaying, totalHoursPlayed FROM GetGamesStats() ORDER BY numberOfPlayersPlaying desc, numberOfPlayers desc, totalHoursPlayed desc, name desc";

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //Read the result and assign values
                        game = new GameBeingPlayed();
                        game.game = (string)reader["name"];
                        game.numberOfPlayers = (int)reader["numberOfPlayers"];
                        game.numberOfPlayersPlaying = (int)reader["numberOfPlayersPlaying"];
                        game.totalRegisteredHours = (decimal)reader["totalHoursPlayed"];
                        list.Add(game);
                    }
                }

            }
            catch (SqlException ex) { list = null; }
            catch (Exception ex) { list = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return list;
        }

        /// <summary>
        /// Method which will save the game in the database
        /// </summary>
        /// <param name="relationship">The relationship to be saved</param>
        /// <returns>1 if saved successfully; 0 an error in SQL; -1 other errors</returns>
        public static int banUserAndIp(User user)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = null;
            int returnStatus;

            try
            {
                //Get connection
                sqlConnection = connection.getConnection();

                //Prepare command
                command = new SqlCommand("BanUserAndIP", sqlConnection);
                command.CommandType = CommandType.StoredProcedure;

                //Create parameters
                command.Parameters.Add("@userToBan", SqlDbType.VarChar).Value = user.nickname;
                command.Parameters.Add("@bannedUntil", SqlDbType.DateTime).Value = user.bannedUntil;
                command.Parameters.Add("@banReason", SqlDbType.VarChar).Value = user.banReason;
                command.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;
                
                //Execute query
                command.ExecuteNonQuery();

                returnStatus = (int)command.Parameters["@status"].Value;
            }
            catch (Exception ex)
            {
                returnStatus = -1; //Instead of throwing exception, change returnStatus to -1
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return returnStatus;
        }

        /// <summary>
        /// Method which will pardon an user from his ban
        /// </summary>
        /// <param name="relationship">The relationship to be saved</param>
        /// <returns>1 if saved successfully; 0 an error in SQL; -1 other errors</returns>
        public static int pardonUser(string nickname)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = null;
            int returnStatus;

            try
            {
                //Get connection
                sqlConnection = connection.getConnection();

                //Prepare command
                command = new SqlCommand("PardonUser", sqlConnection);
                command.CommandType = CommandType.StoredProcedure;

                //Create parameters
                command.Parameters.Add("@userToPardon", SqlDbType.VarChar).Value = nickname;
                command.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;

                //Execute query
                command.ExecuteNonQuery();

                returnStatus = (int)command.Parameters["@status"].Value;
            }
            catch (Exception ex)
            {
                returnStatus = -1; //Instead of throwing exception, change returnStatus to -1
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return returnStatus;
        }

        /// <summary>
        /// Method which will pardon an user and his IP from the ban
        /// </summary>
        /// <param name="relationship">The relationship to be saved</param>
        /// <returns>1 if saved successfully; 0 an error in SQL; -1 other errors</returns>
        public static int pardonUserAndIP(string nickname)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = null;
            int returnStatus;

            try
            {
                //Get connection
                sqlConnection = connection.getConnection();

                //Prepare command
                command = new SqlCommand("PardonUserAndIP", sqlConnection);
                command.CommandType = CommandType.StoredProcedure;

                //Create parameters
                command.Parameters.Add("@userToPardon", SqlDbType.VarChar).Value = nickname;
                command.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;

                //Execute query
                command.ExecuteNonQuery();

                returnStatus = (int)command.Parameters["@status"].Value;
            }
            catch (Exception ex)
            {
                returnStatus = -1; //Instead of throwing exception, change returnStatus to -1
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return returnStatus;
        }

        /// <summary>
        /// Method which will search the database for all the users and bring their usernames
        /// </summary>
        /// <returns>A list with all the games if they're found, null otherwiser</returns>
        public static List<string> getAllUsers()
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            List<string> list = new List<string>();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT nickname FROM Users WHERE admin = 0";

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //Read the result and assign values
                        list.Add((string)reader["nickname"]);
                    }
                }

            }
            catch (SqlException ex) { list = null; }
            catch (Exception ex) { list = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return list;
        }

        /// <summary>
        /// Method which will search the database for all the users and bring their usernames
        /// </summary>
        /// <returns>A list with all the games if they're found, null otherwiser</returns>
        public static List<string> getAllStoreGamesNames()
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            List<string> list = new List<string>();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT name FROM Games";

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //Read the result and assign values
                        list.Add((string)reader["name"]);
                    }
                }

            }
            catch (SqlException ex) { list = null; }
            catch (Exception ex) { list = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return list;
        }

        /// <summary>
        /// Method which will search the database for all the banned users and bring their usernames
        /// </summary>
        /// <returns>A list with all the games if they're found, null otherwiser</returns>
        public static List<string> getAllBannedUsers()
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            List<string> list = new List<string>();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT nickname FROM Users WHERE bannedUntil > CURRENT_TIMESTAMP";

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //Read the result and assign values
                        list.Add((string)reader["nickname"]);
                    }
                }

            }
            catch (SqlException ex) { list = null; }
            catch (Exception ex) { list = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return list;
        }

        /// <summary>
        /// Method which will change the game that is displayed in the front page
        /// </summary>
        /// <param name="name">Name of the game to be set to front page</param>
        /// <returns>1 if updated successfully; 0 if no game was found; -1 otherwise</returns>
        public static int updateFrontPageGame(string name)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE Games SET frontPage = 1 WHERE name = @name";

                //Create parameters
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = name;

                //Get connection
                sqlConnection = connection.getConnection();

                //Give the connection to the command
                command.Connection = sqlConnection;

                //Execute query
                affectedRows = command.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {
                affectedRows = -1; //Instead of throwing exception, change affectedRows to -1
            }
            catch (Exception ex)
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
    }
}
