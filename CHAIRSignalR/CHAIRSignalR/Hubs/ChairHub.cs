using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using CHAIRSignalR_DAL;
using CHAIRSignalR_Entities.Persistent;
using System.Net;
using CHAIRSignalR_Entities.Complex;
using CHAIRSignalR_Entities.Responses;
using System.Threading;
using CHAIRSignalR.Common;
using CHAIRSignalR_DAL.Calls;
using CHAIRSignalR_Entities.Enums;

namespace CHAIRSignalR.Hubs
{
    public class ChairHub : Hub
    {
        /// <summary>
        /// Method used to retrieve all the games available in the store
        /// </summary>
        /// <param name="token"></param>
        public void getAllStoreGames(string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            List<Game> games = GameCallback.getAllStoreGames(token, out statusCode);

            if (games != null)
                Clients.Caller.getAllStoreGames(games);
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to fetch the store games. Please try again when it's fixed :D");
        }

        /// <summary>
        /// Method used to retrieve all the games a user plays, along with the information about each game and which friends play them
        /// </summary>
        /// <param name="token">The user's token</param>
        public void getAllMyGamesAndFriends(string token)
        {
            //Get the user's nickname
            string nickname = ChairInfo.onlineUsers.Single(x => x.Value == Context.ConnectionId).Key;

            //Make the call to the API
            HttpStatusCode statusCode;
            List<UserGamesWithGameAndFriends> games = UserGamesCallback.getAllMyGames(nickname, token, out statusCode);

            if (games != null)
                Clients.Caller.getAllMyGames(games);
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to fetch your games. Please try again when it's fixed :D");
        }

        public void getUserProfile(string nickname, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            UserProfile games = UserProfileCallback.getUserProfile(nickname, token, out statusCode);

            if (games != null)
                Clients.Caller.getUserProfile(games);
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to fetch this user's profile. Please try again when it's fixed :D");
        }

        public void getGameInformation(string nickname, string game, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            GameStore gameStore = GameStoreCallback.getGameAndRelationship(nickname, game, token, out statusCode);

            if (gameStore.relationship.game == null || gameStore.relationship.user == null)
                gameStore.relationship = null;

            if (gameStore != null)
                Clients.Caller.getGameInformation(gameStore);
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to fetch the game information. Please try again when it's fixed :D");
        }

        public void searchForUsers(string search, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            List<UserSearch> users = UserSearchCallback.getSearchResults(search, token, out statusCode);

            if (users != null)
                Clients.Caller.searchForUsers(users);
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to search for users. Please try again when it's fixed :D");

        }

        public void addFriend(string user1, string user2, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            UserFriendsCallback.saveNewRelationship(user1, user2, token, out statusCode);

            if(statusCode != HttpStatusCode.Created)
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to add a friend. Please try again when it's fixed :D");
        }

        public void acceptFriendship(string myself, string friend, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            UserFriendsCallback.acceptFriendship(myself, friend, token, out statusCode);

            if (statusCode == HttpStatusCode.NoContent)
            {
                string conId;
                if (ChairInfo.onlineUsers.TryGetValue(friend, out conId))
                {
                    Clients.Client(conId).updateFriendListWithNotification($"{myself} and {friend} are now friends!", NotificationType.ONLINE); //Tell our new online friend we accepted their request
                    Clients.Caller.updateFriendListWithNotification($"{myself} and {friend} are now friends!", NotificationType.ONLINE); //Update our friendlist and show notification
                }
                else
                    Clients.Caller.updateFriendListWithNotification($"{myself} and {friend} are now friends!", NotificationType.OFFLINE); //Update our friendlist and show notification

            }
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to accept a friendship. Please try again when it's fixed :D");
        }

        public void endFriendship(string myself, string friend, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            UserFriendsCallback.endFriendship(myself, friend, token, out statusCode);

            if (statusCode == HttpStatusCode.NoContent)
            {
                string conId;
                if (ChairInfo.onlineUsers.TryGetValue(friend, out conId))
                    Clients.Client(conId).updateFriendListWithNotification("", NotificationType.GENERIC); //We don't tell them who deleted them, we just want them to update their friend list
            }
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to end a friendship. Please try again when it's fixed :D");
        }

