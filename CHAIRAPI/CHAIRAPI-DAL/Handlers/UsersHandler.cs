using CHAIRAPI_Entidades.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class UsersHandler
    {
        /// <summary>
        /// Method which will save the user in the database
        /// </summary>
        /// <param name="user">The user to be saved</param>
        /// <returns>1 if saved successfully; 0 if the nickname already exists; -1 other errors</returns>
        public static int saveNewUser(UserWithSalt user)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "INSERT INTO Users(nickname, password, salt, birthDate, privateProfile, lastIP)" +
                    "VALUES (@nickname, @password, @salt, @birthDate, @privateProfile, @lastIP)";

                //Create parameters
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = user.nickname;
                command.Parameters.Add("@password", SqlDbType.VarChar).Value = user.password;
                command.Parameters.Add("@salt", SqlDbType.VarChar).Value = user.salt;
                command.Parameters.Add("@birthDate", SqlDbType.DateTime).Value = user.birthDate;
                command.Parameters.Add("@privateProfile", SqlDbType.Bit).Value = user.privateProfile;
                command.Parameters.Add("@lastIP", SqlDbType.VarChar).Value = user.lastIP;

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
            catch(Exception ex)
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
        /// Method used to update the information of an user given the updated information and its nickname
        /// </summary>
        /// <param name="user">The updated info of the user</param>
        /// <param name="nickname">The nickname of the user (in case the user is changing his nickname)</param>
        /// <returns>1 if it updated successfully; 0 if the user can't be found; -1 if the nickname is taken; -2 otherwise</returns>
        public static int updateUser(UserWithSalt user, string nickname)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE Users SET nickname = @nickname, [password] = @password, salt = @salt, profileDescription = @profileDescription, profileLocation = @profileLocation, birthDate = @birthDate, privateProfile = @privateProfile, lastIP = @lastIP WHERE nickname = @previousNickname";

                //Create parameters
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = user.nickname;
                command.Parameters.Add("@password", SqlDbType.VarChar).Value = user.password;
                command.Parameters.Add("@salt", SqlDbType.VarChar).Value = user.salt;
                command.Parameters.Add("@profileDescription", SqlDbType.VarChar).Value = user.profileDescription ?? "";
                command.Parameters.Add("@profileLocation", SqlDbType.VarChar).Value = user.profileLocation ?? "";
                command.Parameters.Add("@birthDate", SqlDbType.DateTime).Value = user.birthDate;
                command.Parameters.Add("@privateProfile", SqlDbType.Bit).Value = user.privateProfile;
                command.Parameters.Add("@lastIP", SqlDbType.VarChar).Value = user.lastIP;
                command.Parameters.Add("@previousNickname", SqlDbType.VarChar).Value = nickname;

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
        /// Method used to update the information of an user given the updated information and its nickname
        /// </summary>
        /// <param name="user">The updated info of the user</param>
        /// <param name="nickname">The nickname of the user (in case the user is changing his nickname)</param>
        /// <returns>1 if it updated successfully; 0 if the user can't be found; -1 otherwise</returns>
        public static int updateUserStatus(string nickname, bool online)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE Users SET online = @online WHERE nickname = @nickname";

                //Create parameters
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;
                command.Parameters.Add("@online", SqlDbType.Bit).Value = online ? 1 : 0;

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

        /// <summary>
        /// Method used to update the information of an user in order to ban 
        /// </summary>
        /// <param name="user">The user to be banned. Has to contain nickname, banReason and bannedUntil</param>
        /// <returns>1 if it updated successfully; 0 if the user can't be found; -1 otherwise</returns>
        public static int banUser(User user)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE Users SET banReason = @banReason, bannedUntil = @bannedUntil WHERE nickname = @nickname";

                //Create parameters
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = user.nickname;
                command.Parameters.Add("@banReason", SqlDbType.VarChar).Value = user.banReason;
                command.Parameters.Add("@bannedUntil", SqlDbType.DateTime).Value = user.bannedUntil;

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

        /// <summary>
        /// Method used to update the information of an user in order to pardon a ban
        /// </summary>
        /// <param name="nickname">The nickname of the user to be pardoned.</param>
        /// <returns>1 if it updated successfully; 0 if the user can't be found; -1 otherwise</returns>
        public static int pardonUser(string nickname)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            int affectedRows = -1;

            try
            {
                //Define parameters
                command.CommandText = "UPDATE Users SET banReason = '', bannedUntil = NULL WHERE nickname = @nickname";

                //Create parameters
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;

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

        /// <summary>
        /// Method which will search the database for the user with the specified nickname
        /// </summary>
        /// <param name="nickname">The nickname of the user to be searched</param>
        /// <returns>The user with all its information if it was found, false otherwise</returns>
        public static UserWithSalt searchUserByNickname(string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            UserWithSalt user = null;


            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT nickname, password, salt, profileDescription, profileLocation, birthDate, privateProfile, accountCreationDate, admin, lastIP, bannedUntil, banReason FROM Users WHERE nickname = @nickname";

                //Set the parameter
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Read the result and assign values
                    reader.Read();
                    user = new UserWithSalt();
                    user.nickname = (string)reader["nickname"];
                    user.salt = (string)reader["salt"];
                    user.password = (string)reader["password"];
                    user.birthDate = (DateTime)reader["birthDate"];
                    user.privateProfile = (bool)reader["privateProfile"];
                    user.accountCreationDate = (DateTime)reader["accountCreationDate"];
                    user.admin = (bool)reader["admin"];
                    user.lastIP = reader["lastIP"] is DBNull ? null : (string)reader["lastIP"];
                    user.bannedUntil = reader["bannedUntil"] is DBNull ? null : (DateTime?)reader["bannedUntil"];
                    user.banReason = reader["banReason"] is DBNull ? "" : (string)reader["banReason"];
                    user.profileDescription = (string)reader["profileDescription"];
                    user.profileLocation = (string)reader["profileLocation"];
                }

            }
            catch (SqlException ex) { user = null; }
            catch (Exception ex) { user = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return user;
        }

        /// <summary>
        /// Method which will search the database for the user with the specified nickname and return the basic information
        /// </summary>
        /// <param name="nickname">The nickname of the user to be searched</param>
        /// <returns>The user with all its information if it was found, false otherwise</returns>
        public static User searchBasicUserByNickname(string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            User user = null;


            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT nickname, profileDescription, profileLocation, birthDate, privateProfile, accountCreationDate, online, admin, bannedUntil, banReason FROM Users WHERE nickname = @nickname";

                //Set the parameter
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Read the result and assign values
                    reader.Read();
                    user = new User();
                    user.nickname = (string)reader["nickname"];
                    user.profileDescription = (string)reader["profileDescription"];
                    user.profileLocation = (string)reader["profileLocation"];
                    user.birthDate = (DateTime)reader["birthDate"];
                    user.privateProfile = (bool)reader["privateProfile"];
                    user.accountCreationDate = (DateTime)reader["accountCreationDate"];
                    user.online = (bool)reader["online"];
                    user.admin = (bool)reader["admin"];
                    user.bannedUntil = reader["bannedUntil"] is DBNull ? null : (DateTime?)reader["bannedUntil"];
                    user.banReason = reader["banReason"] is DBNull ? "" : (string)reader["banReason"];
                }

            }
            catch (SqlException ex) { user = null; }
            catch (Exception ex) { user = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return user;
        }
    }
}
