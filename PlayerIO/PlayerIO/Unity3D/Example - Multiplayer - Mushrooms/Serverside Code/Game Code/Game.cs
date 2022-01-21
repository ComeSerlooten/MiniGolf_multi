using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace MushroomsUnity3DExample
{
    public class Player : BasePlayer
    {

    }

    [RoomType("UnityMushrooms")]
    public class GameCode : Game<Player>
    {

        private int _playerNumber = 0;
        private int _compteurEndLevel;
        private int _turn = 0;
        // This method is called when an instance of your the game is created
        public override void GameStarted()
        {
            // anything you write to the Console will show up in the 
            // output window of the development server
            Console.WriteLine("Game is started: " + RoomId);





        }


        // This method is called when the last player leaves the room, and it's closed down.
        public override void GameClosed()
        {
            Console.WriteLine("RoomId: " + RoomId);
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(Player player)
        {
            _playerNumber++;
            player.Send("YouJoined", _playerNumber);
            foreach (Player pl in Players)
            {
                if (pl.ConnectUserId != player.ConnectUserId)
                    pl.Send("OtherJoined", _playerNumber);
            }
            



        }

        // This method is called when a player leaves the game
        public override void UserLeft(Player player)
        {

        }

        // This method is called when a player sends a message into the server code
        public override void GotMessage(Player player, Message message)
        {
            switch (message.Type)
            {
                case "MoveBall":
                    foreach (Player pl in Players)
                    {
                        if (pl.ConnectUserId != player.ConnectUserId)
                            pl.Send(message);
                    }
                    break;

                case "PlayerEndLevel":
                    _compteurEndLevel++;
                    if (_compteurEndLevel == _playerNumber || _compteurEndLevel == 4)
                    {
                        Broadcast("NextLevel");
                        _compteurEndLevel = 0;
                    }                        
                    break;
                case "NextTurn":
                    _turn++;
                    if (_turn > _playerNumber)
                        _turn %= _playerNumber;
                    else if (_turn > 4)
                        _turn %= 4;
                    if(_turn == 0)
                        _turn = 1;
                 
                    Broadcast("NextTurn", _turn);
                    break;
                case "StartGame":
                    _turn = 1;
                    Broadcast("NextTurn", _turn);
                    break;
            }
        }
    }
}