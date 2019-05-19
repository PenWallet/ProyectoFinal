using CHAIRAPI_Entities.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class GamesHandler
    {
        /// <summary>
        /// Method which will save the game in the database
        /// </summary>
        /// <param name="game">The game to be saved</param>
        /// <returns>1 if saved successfully; 0 if the name already exists; -1 other errors</returns>
        public static int saveNewGame(Game game)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "INSERT INTO Games(name, description, developer, minimumAge, releaseDate, instructions, downloadUrl, storeImageUrl, libraryImageUrl) VALUES (@name, @description, @developer, @minimumAge, @releaseDate, @instructions, @downloadUrl, @storeImageUrl, @libraryImageUrl)";

                //Create parameters
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = game.name;
                command.Parameters.Add("@description", SqlDbType.VarChar).Value = game.description;
                command.Parameters.Add("@developer", SqlDbType.VarChar).Value = game.developer;
                command.Parameters.Add("@minimumAge", SqlDbType.TinyInt).Value = game.minimumAge;
                command.Parameters.Add("@releaseDate", SqlDbType.Date).Value = game.releaseDate;
                command.Parameters.Add("@instructions", SqlDbType.VarChar).Value = game.instructions;
                command.Parameters.Add("@downloadUrl", SqlDbType.VarChar).Value = game.downloadUrl;
                command.Parameters.Add("@storeImageUrl", SqlDbType.VarChar).Value = game.storeImageUrl;
                command.Parameters.Add("@libraryImageUrl", SqlDbType.VarChar).Value = game.libraryImageUrl;

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
            catch (Exception) {
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
        /// Method which will save the changes made to the game in the database
        /// </summary>
        /// <param name="game">The game to be updated</param>
        /// <returns>1 if updated successfully; 0 if no game was found; -1 otherwise</returns>
        public static int updateGame(Game game)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE Games SET description = @description, developer = @developer, minimumAge = @minimumAge, releaseDate = @releaseDate, instructions = @instructions, downloadUrl = @downloadUrl, storeImageUrl = @storeImageUrl, libraryImageUrl = @libraryImageUrl, frontPage = @frontPage WHERE name = @name";

                //Create parameters
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = game.name;
                command.Parameters.Add("@description", SqlDbType.VarChar).Value = game.description;
                command.Parameters.Add("@developer", SqlDbType.VarChar).Value = game.developer;
                command.Parameters.Add("@minimumAge", SqlDbType.TinyInt).Value = game.minimumAge;
                command.Parameters.Add("@releaseDate", SqlDbType.Date).Value = game.releaseDate;
                command.Parameters.Add("@instructions", SqlDbType.VarChar).Value = game.instructions;
                command.Parameters.Add("@downloadUrl", SqlDbType.VarChar).Value = game.downloadUrl;
                command.Parameters.Add("@storeImageUrl", SqlDbType.VarChar).Value = game.storeImageUrl;
                command.Parameters.Add("@libraryImageUrl", SqlDbType.VarChar).Value = game.libraryImageUrl;
                command.Parameters.Add("@frontPage", SqlDbType.Bit).Value = game.frontPage;

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
        /// Method which will search the database for the game with the specified name
        /// </summary>
        /// <param name="name">The name of the game to be searched</param>
        /// <returns>The game with all its information if it was found, false otherwise</returns>
        public static Game searchGameByName(string name)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            Game game = null;


            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT name, description, developer, minimumAge, releaseDate, instructions, downloadUrl, storeImageUrl, libraryImageUrl FROM Games WHERE name = @name";

                //Set the parameter
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = name;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Read the result and assign values
                    reader.Read();
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
                }

            }
            catch (SqlException) { game = null; }
            catch (Exception) { game = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return game;
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
