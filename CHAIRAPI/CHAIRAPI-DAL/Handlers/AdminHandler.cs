using CHAIRAPI_Entities.Complex;
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
                command.CommandText = "SELECT name, numberOfPlayers, numberOfPlayersPlaying, totalHoursPlayed FROM GetGamesStats()";

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
            GameBeingPlayed game = null;
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
                command.CommandText = "UPDATE Games SET frontPage = @frontPage WHERE name = @name";

                //Create parameters
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = name;

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
    }
}
