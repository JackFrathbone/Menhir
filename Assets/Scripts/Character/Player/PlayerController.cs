using UnityEngine;
using FMOD.Studio;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Move Restrictions")]
    public bool cantJump;
    public bool cantSprint;
    public bool cantMove;
    public bool isCrouching;

    [Header("Data")]
    private bool _underKnockback;

    [Header("References")]
    [SerializeField] PlayerControllerSettings _controllerSettings;

    private CharacterController _characterController;
    private Camera _playerCamera;
    private Vector3 _moveDirection;
    private float _rotationX;

    private bool isRunning = false;

    //For slopes
    private Vector3 _hitNormal;

    //For water
    private bool _inWater;
    private float _originalSpeed;

    private PlayerCharacterManager _playerCharacterManager;

    //For audio
    private EventInstance _playerFootstep;


    private void Awake()
    {
        GameManager.instance.playerObject = this.gameObject;
    }

    private void Start()
    {
        _playerCharacterManager = GetComponent<PlayerCharacterManager>();
        _characterController = GetComponent<CharacterController>();

        _playerCamera = Camera.main;

        //Plays the game on player start//Move to level load
        GameManager.instance.UnPauseGame(true);

        _originalSpeed = _controllerSettings.walkingSpeed;

        _playerFootstep = AudioManager.instance.CreateInstance("event:/Footsteps");
    }

    private void Update()
    {
        ToggleCrouch();
        MovePlayer();
        UpdateSound();
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

    public void SlowMovement()
    {
        if (_controllerSettings.walkingSpeed == _originalSpeed)
        {
            cantSprint = true;
            _controllerSettings.walkingSpeed = 2f;
        }
    }

    //To return to normal  after being slowed
    public void NormalMovement()
    {
        cantSprint = false;
        _controllerSettings.walkingSpeed = _originalSpeed;
    }

    private void ToggleCrouch()
    {
        //Press left ctrl to crouch 
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;
            _playerCharacterManager.isCrouching = isCrouching;
        }
    }

    private void MovePlayer()
    {
        Vector3 forward = _characterController.transform.TransformDirection(Vector3.forward);
        Vector3 right = _characterController.transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        isRunning = Input.GetButton("Sprint");

        if (cantSprint)
        {
            isRunning = false;
        }


        if (isCrouching || _inWater)
        {
            isRunning = false;
            _characterController.height = 1f;

            if (_inWater)
            {
                _playerCharacterManager.DamageStamina(5f * Time.deltaTime);

                if(_playerCharacterManager.staminaCurrent <= 0)
                {
                    _playerCharacterManager.DamageHealth(1, null);
                }
            }
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
            curSpeedX /= 2;
            curSpeedY /= 2;
        }

        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && !cantMove && _characterController.isGrounded && !cantJump && _playerCharacterManager.staminaCurrent >= 5f)
        {
            _moveDirection.y = _controllerSettings.jumpSpeed;
            _playerCharacterManager.DamageStamina(5f);
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
            _moveDirection += new Vector3(_hitNormal.x, -_hitNormal.y, _hitNormal.z) * _controllerSettings.slopeSpeed;
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

        if (_characterController.velocity != Vector3.zero)
        {

        }

        //If player is under the effect of knockback
        if (_underKnockback)
        {
            _characterController.Move((-forward * 4) * Time.deltaTime);
        }
    }

    public void StartKnockback(float amount)
    {
        StopCoroutine(ApplyKnockback(0f));
        StartCoroutine(ApplyKnockback(amount));
    }


    private bool CheckSlide()
    {
        if (_characterController.isGrounded && Physics.Raycast(_characterController.transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
        {
            _hitNormal = slopeHit.normal;

            if (slopeHit.collider.CompareTag("Water"))
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

    private void UpdateSound()
    {
        if (_characterController.velocity.x != 0 && _characterController.isGrounded)
        {
            _playerFootstep.getPlaybackState(out PLAYBACK_STATE playbackstate);

            if (playbackstate.Equals(PLAYBACK_STATE.STOPPED))
            {
                _playerFootstep.start();
            }

            //If in water change the surface type
            if (_inWater)
            {
                _playerFootstep.setParameterByName("SurfaceType", 1);
            }
            else
            {
                _playerFootstep.setParameterByName("SurfaceType", 0);
            }
        }
        else
        {
            _playerFootstep.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDestroy()
    {
        _playerFootstep.stop(STOP_MODE.IMMEDIATE);
        _playerFootstep.release();
    }

    IEnumerator ApplyKnockback(float knockbackAmount)
    {
        _underKnockback = true;
        yield return new WaitForSeconds(knockbackAmount/2);
        _underKnockback = false;
    }
}
