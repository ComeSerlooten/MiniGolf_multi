using System;
using System.Collections.Generic;
using PlayerIOClient;

namespace Player.IO_Client_Example {
	class Program {
		static void Main(string[] args) {
			//---- Connecting to PlayerIO  --
			//-------------------------------
			var client = PlayerIO.Authenticate(
				"[Enter your game id here]",            //Your game id
				"public",                               //Your connection id
				new Dictionary<string, string> {        //Authentication arguments
					{ "userId", "MyUserName" },
				},
				null									//PlayerInsight segments
			);
			Console.WriteLine("Connected to PlayerIO as: " + client.ConnectUserId);

			//---- BigDB Example       -------
			//--------------------------------

			// load my player object from BigDB
			DatabaseObject myPlayerObject = client.BigDB.LoadMyPlayerObject();
			myPlayerObject.Set("awesome", true); // set properties
			myPlayerObject.Save(); // save changes


			//---- Multiplayer Example -------
			//--------------------------------

			// join a multiplayer room
			var connection = client.Multiplayer.CreateJoinRoom("my-room-id", "bounce", true, null, null);
			Console.WriteLine("Joined Multiplayer Room");

			// on message => print to console
			connection.OnMessage += delegate (object sender, PlayerIOClient.Message m) {
				Console.WriteLine(m.ToString());
			};

			// when disconnected => print reason
			connection.OnDisconnect += delegate (object sender, string reason) {
				Console.WriteLine("Disconnected, reason = " + reason);
			};

			Console.WriteLine(" - press enter to quit - ");
			Console.ReadLine();
		}
	}
}