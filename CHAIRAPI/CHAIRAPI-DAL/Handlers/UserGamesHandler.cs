using CHAIRAPI_Entities.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class UserGamesHandler
    {
        /// <summary>
        /// Method which will save the relationship in the database
        /// </summary>
        /// <param name="relationship">The relationship to be saved</param>
        /// <returns>1 if saved successfully; 0 if the specified user or game can't be found; -1 if the relationship already exists; -2 other errors</returns>
        public static int saveNewRelationship(UserGames relationship)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "INSERT INTO UserGames([user], game) VALUES (@user, @game)";

                //Create parameters
                command.Parameters.Add("@user", SqlDbType.VarChar).Value = relationship.user;
                command.Parameters.Add("@game", SqlDbType.VarChar).Value = relationship.game;

                //Get connection
                sqlConnection = connection.getConnection();

                //Give the connection to the command
                command.Connection = sqlConnection;

                //Execute query
                affectedRows = command.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {

                if (ex.Number == 547) //FOREIGN KEY Exception (Can't find the specified user or game)
                    affectedRows = 0;
                else if (ex.Number == 2627) //Duplicate PRIMARY KEY Exception Number
                    affectedRows = -1;
                else
                    affectedRows = -2; //Instead of throwing exception, change affectedRows to -2
            }
            catch (Exception)
            {
                affectedRows = -2; //Instead of throwing exception, change affectedRows to -2
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return affectedRows;
        }

        /// <summary>
        /// Method which will save the changes made to the relationship in the database
        /// </summary>
        /// <param name="relationship">The relationship to be updated</param>
        /// <returns>True if updated successfully, false otherwise</returns>
        public static int updateRelationship(UserGames relationship)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE UserGames SET hoursPlayed = @hoursPlayed,"+(relationship.lastPlayed == new DateTime() ? "" : " lastPlayed = @lastPlayed,")+" playing = @playing WHERE [user] = @user AND game = @game";

                //Create parameters
                command.Parameters.Add("@hoursPlayed", SqlDbType.Decimal).Value = relationship.hoursPlayed;
                command.Parameters.Add("@playing", SqlDbType.Bit).Value = relationship.playing;
                command.Parameters.Add("@user", SqlDbType.VarChar).Value = relationship.user;
                command.Parameters.Add("@game", SqlDbType.VarChar).Value = relationship.game;
                if (relationship.lastPlayed != new DateTime())
                    command.Parameters.Add("@lastPlayed", SqlDbType.DateTime).Value = relationship.lastPlayed;

                //Get connection
                sqlConnection = connection.getConnection();

                //Give the connection to the command
                command.Connection = sqlConnection;

                //Execute query
                affectedRows = command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) //FOREIGN KEY Exception (Can't find the specified user or game)
                    affectedRows = 0;
                else
                    affectedRows = -1; //Instead of throwing exception, change affectedRows to -1
            }
            catch (Exception)
            {
                affectedRows = -1; //Instead of throwing exception, change affectedRows to -2
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return affectedRows;
        }

        /// <summary>
        /// Method which will search the database for the relationship given the user and game
        /// </summary>
        /// <param name="user">The nickname of the user to be searched</param>
        /// <param name="game">The name of the game to be searched</param>
        /// <returns>The relationship with all its information if it was found, null otherwise</returns>
        public static UserGames searchRelationshipByUserAndGame(string user, string game)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            UserGames relationship = null;

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT [user], game, hoursPlayed, acquisitionDate, lastPlayed, playing FROM UserGames WHERE [user] = @user AND game = @game";

                //Set the parameter
                command.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
                command.Parameters.Add("@game", SqlDbType.VarChar).Value = game;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Read the result and assign values
                    reader.Read();
                    relationship = new UserGames();
                    relationship.user = (string)reader["user"];
                    relationship.game = (string)reader["game"];
                    relationship.hoursPlayed = (decimal)reader["hoursPlayed"];
                    relationship.acquisitionDate = (DateTime)reader["acquisitionDate"];
                    relationship.lastPlayed = reader["lastPlayed"] is DBNull ? null : (DateTime?)reader["lastPlayed"];
                    relationship.playing = (bool)reader["playing"];
                }

            }
            catch (SqlException ex) { relationship = null; }
            catch (Exception ex) { relationship = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return relationship;
        }

        /// <summary>
        /// Method which will search the database for all the games an user owns given the user's nickname
        /// </summary>
        /// <param name="nickname">The nickname of the user to be searched</param>
        /// <returns>The userGames with all its information if they were found, false otherwise</returns>
        public static List<UserGames> searchRelationshipsByUser(string user)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            List<UserGames> list = null;
            UserGames relationship = null;

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT [user], game, hoursPlayed, acquisitionDate, lastPlayed, playing FROM UserGames WHERE [user] = @user";

                //Set the parameter
                command.Parameters.Add("@user", SqlDbType.VarChar).Value = user;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    list = new List<UserGames>();

                    //While there are results, read
                    while (reader.Read())
                    {
                        relationship = new UserGames();
                        relationship.user = (string)reader["user"];
                        relationship.game = (string)reader["game"];
                        relationship.hoursPlayed = (decimal)reader["hoursPlayed"];
                        relationship.acquisitionDate = (DateTime)reader["acquisitionDate"];
                        relationship.lastPlayed = reader["lastPlayed"] is DBNull ? null : (DateTime?)reader["lastPlayed"];
                        relationship.playing = (bool)reader["playing"];
                        list.Add(relationship);
                    }
                }
            }
            catch (SqlException ex) { relationship = null; }
            catch (Exception ex) { relationship = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return list;
        }

        /// <summary>
        /// Method used to update the information of an user's relationship with a game
        /// </summary>
        /// <param name="user">The nickname of the user</param>
        /// <param name="game">The name of the game</param>
        /// <param name="playing">Whether the user is playing or not</param>
        /// <param name="secondsToAdd">The total amount of seconds the user played for (only necessary if playing is false)</param>
        /// <returns>1 if it updated successfully; 0 if the user can't be found; -1 otherwise</returns>
        public static int updatePlayingStatus(string user, string game, bool playing, int secondsToAdd)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                if(playing)
                    command.CommandText = "UPDATE UserGames SET playing = @playing WHERE [user] = @user AND game = @game";
                else
                    command.CommandText = "UPDATE UserGames SET playing = @playing, hoursPlayed += @hoursPlayed WHERE [user] = @user AND game = @game";

                //Create parameters
                command.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
                command.Parameters.Add("@game", SqlDbType.VarChar).Value = game;
                command.Parameters.Add("@playing", SqlDbType.Bit).Value = playing ? 1 : 0;
                if(!playing) command.Parameters.Add("@hoursPlayed", SqlDbType.Decimal).Value = decimal.Divide(secondsToAdd, 3600);

                //Get connection
                sqlConnection = connection.getConnection();

                //Give the connection to the command
                command.Connection = sqlConnection;

                //Execute query
                affectedRows = command.ExecuteNonQuery();

            }
            catch (Exception)
            {
                affectedRows = -1;
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
