using CHAIRAPI_Entities.Complex;
using DAL.Conexion;
using System;
using System.Collections.Generic;
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
                command.CommandText = "SELECT game, SUM(CASE WHEN playing=1 THEN 1 ELSE 0 END) AS numberOfPlayers, COUNT(*) AS numberOfPlayersPlaying FROM UserGames GROUP BY game";

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
                        game.game = (string)reader["game"];
                        game.numberOfPlayers = (int)reader["numberOfPlayers"];
                        game.numberOfPlayersPlaying = (int)reader["numberOfPlayersPlaying"];
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
    }
}
