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
    public class GameStoreHandler
    {
        /// <summary>
        /// Method which will search the database for the game with the specified name
        /// </summary>
        /// <param name="name">The name of the game to be searched</param>
        /// <returns>The game with all its information and the relationship, false otherwise</returns>
        public static GameStore searchGameByNameAndUser(string game, string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlDataReader readerRel = null;
            SqlCommand command = new SqlCommand();
            SqlCommand commandRel = new SqlCommand();
            Connection connection = new Connection();
            GameStore gameStore = new GameStore();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT name, description, developer, minimumAge, releaseDate, instructions, downloadUrl, storeImageUrl, libraryImageUrl FROM Games WHERE name = @game";

                //Set the parameter
                command.Parameters.Add("@game", SqlDbType.VarChar).Value = game;

                //Define the connection
                command.Connection = sqlConnection;
                commandRel.Connection = sqlConnection;
                commandRel.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Read the result and assign values
                    reader.Read();
                    gameStore.game.name = (string)reader["name"];
                    gameStore.game.description = (string)reader["description"];
                    gameStore.game.developer = (string)reader["developer"];
                    gameStore.game.minimumAge = reader["minimumAge"] is DBNull ? 0 : (int)reader["minimumAge"];
                    gameStore.game.releaseDate = reader["releaseDate"] is DBNull ? new DateTime() : (DateTime)reader["releaseDate"];
                    gameStore.game.instructions = reader["instructions"] is DBNull ? "" : (string)reader["instructions"];
                    gameStore.game.downloadUrl = reader["downloadUrl"] is DBNull ? "" : (string)reader["downloadUrl"];
                    gameStore.game.storeImageUrl = reader["storeImageUrl"] is DBNull ? "" : (string)reader["storeImageUrl"];
                    gameStore.game.libraryImageUrl = reader["libraryImageUrl"] is DBNull ? "" : (string)reader["libraryImageUrl"];

                    //Prepare the statement for the relationship of the user with this game
                    commandRel.CommandText = "SELECT [user], game, acquisitionDate FROM UserGames WHERE [user] = @nickname AND game = @game";
                    commandRel.Parameters.Add("@game", SqlDbType.VarChar).Value = game;
                    commandRel.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;
                    readerRel = commandRel.ExecuteReader();

                    if (readerRel.HasRows)
                    {
                        readerRel.Read();

                        gameStore.relationship.user = (string)readerRel["user"];
                        gameStore.relationship.game = (string)readerRel["game"];
                        gameStore.relationship.acquisitionDate = (DateTime)readerRel["acquisitionDate"];
                    }
                    else
                        gameStore.relationship = null;
                }

            }
            catch (SqlException) { gameStore = null; }
            catch (Exception) { gameStore = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return gameStore;
        }

        /// <summary>
        /// Method which will search the database for all games and return all of the games available with the basic information to be displayed in a list
        /// </summary>
        /// <returns>A list with all the games if they're found, null otherwiser</returns>
        public static List<Game> getAllGames()
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            Game game = null;
            List<Game> list = new List<Game>();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT name, description, developer, minimumAge, releaseDate, instructions, downloadUrl, storeImageUrl, libraryImageUrl, frontPage FROM Games";

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
                        game = new Game();
                        game.name = (string)reader["name"];
                        game.description = (string)reader["description"];
                        game.developer = (string)reader["developer"];
                        game.minimumAge = reader["minimumAge"] is DBNull ? 0 : (int)reader["minimumAge"];
                        game.releaseDate = reader["releaseDate"] is DBNull ? new DateTime() : (DateTime)reader["releaseDate"];
                        game.instructions = reader["instructions"] is DBNull ? "" : (string)reader["instructions"];
                        game.downloadUrl = reader["downloadUrl"] is DBNull ? "" : (string)reader["downloadUrl"];
                        game.storeImageUrl = reader["storeImageUrl"] is DBNull ? "" : (string)reader["storeImageUrl"];
                        game.libraryImageUrl = reader["libraryImageUrl"] is DBNull ? "" : (string)reader["libraryImageUrl"];
                        game.frontPage = (bool)reader["frontPage"];
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
