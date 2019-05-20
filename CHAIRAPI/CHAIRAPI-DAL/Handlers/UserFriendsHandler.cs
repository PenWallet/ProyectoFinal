using CHAIRAPI_Entities.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class UserFriendsHandler
    {
        /// <summary>
        /// Method which will save the game in the database
        /// </summary>
        /// <param name="relationship">The relationship to be saved</param>
        /// <returns>1 if saved successfully; 0 if the relationship already exists; -1 other errors</returns>
        public static int saveNewRelationship(string user1, string user2)
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
                command = new SqlCommand("InsertNewFriendship", sqlConnection);
                command.CommandType = CommandType.StoredProcedure;

                //Create parameters
                command.Parameters.Add("@user1", SqlDbType.VarChar).Value = user1;
                command.Parameters.Add("@user2", SqlDbType.VarChar).Value = user2;
                command.Parameters.Add("@status", SqlDbType.Int).Direction = ParameterDirection.Output;
                
                //Execute query
                command.ExecuteNonQuery();

                returnStatus = (int)command.Parameters["@status"].Value;
            }
            catch (Exception ex)
            {
                returnStatus = -2; //Instead of throwing exception, change returnStatus to -2
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return returnStatus;
        }

        /// <summary>
        /// Method which will save the changes made to the game in the database
        /// </summary>
        /// <param name="relationship">The relationship to be updated</param>
        /// <returns>True if updated successfully, false otherwise</returns>
        public static int acceptRelationship(string user1, string user2)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE UserFriends SET acceptedRequestDate = CURRENT_TIMESTAMP WHERE user1 = @user1 AND user2 = @user2 OR user1 = @user2 AND user2 = @user1";

                //Create parameters
                command.Parameters.Add("@user1", SqlDbType.VarChar).Value = user1;
                command.Parameters.Add("@user2", SqlDbType.VarChar).Value = user2;

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
        /// Method which will delete a relationship in the database
        /// </summary>
        /// <param name="relationship">The relationship to delete</param>
        /// <returns>1 if deleted successfully; 0 if there's no relationship like that; -1 otherwise</returns>
        public static int deleteRelationship(string user1, string user2)
        {
            //Variables
            SqlConnection sqlConnection = null;
            int affectedRows = -1;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "DELETE FROM UserFriends WHERE user1 = @user1 AND user2 = @user2 OR user1 = @user2 AND user2 = @user1";

                //Set the parameter
                command.Parameters.Add("@user1", SqlDbType.VarChar).Value = user1;
                command.Parameters.Add("@user2", SqlDbType.VarChar).Value = user2;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                affectedRows = command.ExecuteNonQuery();

            }
            catch (SqlException) { affectedRows = -1; }
            catch (Exception) { affectedRows = -1; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
            }

            return affectedRows;
        }

        /// <summary>
        /// Method which will search the database for the user with the specified nickname
        /// </summary>
        /// <param name="user1">One of the nicknames to search</param>
        /// <param name="user2">One of the nicknames to search</param>
        /// <returns>The user with all its information if it was found, false otherwise</returns>
        public static UserFriends searchRelationshipByUsers(string user1, string user2)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            UserFriends relationship = null;

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT user1, user2, acceptedRequestDate FROM UserFriends WHERE user1 = @user1 AND user2 = @user2 OR user1 = @user2 AND user2 = @user1";

                //Set the parameter
                command.Parameters.Add("@user1", SqlDbType.VarChar).Value = user1;
                command.Parameters.Add("@user2", SqlDbType.VarChar).Value = user2;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Read the result and assign values
                    reader.Read();
                    relationship = new UserFriends();
                    relationship.user1 = (string)reader["user1"];
                    relationship.user2 = (string)reader["user2"];
                    relationship.acceptedRequestDate = reader["acceptedRequestDate"] is DBNull ? null : (DateTime?)reader["acceptedRequestDate"];
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
        /// Method which will search the database for the friends of the specified user
        /// </summary>
        /// <param name="nickname">One of the nicknames to search</param>
        /// <returns>A list with all the relationships if they're found, null otherwiser</returns>
        public static List<UserFriends> searchRelationshipsByUser(string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            UserFriends relationship = null;
            List<UserFriends> list = null;

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT user1, user2, acceptedRequestDate FROM UserFriends WHERE user1 = @nickname OR user2 = @nickname";

                //Set the parameter
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    list = new List<UserFriends>();

                    while (reader.Read())
                    {
                        //Read the result and assign values
                        relationship = new UserFriends();
                        relationship.user1 = (string)reader["user1"];
                        relationship.user2 = (string)reader["user2"];
                        relationship.acceptedRequestDate = reader["acceptedRequestDate"] is DBNull ? null : (DateTime?)reader["acceptedRequestDate"];

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

    }
}
