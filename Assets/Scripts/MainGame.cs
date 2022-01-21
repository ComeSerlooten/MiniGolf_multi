using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerColor
{
    blue = 1,
    red,
    green,
    yellow
}
public class MainGame : MonoBehaviour
{
    public static MainGame instance;
    public float _delayBetweenSend = 0.025f;
    private Player _myPlayer;
    private PlayerColor _currentPlayerColor;
    private Vector3 _startPosition;
    private Vector3 _destination;
    private float _step = 0f;
    private float _time = 0f;
    private bool _isMovingaBall = false;
    private int _levelNumber = 0;
    private GameObject _gameButton;
    [SerializeField]
    private GameObject _blueBall;
    [SerializeField]
    private GameObject _redBall;
    [SerializeField]
    private GameObject _greenBall;
    [SerializeField]
    private GameObject _yellowBall;
    [SerializeField]
    private GameObject _startGame;
    [SerializeField] GameObject cameraSphere;

    private List<GameObject> players = new List<GameObject>();
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_isMovingaBall && _time < _delayBetweenSend)
        {
            players[(int)_currentPlayerColor - 1].transform.position = Vector3.MoveTowards(players[(int)_currentPlayerColor - 1].transform.position,
            _destination, _step * Time.deltaTime);
            _time += Time.deltaTime;
        }
        if (_time > _delayBetweenSend)
            _isMovingaBall = false;        
    }

    public void CreateMainPlayer(PlayerColor playercolor)
    {
        for(PlayerColor color = PlayerColor.blue; color <= playercolor; color++)
        {
            if (color == playercolor)
            {
                if (color == PlayerColor.blue)
                    _gameButton = Instantiate(_startGame, new Vector3(0, 0, 0), Quaternion.identity);
                GameObject player = null;
                if (playercolor == PlayerColor.blue)
                    player = Instantiate(_blueBall, LevelDesigner.instance.startPositions[_levelNumber].position, Quaternion.identity);
                else if (playercolor == PlayerColor.red)
                    player = Instantiate(_redBall, LevelDesigner.instance.startPositions[_levelNumber].position, Quaternion.identity);
                else if (playercolor == PlayerColor.green)
                    player = Instantiate(_greenBall, LevelDesigner.instance.startPositions[_levelNumber].position, Quaternion.identity);
                else if (playercolor == PlayerColor.yellow)
                    player = Instantiate(_yellowBall, LevelDesigner.instance.startPositions[_levelNumber].position, Quaternion.identity);
                cameraSphere.GetComponent<CameraController>().enabled = true;
                cameraSphere.GetComponent<CameraController>().target = player.transform;
                _myPlayer = player.GetComponent<Player>();
                players.Add(player);
                break;
            }
            else
                CreateOtherPlayer(color);
        }
        
    }

    public void CreateOtherPlayer(PlayerColor playercolor)
    {
        GameObject otherPlayer = null;
        if (playercolor == PlayerColor.blue)
        {
            otherPlayer = Instantiate(_blueBall, LevelDesigner.instance.startPositions[_levelNumber].position, Quaternion.identity);
        }
        else if (playercolor == PlayerColor.red)
        {
            otherPlayer = Instantiate(_redBall, LevelDesigner.instance.startPositions[_levelNumber].position, Quaternion.identity);
        }          
        else if (playercolor == PlayerColor.green)
        {
            otherPlayer = Instantiate(_greenBall, LevelDesigner.instance.startPositions[_levelNumber].position, Quaternion.identity);
        }
        else if (playercolor == PlayerColor.yellow)
        {
            otherPlayer = Instantiate(_yellowBall, LevelDesigner.instance.startPositions[_levelNumber].position, Quaternion.identity);
        }
        if (otherPlayer != null)
        {
            
            otherPlayer.GetComponent<Player>().enabled = false;
            otherPlayer.GetComponent<Rigidbody>().isKinematic = true;
            otherPlayer.GetComponent<Collider>().enabled = false;
            players.Add(otherPlayer);
        }

    }

    public void MoveBall(PlayerColor playerColor, Vector3 destination)
    {
        _currentPlayerColor = playerColor;
        _startPosition = players[(int)playerColor - 1].transform.position;
        _step = (destination - _startPosition).magnitude;
        _destination = destination;
        _isMovingaBall = true;
        _time = 0;
    }

    public void NextTurn(int turn)
    {
        Debug.Log(turn);
        if (turn == (int)_myPlayer._playerColor)
            _myPlayer._isMyTurn = true;        
    }

    public void StartGame(int turn)
    {
        if (turn == (int)_myPlayer._playerColor)
            _myPlayer._isMyTurn = true;
    }

    public void NextLevel()
    {

        _levelNumber++;
        if(_levelNumber < LevelDesigner.instance.startPositions.Length)
            foreach(GameObject player in players)
            {
                player.transform.position = LevelDesigner.instance.startPositions[_levelNumber].position;
                player.GetComponent<Player>().hasWon = false;
            }
    }


}
