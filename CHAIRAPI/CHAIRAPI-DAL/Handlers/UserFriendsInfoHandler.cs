using CHAIRAPI_Entidades.Complex;
using CHAIRAPI_Entidades.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class UserFriendsInfoHandler
    {
        /// <summary>
        /// Method which will search the database for all the games the specified user plays, along with the information about the game and
        /// all the friends who play the same game
        /// </summary>
        /// <param name="nickname">The user who wants all the games he plays</param>
        /// <returns>A list with all the relationships if they're found, null otherwiser</returns>
        public static List<UserGamesWithGameAndFriends> searchAllGames(string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            UserFriends relationship = null;
            List<UserGamesWithGameAndFriends> list = null;

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
                    list = new List<UserGamesWithGameAndFriends>();

                    while (reader.Read())
                    {
                        //Read the result and assign values
                        relationship = new UserFriends();
                        relationship.user1 = (string)reader["user1"];
                        relationship.user2 = (string)reader["user2"];
                        relationship.acceptedRequestDate = reader["acceptedRequestDate"] is DBNull ? null : (DateTime?)reader["acceptedRequestDate"];

                        //list.Add(relationship);
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
