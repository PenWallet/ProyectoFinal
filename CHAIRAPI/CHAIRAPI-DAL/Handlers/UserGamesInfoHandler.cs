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
    public class UserGamesInfoHandler
    {
        /// <summary>
        /// Method which will search the database for all the games the specified user plays, along with the information about the game and
        /// all the friends who play the same game
        /// </summary>
        /// <param name="nickname">The nickname of the user who wants all the games he plays</param>
        /// <returns>A list with all the relationships if they're found, null otherwiser</returns>
        public static List<UserGamesWithGameAndFriends> searchAllMyGamesAndFriendsWhoPlayThem(string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlDataReader readerFriends = null;
            SqlCommand command = new SqlCommand();
            SqlCommand commandFriends = new SqlCommand();
            Connection connection = new Connection();
            UserGamesWithGameAndFriends relationship = null;
            List<UserGamesWithGameAndFriends> list = new List<UserGamesWithGameAndFriends>();
            List<UserWhoPlaysMyGame> searchList = null;

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT UG.game, UG.hoursPlayed, UG.acquisitionDate, UG.lastPlayed, UG.playing, G.description, G.developer, G.minimumAge, G.releaseDate, G.frontPage, G.instructions, G.downloadUrl, G.storeImageUrl, G.libraryImageUrl FROM UserGames AS UG INNER JOIN Games AS G ON UG.game = G.name WHERE UG.[user] = @nickname";

                //Set the parameter
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;

                //Define the connections
                command.Connection = sqlConnection;
                commandFriends.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    //Prepare the friends call
                    commandFriends.CommandText = "SELECT nickname AS frname, frgame, privateProfile, [online], [admin] FROM GetFriendsWhoPlayMyGames(@nickname)";
                    commandFriends.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;
                    readerFriends = commandFriends.ExecuteReader();
                    List<UserWhoPlaysMyGame> tempFriendList = new List<UserWhoPlaysMyGame>();
                    UserWhoPlaysMyGame tempFriend = null;

                    //We're going to store all the relationships in a list and search for each game later when we need it
                    if (readerFriends.HasRows)
                    {
                        while(readerFriends.Read())
                        {
                            tempFriend = new UserWhoPlaysMyGame();

                            //Store all the information
                            tempFriend.game = (string)readerFriends["frgame"];
                            tempFriend.user.nickname = (string)readerFriends["frname"];
                            tempFriend.user.privateProfile = (bool)readerFriends["privateProfile"];
                            tempFriend.user.online = (bool)readerFriends["online"];
                            tempFriend.user.admin = (bool)readerFriends["admin"];

                            tempFriendList.Add(tempFriend);
                        }
                    }

                    //Now we read each game the user plays
                    while (reader.Read())
                    {
                        //Create a new relationship
                        relationship = new UserGamesWithGameAndFriends();

                        //Read each relationship
                        relationship.relationship.game = (string)reader["game"];
                        relationship.relationship.hoursPlayed = (decimal)reader["hoursPlayed"];
                        relationship.relationship.acquisitionDate = (DateTime)reader["acquisitionDate"];
                        relationship.relationship.lastPlayed = reader["lastPlayed"] is DBNull ? null : (DateTime?)reader["lastPlayed"];
                        relationship.relationship.playing = (bool)reader["playing"];

                        //Read the game info
                        relationship.game.name = (string)reader["game"];
                        relationship.game.description = (string)reader["description"];
                        relationship.game.developer = (string)reader["developer"];
                        relationship.game.minimumAge = (int)reader["minimumAge"];
                        relationship.game.releaseDate = (DateTime)reader["releaseDate"];
                        relationship.game.frontPage = (bool)reader["frontPage"];
                        relationship.game.instructions = (string)reader["instructions"];
                        relationship.game.downloadUrl = (string)reader["downloadUrl"];
                        relationship.game.storeImageUrl = (string)reader["storeImageUrl"];
                        relationship.game.libraryImageUrl = (string)reader["libraryImageUrl"];

                        //Search all the users who play the same game and store it in a list
                        searchList = tempFriendList.FindAll(x => x.game == relationship.game.name);

                        foreach(UserWhoPlaysMyGame us in searchList)
                        {
                            relationship.friends.Add(us.user);
                        }
                        
                        list.Add(relationship);
                    }
                }

            }
            catch (SqlException ex) { list = null; }
            catch (Exception ex) { list = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
                readerFriends?.Close();
            }

            return list;
        }

    }
}
