using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 force = direction * _speed * Time.deltaTime;

        _rigidbody.MovePosition(transform.position + force);
    }
}
