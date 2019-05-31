using CHAIRAPI_Entities.Persistent;
using DAL.Conexion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CHAIRAPI_DAL.Handlers
{
    public class MessagesHandler
    {
        /// <summary>
        /// Method which will save the message in the database
        /// </summary>
        /// <param name="message">The message to be saved</param>
        /// <returns>True if saved successfully, false otherwise</returns>
        public static bool saveNewMessage(Message message)
        {
            Connection connection = new Connection();
            SqlConnection sqlConnection = new SqlConnection();
            SqlCommand command = new SqlCommand();
            bool savedSuccessfully;

            try
            {
                //Define parameters
                command.CommandText = "INSERT INTO Messages(text, sender, receiver) VALUES (@text, @sender, @receiver)";

                //Create parameters
                command.Parameters.Add("@text", SqlDbType.VarChar).Value = message.text;
                command.Parameters.Add("@sender", SqlDbType.VarChar).Value = message.sender;
                command.Parameters.Add("@receiver", SqlDbType.VarChar).Value = message.receiver;

                //Get connection
                sqlConnection = connection.getConnection();

                //Give the connection to the command
                command.Connection = sqlConnection;

                //Execute query
                savedSuccessfully = command.ExecuteNonQuery() == 1 ? true : false;
            }
            catch (SqlException ex)
            {
                savedSuccessfully = false; //Instead of throwing exception, change affectedRows to -1
            }
            catch (Exception ex)
            {
                savedSuccessfully = false; //Instead of throwing exception, change affectedRows to -1
            }
            finally
            {
                //Close connection
                connection.closeConnection(ref sqlConnection);
            }

            return savedSuccessfully;
        }

        /// <summary>
        /// Method which will search the database for the conversation between two users
        /// </summary>
        /// <param name="user1">One of the nicknames to search</param>
        /// <param name="user2">One of the nicknames to search</param>
        /// <returns>A list with all the messages if they're found, null otherwiser</returns>
        public static List<Message> getConversationBetweenTwoUsers(string user1, string user2)
        {
            //Variables
            SqlConnection sqlConnection = null;
            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand();
            Connection connection = new Connection();
            Message message = null;
            List<Message> list = new List<Message>();

            try
            {
                //Get open connection
                sqlConnection = connection.getConnection();

                //Define the command
                command.CommandText = "SELECT [text], sender, receiver, [date] FROM (SELECT TOP 100 ID, [text], sender, receiver, [date] FROM[Messages] WHERE sender = @user1 AND receiver = @user2 OR sender = @user2 AND receiver = @user1 ORDER BY ID DESC) AS Q ORDER BY Q.ID ASC";

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
                    while (reader.Read())
                    {
                        //Read the result and assign values
                        message = new Message();
                        message.text = (string)reader["text"];
                        message.sender = (string)reader["sender"];
                        message.receiver = (string)reader["receiver"];
                        message.date = (DateTime)reader["date"];
                        list.Add(message);
                    }
                }

            }
            catch (SqlException ex) { message = null; }
            catch (Exception ex) { message = null; }
            finally
            {
                connection.closeConnection(ref sqlConnection);
                reader?.Close();
            }

            return list;
        }
    }
}
