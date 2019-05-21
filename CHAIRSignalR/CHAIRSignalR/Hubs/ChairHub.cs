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
using CHAIRSignalR_Entidades.Complex;

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