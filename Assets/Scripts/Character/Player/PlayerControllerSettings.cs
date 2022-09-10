using UnityEngine;

public class PlayerControllerSettings : ScriptableObject
{
    [Header("Move Variables")]
    public float mouseSensitivity;
    public float walkingSpeed;
    public float runningSpeed;
    public float jumpSpeed;
    public float gravity;
    public float lookXLimit;
    public float slopeSpeed;
}
