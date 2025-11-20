using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;


    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public bool isSprinting;

    private MovementControls movementControls;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        movementControls = new MovementControls();

    }

    private void OnEnable()
    {
        movementControls.Enable();
    }

    private void OnDisbale()
    {
        movementControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return movementControls.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return movementControls.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame()
    {
        return movementControls.Player.Jump.triggered;
    }

    public bool FirstPersonCameraTriggered()
    {
        return movementControls.Player.FirstPersonCamera.triggered;
    }

    public bool ThirdPersonCameraTriggered()
    {
        return movementControls.Player.ThirdPersonCamera.triggered;
    }

    public bool SprintStarting()
    {
        return movementControls.Player.SprintStart.triggered;
    }

    public bool SprintFinishing()
    {
        return movementControls.Player.SprintFinish.triggered;
    }

    public void SprintPressed()
    {
        isSprinting = true;
    }

    public void SprintReleased()
    {
        isSprinting = false;
    }




}
