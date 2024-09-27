using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class RigidbodyController : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _jumpForce;

    private float _jumpCooldown = 1f;
    private float _smoothTime = 0.05f;
    private float _currentVelocity;
    private Vector2 _locomotionInput;
    private Vector3 _direction;
    private bool _jumpInput;
    private bool _readyToJump = true;
    private bool _isGrounded;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.OverlapSphere(transform.position + new Vector3(0, -0.6f, 0), 0.45f).Length > 1;

        //Guard that makes player continue facing previous direction
        if (_locomotionInput.sqrMagnitude == 0)
            return;

        //Storing the target angle as radians converted to degrees
        float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        //Smoothing player rotation
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, _smoothTime);
        //Rotating player
        _rigidbody.MoveRotation(Quaternion.Euler(0f, angle, 0f));
    }

    private void FixedUpdate()
    { 
        Vector3 force = _direction.normalized * (_speed * 2) * Time.fixedDeltaTime;
        _rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Storing player input and direction when player moves
        _locomotionInput = context.action.ReadValue<Vector2>();
        _direction = new Vector3(_locomotionInput.x, 0f, _locomotionInput.y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded && _readyToJump)
        {
            _readyToJump = false;

            //Resetting y velocity
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);

            //Waiting until cooldown time has elapsed before allowing another jump
            StartCoroutine(Wait(() => { _readyToJump = true; _jumpInput = false; }, _jumpCooldown));
            //Storing that the player has jumped
            _jumpInput = context.action.ReadValue<float>() > 0;
        }
    }

    private IEnumerator Wait(Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
}
