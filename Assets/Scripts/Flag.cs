using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    Ray ray;
    private bool _isUp = false;
    private bool _isDown = false;
    private bool _playerNear = false;
    private Vector3 _downPos;

    // Start is called before the first frame update
    void Start()
    {
        _downPos = transform.position;
    }

    void CheckForBall()
    {
        Collider[] collision = Physics.OverlapSphere(transform.position, 1f);
        _playerNear = false;
        foreach (Collider collided in collision)
        {
            if (collided.GetComponent<Player>())
            {
                _playerNear = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForBall();
        _isUp = (transform.position == _downPos + Vector3.up / 2);
        _isDown = (transform.position == _downPos);

        if (_playerNear && !_isUp)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up / 10, 0.1f);
        }
        if (!_playerNear && !_isDown)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - Vector3.up / 10, 0.1f);
        }
    }
}
