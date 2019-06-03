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
    public class UserSearchHandler
    {
        /// <summary>
        /// Method which will get the users whose nicknames match the 'search' parameter
        /// <param name="search">The string to be searched</param>
        /// <param name="nickname">The nickname of the user searching (used to exclude his name from the results)</param>
        /// <returns>The user with all its information if it was found, false otherwise</returns>
        public static List<UserSearch> searchUsersFromString(string search, string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlDataReader readerRel = null;
            SqlCommand command = new SqlCommand();
            SqlCommand commandRel = new SqlCommand();
            Connection connection = new Connection();
            UserSearch user = new UserSearch();
            List<UserSearch> searchList = new List<UserSearch>();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT nickname, profileDescription, admin, [online], bannedUntil, banReason FROM Users WHERE nickname LIKE @search AND nickname != @nickname";
                commandRel.CommandText = "SELECT acceptedRequestDate FROM UserFriends WHERE user1 = @user1 AND user2 = @user2 OR user1 = @user2 AND user2 = @user1";

                //Set the parameter
                command.Parameters.AddWithValue("@search", "%" + search + "%");
                command.Parameters.AddWithValue("@nickname", nickname);
                commandRel.Parameters.Add("@user1", SqlDbType.VarChar).Value = nickname;;
                commandRel.Parameters.Add("@user2", SqlDbType.VarChar);

                //Define the connection
                command.Connection = sqlConnection;
                commandRel.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Read every result from the search
                    while(reader.Read())
                    {
                        user = new UserSearch();
                        user.user.nickname = (string)reader["nickname"];
                        user.user.profileDescription = (string)reader["profileDescription"];
                        user.user.admin = (bool)reader["admin"];
                        user.user.online = (bool)reader["online"];
                        user.user.bannedUntil = reader["bannedUntil"] is DBNull ? null : (DateTime?)reader["bannedUntil"];
                        user.user.banReason = reader["banReason"] is DBNull ? "" : (string)reader["banReason"];

                        //Set relationship parameters and execuet
                        commandRel.Parameters["@user2"].Value = user.user.nickname;
                        readerRel = commandRel.ExecuteReader();

                        if(readerRel.HasRows)
                        {
                            readerRel.Read();
                            user.relationshipExists = true;
                            user.friendsSince = readerRel["acceptedRequestDate"] is DBNull ? null : (DateTime?)readerRel["acceptedRequestDate"];
                        }
                        else
                        {
                            user.friendsSince = null;
                            user.relationshipExists = false;
                        }

                        readerRel.Close();
                        searchList.Add(user);
                    }
                }

            }
            catch (SqlException ex) { searchList = null; }
            catch (Exception ex) { searchList = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
                readerRel?.Close();
            }

            return searchList;
        }
    }
}
