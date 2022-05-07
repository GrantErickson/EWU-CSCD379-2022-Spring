using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace Wordle.Api.Hubs
{
    [SignalRHub]
    public class GameHub : Hub
    {
        public async Task NewCharacter(string username, string character, string roomName)
        {
            //await Clients.All.SendAsync("characterReceived", username, character);
            await Clients.Group(roomName).SendAsync("CharacterReceived", username, character);
        }

        public async Task JoinGame(string username, string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserJoined", username);
        }

        public async Task LeaveGame(string username, string roomName)
        {
            await Clients.Group(roomName).SendAsync("UserLeft", username);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}
