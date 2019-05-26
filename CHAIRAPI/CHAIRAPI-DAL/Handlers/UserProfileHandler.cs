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
    public class UserProfileHandler
    {
        /// <summary>
        /// Method which will search the database for the user with the specified nickname and fetch all its information as well as
        /// all the information about the last 3 games he has played
        /// </summary>
        /// <param name="nickname">The nickname of the user to be searched</param>
        /// <returns>The user with all its information if it was found, false otherwise</returns>
        public static UserProfile searchUserProfileInformation(string nickname)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlDataReader readerGames = null;
            SqlCommand command = new SqlCommand();
            SqlCommand commandGames = new SqlCommand();
            Connection connection = new Connection();
            UserProfile user = new UserProfile();


            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT nickname, profileDescription, profileLocation, birthDate, privateProfile, accountCreationDate, [online], admin, bannedUntil, banReason FROM Users WHERE nickname = @nickname";

                //Set the parameter
                command.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;

                //Define the connection
                command.Connection = sqlConnection;
                commandGames.Connection = sqlConnection;

                //Execute
                reader = command.ExecuteReader();

                //Check if the user exists
                if (reader.HasRows)
                {
                    commandGames.CommandText = "SELECT TOP 3 UG.game, UG.hoursPlayed, UG.acquisitionDate, UG.lastPlayed, UG.playing, G.developer, G.libraryImageUrl FROM UserGames AS UG INNER JOIN Games AS G ON UG.game = G.name WHERE UG.[user] = @nickname AND lastPlayed IS NOT NULL ORDER BY UG.playing desc, UG.lastPlayed desc";
                    commandGames.Parameters.Add("@nickname", SqlDbType.VarChar).Value = nickname;
                    readerGames = commandGames.ExecuteReader();

                    //Read the result and assign values
                    reader.Read();
                    user = new UserProfile();
                    user.user.nickname = (string)reader["nickname"];
                    user.user.profileDescription = (string)reader["profileDescription"];
                    user.user.profileLocation = (string)reader["profileLocation"];
                    user.user.birthDate = (DateTime)reader["birthDate"];
                    user.user.privateProfile = (bool)reader["privateProfile"];
                    user.user.accountCreationDate = (DateTime)reader["accountCreationDate"];
                    user.user.online = (bool)reader["online"];
                    user.user.admin = (bool)reader["admin"];
                    user.user.bannedUntil = reader["bannedUntil"] is DBNull ? null : (DateTime?)reader["bannedUntil"];
                    user.user.banReason = reader["banReason"] is DBNull ? "" : (string)reader["banReason"];

                    if(readerGames.HasRows)
                    {
                        UserGames tempRel;
                        Game tempGame;

                        while(readerGames.Read())
                        {
                            tempRel = new UserGames();
                            tempGame = new Game();

                            //Information about the user's playing habits
                            tempRel.game = (string)readerGames["game"];
                            tempRel.hoursPlayed = (decimal)readerGames["hoursPlayed"];
                            tempRel.acquisitionDate = (DateTime)readerGames["acquisitionDate"];
                            tempRel.lastPlayed = readerGames["lastPlayed"] is DBNull ? null : (DateTime?)readerGames["lastPlayed"];
                            tempRel.playing = (bool)readerGames["playing"];

                            //Information about the game
                            tempGame.name = (string)readerGames["game"];
                            tempGame.developer = (string)readerGames["developer"];
                            tempGame.libraryImageUrl = (string)readerGames["libraryImageUrl"];

                            user.games.Add(new UserGamesWithGame(tempRel, tempGame));
                        }
                    }
                }

            }
            catch (SqlException ex) { user = null; }
            catch (Exception ex) { user = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
                readerGames?.Close();
            }

            return user;
        }
    }
}
