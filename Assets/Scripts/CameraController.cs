using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    private Vector3 _camOffset;
    [SerializeField] private float _camDistance = 3;
    [Range(0.1f,5f)] [SerializeField] private float _sensitivity = 2;
    [SerializeField] private Vector3 _startingPos;

    private float xInput = 0;
    private float yInput = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.position + _startingPos;
        _camOffset = transform.position - target.position;
        _camOffset = _camOffset.normalized * _camDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            if (Input.GetMouseButton(1))
            {
                xInput = Input.GetAxis("Mouse X") * _sensitivity;
                yInput = Input.GetAxis("Mouse Y") * _sensitivity;

            }
            _camDistance -= Input.GetAxis("Mouse ScrollWheel");
            _camDistance = Mathf.Clamp(_camDistance, 1, 5);

            _camOffset = Quaternion.Euler(yInput, xInput, 0) * _camOffset;
            _camOffset = _camOffset.normalized * _camDistance;



            transform.LookAt(target);
            transform.position = Vector3.Lerp(transform.position, target.position + _camOffset, 0.1f);
        }
        
    }
}
