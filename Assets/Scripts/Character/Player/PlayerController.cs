using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move Restrictions")]
    public bool cantJump;
    public bool cantSprint;
    public bool cantMove;
    public bool isCrouching;

    [SerializeField] PlayerControllerSettings _controllerSettings;

    private CharacterController _characterController;
    private Camera _playerCamera;
    private Vector3 _moveDirection;
    private float _rotationX;

    //For slopes
    private Vector3 _hitNormal;
    private bool _isSliding;

    //For waer
    private bool _inWater;

    private void Awake()
    {
        GameManager.instance.playerObject = this.gameObject;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        _playerCamera = Camera.main;

        //Plays the game on player start//Move to level load
        GameManager.instance.UnPauseGame(true);
    }

    private void Update()
    {
        ToggleCrouch();
        MovePlayer();
    }

    public void StopMovement()
    {
        cantJump = true;
        cantMove = true;
    }

    public void StartMovement()
    {
        cantJump = false;
        cantMove = false;
    }

    private void ToggleCrouch()
    {
        //Press left ctrl to crouch 
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
        }
    }

    private void MovePlayer()
    {
        Vector3 forward = _characterController.transform.TransformDirection(Vector3.forward);
        Vector3 right = _characterController.transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetButton("Sprint");

        if (cantSprint)
        {
            isRunning = false;
        }


        if (isCrouching || _inWater)
        {
            isRunning = false;
            _characterController.height = 1f;
        }
        else
        {
            _characterController.height = 2f;
        }

        float curSpeedX = !cantMove ? (isRunning ? _controllerSettings.runningSpeed : _controllerSettings.walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = !cantMove ? (isRunning ? _controllerSettings.runningSpeed : _controllerSettings.walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = _moveDirection.y;

        if (isCrouching)
        {
            curSpeedX = curSpeedX / 2;
            curSpeedY = curSpeedY / 2;
        }

        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && !cantMove && _characterController.isGrounded && !cantJump)
        {
            _moveDirection.y = _controllerSettings.jumpSpeed;
        }
        else
        {
            _moveDirection.y = movementDirectionY;
        }

        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= _controllerSettings.gravity * Time.deltaTime;
        }

        //Checks slide
        if (!cantMove && CheckSlide())
        {
            _moveDirection += new Vector3(_hitNormal.x, -_hitNormal.y, _hitNormal.z)* _controllerSettings.slopeSpeed;
        }

        // Move the controller
        _characterController.Move(_moveDirection * Time.deltaTime);


        // Player and Camera rotation
        if (!cantMove)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * _controllerSettings.mouseSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -_controllerSettings.lookXLimit, _controllerSettings.lookXLimit);
            _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            _characterController.gameObject.transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _controllerSettings.mouseSensitivity, 0);
        }
    }

    private bool CheckSlide()
    {
        if (_characterController.isGrounded && Physics.Raycast(_characterController.transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
        {
            _hitNormal = slopeHit.normal;

            if (slopeHit.collider.tag == "Water")
            {
                _inWater = true;
            }
            else
            {
                _inWater = false;
            }

            return (Vector3.Angle(_hitNormal, Vector3.up) > _characterController.slopeLimit);
        }
        else
        {
            return false;
        }
    }
}
