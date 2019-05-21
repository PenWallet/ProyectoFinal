using CHAIRAPI_Entidades.Complex;
using CHAIRAPI_Entities.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class UserForFriendListHandler
    {
        /// <summary>
        /// Method which will search the database for the user with the specified nickname
        /// </summary>
        /// <param name="user1">One of the nicknames to search</param>
        /// <param name="user2">One of the nicknames to search</param>
        /// <returns>The user with all its information if it was found, false otherwise</returns>
        public static List<UserForFriendList> searchFriends(string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            List<UserForFriendList> list = null;
            UserForFriendList tempUser = null;

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT user1, user2, acceptedRequestDate, nickname, online, admin, game FROM GetFriends(@nickname)";

                //Set the parameter
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;

                //Define the connection
                command.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    list = new List<UserForFriendList>();

                    //Read the result and assign values
                    while (reader.Read())
                    {
                        tempUser = new UserForFriendList();
                        tempUser.relationship.user1 = (string)reader["user1"];
                        tempUser.relationship.user2 = (string)reader["user2"];
                        tempUser.relationship.acceptedRequestDate = reader["acceptedRequestDate"] is DBNull ? null : (DateTime?)reader["acceptedRequestDate"];

                        tempUser.nickname = (string)reader["nickname"];
                        tempUser.online = (bool)reader["online"];
                        tempUser.admin = (bool)reader["online"];
                        tempUser.gamePlaying = reader["game"] is DBNull ? null : (string)reader["game"];

                        list.Add(tempUser);
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
