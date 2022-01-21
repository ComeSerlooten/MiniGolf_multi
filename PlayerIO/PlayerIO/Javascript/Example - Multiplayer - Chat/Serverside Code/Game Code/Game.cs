using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using PlayerIO.GameLibrary;
using System.Drawing;

namespace Chat {
	public class Player : BasePlayer {
		public string Name;
	}

	[RoomType("Chat")]
	public class GameCode : Game<Player> {
		// This method is called when an instance of your the game is created
		public override void GameStarted() {
		}

		// This method is called when the last player leaves the room, and it's closed down.
		public override void GameClosed() {
		}

		// This method is called whenever a player joins the game
		public override void UserJoined(Player player) {
			player.Name = player.JoinData["name"];

			// Send join message for all users in the room to player
			foreach(var p in Players) {
				if(p.Id != player.Id) {
					player.Send("join", p.Id, p.Name);
				}
			}

			// Send a join message to everybody
			Message m = Message.Create("join");
			m.Add(player.Id);
			m.Add(player.Name);
			Broadcast(m);
		}

		// This method is called when a player leaves the game
		public override void UserLeft(Player player) {
			Broadcast("left", player.Id);
		}

		// This method is called when a player sends a message into the server code
		public override void GotMessage(Player player, Message message) {
			switch(message.Type) {
				case "msg":
					Broadcast("msg", player.Id, message.GetString(0));
					break;
			}
		}
	}
}