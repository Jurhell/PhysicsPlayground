using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class RigidbodyController : MonoBehaviour
{
    [SerializeField, Header("Movement")]
    private float _speed;
    [SerializeField]
    private float _jumpForce;

    private float _topSpeed;
    private float _jumpCooldown = 1f;
    private float _smoothTime = 0.05f;
    private float _currentVelocity;
    private Vector2 _locomotionInput;
    private Vector3 _direction;
    private bool _jumpInput;
    private bool _readyToJump = true;
    private bool _isGrounded;
    private bool _ragdollToggle = false;

    private Rigidbody _rigidbody;

    [SerializeField, Header("Animation")]
    private Animator _unityChanAnimator;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _topSpeed = _speed;
        ToggleRagdoll(_ragdollToggle);
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.OverlapSphere(transform.position + new Vector3(0, -0.6f, 0), 0.45f).Length > 1;

        PlayerFacing();

        //Animation Section
        if (_unityChanAnimator == null)
            return;

        _unityChanAnimator.SetFloat("Speed", _speed);
        _unityChanAnimator.SetBool("Jump", _jumpInput);
        _unityChanAnimator.SetBool("Grounded", _isGrounded);
        _unityChanAnimator.SetBool("Rest", AtRest());
    }

    private void FixedUpdate()
    {
        if (_speed == 0f && _isGrounded)
            _rigidbody.velocity = Vector3.zero;

        Acceleration();
        Vector3 force = _direction.normalized * (_speed * 2) * Time.fixedDeltaTime;
        _rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    #region InputEvents
    public void OnMove(InputAction.CallbackContext context)
    {
        //Storing player input and direction when player moves
        _locomotionInput = context.action.ReadValue<Vector2>();
        _direction = new Vector3(_locomotionInput.x, 0f, _locomotionInput.y);
        _direction = transform.TransformDirection(_direction);
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

    public void OnReset(InputAction.CallbackContext context) => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void OnRagdoll(InputAction.CallbackContext context) => ToggleRagdoll(_ragdollToggle);
    #endregion

    //Rotates the player to face the direction they're moving
    private void PlayerFacing()
    {
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

    private void Acceleration()
    {
        //Checking if the player is moving
        if (_locomotionInput.x != 0 || _locomotionInput.y != 0)
        {
            _speed += 2.56f;

            //Capping Speed at Top Speed
            if (_speed > _topSpeed)
                _speed = _topSpeed;
        }
        else
            _speed -= 3.84f;

        if (_speed < 0)
            _speed = 0;
    }

    private bool AtRest()
    {
        if (_rigidbody.velocity == Vector3.zero)
            return true;
        else
            return false;   
    }

    public void ToggleRagdoll(bool enabled)
    {
        _rigidbody.isKinematic = enabled;
        TryGetComponent(out Collider collider);
        if (collider)
            collider.enabled = !enabled;

        foreach (Rigidbody item in GetComponentsInChildren<Rigidbody>(true))
            if (item != _rigidbody)
                item.isKinematic = !enabled; 
        foreach (Collider item in GetComponentsInChildren<Collider>(true))
            if (item != collider)
                item.enabled = enabled;

        _unityChanAnimator.enabled = !enabled;
        _ragdollToggle = !enabled;
    }

    private IEnumerator Wait(Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
}