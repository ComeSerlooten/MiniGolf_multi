//-----------------------------------------------------------------------------------------------------------------
// 
//	Mushroom Example
//	Created by : Luis Filipe (filipe@seines.pt)
//	Dec 2010
//
//	Source code in this example is in the public domain.
//  The naruto character model in this demo is copyrighted by Ben Mathis.
//  See Assets/Models/naruto.txt for more details
//
//-----------------------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerIOClient;

public class ChatEntry {
	public string text = "";
	public bool mine = true;
}

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	private Connection pioconnection;
	private List<Message> msgList = new List<Message>(); //  Messsage queue implementation
	private bool joinedroom = false;

	// UI stuff
	private string infomsg = "";
    private void Awake()
    {
		instance = this;
    }
    void Start() {
		Application.runInBackground = true;

		// Create a random userid 
		System.Random random = new System.Random();
		string userid = "Guest" + random.Next(0, 10000);

		Debug.Log("Starting");

		PlayerIO.Authenticate(
			"mushroom-ddb5mf6ns0uk4y16h1ks2q",            //Your game id
			"public",                               //Your connection id
			new Dictionary<string, string> {        //Authentication arguments
				{ "userId", userid },
			},
			null,                                   //PlayerInsight segments
			delegate (Client client) {
				Debug.Log("Successfully connected to Player.IO");
				infomsg = "Successfully connected to Player.IO";

				Debug.Log("Create ServerEndpoint");
				// Comment out the line below to use the live servers instead of your development server
				client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

				Debug.Log("CreateJoinRoom");
				//Create or join the room 
				client.Multiplayer.CreateJoinRoom(
					"UnityDemoRoom",                    //Room id. If set to null a random roomid is used
					"UnityMushrooms",                   //The room type started on the server
					true,                               //Should the room be visible in the lobby?
					null,
					null,
					delegate (Connection connection) {
						Debug.Log("Joined Room.");
						infomsg = "Joined Room.";
						// We successfully joined a room so set up the message handler
						pioconnection = connection;
						pioconnection.OnMessage += handlemessage;
						joinedroom = true;
					},
					delegate (PlayerIOError error) {
						Debug.Log("Error Joining Room: " + error.ToString());
						infomsg = error.ToString();
					}
				);
			},
			delegate (PlayerIOError error) {
				Debug.Log("Error connecting: " + error.ToString());
				infomsg = error.ToString();
			}
		);

	}

	void handlemessage(object sender, Message m) {
		msgList.Add(m);
	}

	void FixedUpdate() {
		// process message queue
		foreach (Message m in msgList) {
			switch (m.Type) {
				case "YouJoined":
					if (m.GetInt(0) <= (int)PlayerColor.yellow)
						MainGame.instance.CreateMainPlayer((PlayerColor)m.GetInt(0));
					break;
				case "OtherJoined":
					if (m.GetInt(0) <= (int)PlayerColor.yellow)
						MainGame.instance.CreateOtherPlayer((PlayerColor)m.GetInt(0));
					break;
				case "MoveBall":
					if (m.GetInt(0) <= (int)PlayerColor.yellow)
					{
						Vector3 destination = new Vector3(m.GetFloat(1), m.GetFloat(2), m.GetFloat(3));
						MainGame.instance.MoveBall((PlayerColor)m.GetInt(0), destination);
					}
					break;
				case "NextTurn":
					MainGame.instance.NextTurn(m.GetInt(0));
					break;
				case "StartGame":
					MainGame.instance.StartGame(m.GetInt(0));
					break;
				case "NextLevel":
					MainGame.instance.NextLevel();
					break;

			}
		}

		// clear message queue after it's been processed
		msgList.Clear();
	}


	void OnGUI() {
		if (infomsg != "") {
			GUI.Label(new Rect(10, 180, Screen.width, 20), infomsg);
		}
	}

	public void MoveBall(PlayerColor playerColor,Vector3 ballPosition)
    {
		pioconnection.Send("MoveBall",(int)playerColor, ballPosition.x, ballPosition.y, ballPosition.z);
    }

	public void playerEndLevel()
    {
		pioconnection.Send("PlayerEndLevel");
    }

	public void NextTurn()
    {
		pioconnection.Send("NextTurn");
    }

	public void StartGame()
    {
		pioconnection.Send("StartGame");
    }
	
}
