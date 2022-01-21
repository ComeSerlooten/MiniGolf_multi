using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public PlayerColor _playerColor;
    private bool _isPressed;
    private bool _isChecking = false;
    public bool _isMoving;

    private Rigidbody _rigibody;
    private Vector3 _ballDir = new Vector3();
    private Vector3 _ballPreviousPosition = new Vector3();
    private Vector3 _ballNextPosition = new Vector3();

    private Transform _camera;
    private Vector3 _camForward;
    private Vector3 _camRight;

    public bool _isMyTurn = true;
    public bool hasWon = false;

    public bool outOfBounds = false;
    public Vector3 previousPos;

    LineRenderer liner;


    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody>();
        liner = GetComponentInChildren<LineRenderer>();
        _camera = GameObject.FindGameObjectWithTag("CamObj").transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        liner.SetPosition(0, transform.position);
        liner.SetPosition(1, transform.position);
        liner.material = GetComponent<Renderer>().material;
    }

    void Shooting()
    {
        if (!_isPressed)
        {
            _ballDir = Vector3.zero;
            liner.enabled = true;
        }


        _ballDir -= Input.GetAxis("Mouse Y") * _camForward / 10 + Input.GetAxis("Mouse X") * _camRight / 10;
    }

    private void Move()
    {
        Vector3 camPosHoriz = _camera.position;
        camPosHoriz.y = transform.position.y;
        _camForward = (transform.position - camPosHoriz).normalized;
        _camRight = Quaternion.Euler(0, 90, 0) * _camForward;

        liner.gameObject.transform.rotation = Quaternion.identity;

        if (Input.GetAxis("Shoot") == 1 && !_isMoving)
        {
            Shooting();
            _isPressed = true;
        }
        if (Input.GetAxis("Shoot") == 0 && _isPressed && !_isMoving)
        {
            previousPos = transform.position;
            _isMoving = true;
            _isMyTurn = false;
            _isPressed = false;
            liner.enabled = false;
            _rigibody.AddForce(_ballDir * 50, ForceMode.Impulse);
            _ballDir = new Vector3();
        }

        liner.SetPosition(0, transform.position);
        liner.SetPosition(1, transform.position + _ballDir * 0.75f);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Respawn")
        {
            outOfBounds = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Respawn")
        {
            outOfBounds = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isMoving && outOfBounds)
        {
            transform.position = previousPos;
            _rigibody.velocity = Vector3.zero;
            _rigibody.angularVelocity = Vector3.zero;
        }

        if (_isMyTurn && !hasWon) { Move(); }
        if (!_isChecking)
            StartCoroutine(CheckMovement());
        if (_isMoving)
            StartCoroutine(SendPosition());
        if (_isMyTurn && hasWon)
            GameManager.instance.NextTurn();


        if (_rigibody.velocity.magnitude < 0.02f && _isMoving)
        {
            _rigibody.velocity = Vector3.zero;
            _rigibody.angularVelocity = Vector3.zero;
            _isMoving = false;
            GameManager.instance.NextTurn();
        }
    }

    IEnumerator SendPosition()
    {
        while (_isMoving)
        {
            GameManager.instance.MoveBall(_playerColor, transform.position);
            yield return new WaitForSeconds(MainGame.instance._delayBetweenSend);
        }
    }
    IEnumerator CheckMovement()
    {
        _isChecking = true;
        _ballPreviousPosition = transform.position;
        yield return new WaitForSeconds(MainGame.instance._delayBetweenSend);
        _ballNextPosition = transform.position;
        if (_ballPreviousPosition == _ballNextPosition)
        {
            _isMoving = false;
            GameManager.instance.NextTurn();
        }

        _isChecking = false;
    }
}