        public void getFriends(string nickname, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            List<UserForFriendList> list = UserForFriendListCallback.getFriends(nickname, token, out statusCode);

            if (statusCode == HttpStatusCode.OK)
                Clients.Caller.getFriends(list);
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to get your friends. Please try again when it's fixed :D");
        }

        public void goOnline(string nickname, string token, List<string> usersToNotify)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            UserCallback.online(nickname, token, out statusCode);

            //If it all went well, then we notify the online user's friends that he connected
            if (statusCode == HttpStatusCode.NoContent)
            {
                string conId;
                foreach(string user in usersToNotify)
                {
                    if(ChairInfo.onlineUsers.TryGetValue(user, out conId))
                        Clients.Client(conId).updateFriendListWithNotification($"{nickname} just got online!", NotificationType.ONLINE);
                }

                Clients.Caller.onlineSuccessful();
            }
            else
                Clients.Caller.unexpectedError("An unexpected error occurred when trying to set you online. Please try again when it's fixed :D");
        }

        public void goOffline(string nickname, string token, List<string> usersToNotify)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            UserCallback.offline(nickname, token, out statusCode);

            //If it all went well, then we notify the online user's friends that he disconnected
            if (statusCode == HttpStatusCode.NoContent)
            {
                string conId;
                foreach(string user in usersToNotify)
                {
                    if(ChairInfo.onlineUsers.TryGetValue(user, out conId))
                        Clients.Client(conId).updateFriendListWithNotification($"{nickname} had enough for today", NotificationType.OFFLINE);
                }
            }
        }

        public void startPlayingGame(string nickname, string game, string token, List<string> usersToNotify)
        {
            //We add the user to the usersPlaying liist 
            ChairInfo.usersPlaying.AddOrUpdate(nickname, new KeyValuePair<string, DateTime>(game, DateTime.Now), (key, value) => value);
            
            //Make the call to the API
            HttpStatusCode statusCode;
            UserGamesCallback.setPlayingTrue(nickname, game, token, out statusCode);

            //If it all went well, then we notify the online user's friends that he disconnected
            if (statusCode == HttpStatusCode.NoContent)
            {
                string conId;
                foreach(string user in usersToNotify)
                {
                    if(ChairInfo.onlineUsers.TryGetValue(user, out conId))
                        Clients.Client(conId).updateFriendListWithNotification($"{user} is now playing {game}", NotificationType.GENERIC);
                }
            }
        }

        public void stopPlayingGame(string nickname, string token, List<string> usersToNotify)
        {
            //We add the user to the usersPlaying list
            KeyValuePair<string, DateTime> game;
            ChairInfo.usersPlaying.TryRemove(nickname, out game);
            
            if(!game.Equals(default(KeyValuePair<string, DateTime>)))
            {
                //Calculate how many seconds the user has played
                int secondsToAdd = (int)(DateTime.Now - game.Value).TotalSeconds;

                //Make the call to the API
                HttpStatusCode statusCode;
                UserGamesCallback.setPlayingFalse(nickname, game.Key, secondsToAdd, token, out statusCode);

                //If it all went well, then we notify the online user's friends that he disconnected
                if (statusCode == HttpStatusCode.NoContent)
                {
                    string conId;
                    foreach(string user in usersToNotify)
                    {
                        if(ChairInfo.onlineUsers.TryGetValue(user, out conId))
                            Clients.Client(conId).updateFriendListWithNotification($"{user} stopped playing {game.Key}", NotificationType.GENERIC);
                    }

                    Clients.Caller.closedGameSuccessfully();
                }
                else
                    Clients.Caller.unexpectedError($"An unexpected error occurred when trying to stop you from playing that game. Please try again when it's fixed :D");

            }
            else
                Clients.Caller.unexpectedError($"An unexpected error ocurred trying to find which game you were playing. Please try again when it's fixed :D");
        }

        public void getConversation(string me, string friend, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            List<Message> messages = MessageCallback.getConversation(me, friend, token, out statusCode);

            //If it all went well, then we notify the online user's friends that he disconnected
            if (statusCode == HttpStatusCode.OK)
                Clients.Caller.getConversation(friend, messages);
            else
                Clients.Caller.unexpectedError($"An unexpected error occurred when trying to fetch your messages with {friend}. Please try again when it's fixed :D");

        }

        public void sendMessage(Message message, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            MessageCallback.postMessage(message, token, out statusCode);

            //If it all went well, then we notify the online user's friends that he disconnected
            if (statusCode == HttpStatusCode.Created)
            {
                string conId;
                bool receiverIsOnline = ChairInfo.onlineUsers.TryGetValue(message.receiver, out conId);

                if (receiverIsOnline)
                    Clients.Client(conId).receiveMessage(message);
            }
            else
                Clients.Caller.unexpectedError($"An unexpected error occurred when trying to send a message to {message.receiver}. Please try again when it's fixed :D");
        }

        public void buyGame(UserGames relationship, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            UserGamesCallback.buyGame(relationship, token, out statusCode);

            //If it all went well, then we notify the online user's friends that he disconnected
            if (statusCode == HttpStatusCode.Created)
                Clients.Caller.gameBought(relationship.game);
            else
                Clients.Caller.unexpectedError($"An unexpected error occurred when trying to buy {relationship.game}. Please try again when it's fixed :D");
        }

        #region Admin
        public void adminGetOnlineUsers()
        {
            //Get all the nicknames of the online users
            List<string> onlineUsersNicknames = new List<string>();
            foreach(string user in ChairInfo.onlineUsers.Keys)
                onlineUsersNicknames.Add(user);
            
            //Return all the nicknames of the online users to the caller
            Clients.Caller.adminGetOnlineUsers(onlineUsersNicknames);
        }

        public void adminBanUser(string nickname, string reason, string token, DateTime until)
        {
            User user = new User();
            user.nickname = nickname;
            user.banReason = reason;
            user.bannedUntil = until;

            //Make the call to the API
            HttpStatusCode statusCode;
            AdminCallback.ban(user, token, out statusCode);

            if (statusCode == HttpStatusCode.NoContent)
            {
                string conId;
                if(ChairInfo.onlineUsers.TryGetValue(nickname, out conId))
                    Clients.Client(conId).youHaveBeenBanned(new BanResponse(BannedByEnum.USER, reason, until));
            }
            else
                Clients.Caller.unexpectedError($"An unexpected error occurred when trying to ban {nickname}. Please try again when it's fixed :D");
        }

        public void adminPardonBan(string nickname, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            AdminCallback.pardon(nickname, token, out statusCode);

            if (statusCode != HttpStatusCode.NoContent)
                Clients.Caller.unexpectedError($"An unexpected error occurred when trying to pardon {nickname}. Please try again when it's fixed :D");
        }

        public void adminChangeFrontPageGame(string gameName, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            AdminCallback.changeFrontPageGame(gameName, token, out statusCode);

            if (statusCode != HttpStatusCode.NoContent)
                Clients.Caller.unexpectedError($"An unexpected error occurred when trying to change the front page game to {gameName}. Please try again when it's fixed :D");
        }

        public void addGameToStore(Game game, string token)
        {
            //Make the call to the API
            HttpStatusCode statusCode;
            AdminCallback.addNewGame(game, token, out statusCode);

            if (statusCode != HttpStatusCode.NoContent)
                Clients.Caller.unexpectedError($"An unexpected error occurred when trying to add the new game {game.name}. Please try again when it's fixed :D");
        }
        #endregion

        public override Task OnDisconnected(bool stopCalled)
        {
            //Remove the user from the online list
            string nickname = ChairInfo.onlineUsers.Single(x => x.Value == Context.ConnectionId).Key;
            string conId;
            ChairInfo.onlineUsers.TryRemove(nickname, out conId);

            return base.OnDisconnected(stopCalled);
        }
        
        public override Task OnConnected()
        {
            //Add the user from the online list
            string nickname = Context.QueryString["nickname"];
            ChairInfo.onlineUsers.AddOrUpdate(nickname, Context.ConnectionId, (key, value) => value);

            return base.OnConnected();
        }
    }
}