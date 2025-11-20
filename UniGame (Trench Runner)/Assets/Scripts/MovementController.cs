using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Cinemachine;

namespace Runner.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour
    {
        private CharacterController controller;
        private Vector3 playerVelocity;
        public bool groundedPlayer;
        private InputManager inputManager;
        private Transform cameraTransform;
        private MovementControls movementControls;

        [SerializeField]
        public float playerSpeed = 2.0f;
        [SerializeField]
        public float playerRunning = 6.0f;
        [SerializeField]
        private float jumpHeight = 1.0f;
        [SerializeField]
        private float gravityValue = -9.81f;
        [SerializeField]
        private Animator animator;

        public CinemachineVirtualCamera ThirdPerson;
        public CinemachineVirtualCamera FirstPerson;

        private CinemachineBrain _cinemachineBrain;
        public bool isSprinting;

        //Need to fix this to new input system
        public Rigidbody playerRigid;
        [SerializeField]
        public float ro_speed;
        public bool isWalking;
        public Transform playerTrans;

        //Old InputAction Stuff

        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private LayerMask turnLayer;
        [SerializeField]
        private LayerMask obstacleLayer;
        [SerializeField]
        private UnityEvent<Vector3> turnEvent;
        [SerializeField]
        private UnityEvent<int> gameOverEvent;
        [SerializeField]
        private UnityEvent<int> scoreUpdateEvent;
        [SerializeField]
        private float scoreMultiplier = 10f;


        private PlayerInput playerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;

        private bool sliding = false;
        private float score = 0;

       // private void Awake()
       // {
            //turnAction = playerInput.actions["Turn"];
            //turnAction.performed += PlayerTurn;
            //turnAction.performed -= PlayerTurn;
       // }

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            inputManager = InputManager.Instance;
            cameraTransform = Camera.main.transform;
            //_cinemachineBrain = GetComponent<CinemachineBrain>();
        }


        // Fix This To Spawn New Tiles with Movement
  /*      private void PlayerTurn(InputAction.CallbackContext context)
        {
            Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
            //if (!turnPosition.HasValue)
            //{
            //    GameOver();
            //    return;
            //}

            Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * Vector3.forward;
            turnEvent.Invoke(targetDirection);
            Turn(context.ReadValue<float>(), turnPosition.Value);
        }

        private Vector3? CheckTurn(float turnValue)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
            if (hitColliders.Length != 0)
            {
                Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;
                if ((type == TileType.LEFT && turnValue == -1) || (type == TileType.RIGHT && turnValue == 1) || (type == TileType.SIDEWAYS))
                {
                    return tile.pivot.position;
                }
            }
            return null;
        } */
        // Working Below

        void Update()
        {

            if (inputManager.SprintStarting() && isWalking == true)
            {
                isSprinting = true;
                animator.SetTrigger("ForwardRun");
                animator.ResetTrigger("ForwardWalk");

            }
            if (inputManager.SprintFinishing() && isWalking == true)
            {
                isSprinting = false;
                animator.SetTrigger("ForwardWalk");
                animator.ResetTrigger("ForwardRun");

            }

            // Combine horizontal and vertical movement
            if (isSprinting == true)
            {
                groundedPlayer = controller.isGrounded;
                if (groundedPlayer && playerVelocity.y < 0)
                {
                    playerVelocity.y = 0f;
                }

                // Horizontal input
                Vector2 movement = inputManager.GetPlayerMovement();
                Vector3 move = new Vector3(movement.x, 0f, movement.y);
                move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
                move.y = 0f;
                move = Vector3.ClampMagnitude(move, 1f); // Optional: prevents faster diagonal movement

                //if (move != Vector3.zero)
                //{
                //    transform.forward = move;
                //}

                // Jump
                if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
                {
                    playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
                }
                if (inputManager.PlayerJumpedThisFrame() && !groundedPlayer)
                {
                    animator.SetTrigger("RunJumpInPlace");
                    animator.ResetTrigger("Idle");
                }
                if (groundedPlayer && isWalking == false)
                {
                    animator.ResetTrigger("RunJumpInPlace");
                    animator.SetTrigger("Idle");
                }
                if (!groundedPlayer && isWalking == true && isSprinting == false)
                {
                    animator.SetTrigger("Jump");
                    animator.ResetTrigger("ForwardWalk");
                }
                if (groundedPlayer && isWalking == true && isSprinting == false)
                {
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("ForwardWalk");
                }
                if (!groundedPlayer && isSprinting == true)
                {
                    animator.SetTrigger("RunJump");
                    animator.ResetTrigger("ForwardRun");
                }
                if (groundedPlayer && isSprinting == true)
                {
                    animator.ResetTrigger("RunJump");
                    animator.SetTrigger("ForwardRun");
                }




                // Apply gravity
                playerVelocity.y += gravityValue * Time.deltaTime;
                Vector3 finalMove = (move * playerRunning) + (playerVelocity.y * Vector3.up);
                controller.Move(finalMove * Time.deltaTime);

            }
            else
            {

                groundedPlayer = controller.isGrounded;
                if (groundedPlayer && playerVelocity.y < 0)
                {
                    playerVelocity.y = 0f;
                }

                // Horizontal input
                Vector2 movement = inputManager.GetPlayerMovement();
                Vector3 move = new Vector3(movement.x, 0f, movement.y);
                move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
                move.y = 0f;
                move = Vector3.ClampMagnitude(move, 1f); // Optional: prevents faster diagonal movement

                //if (move != Vector3.zero)
                //{
                //    transform.forward = move;
                //}

                // Jump
                if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
                {
                    playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
                }
                if (inputManager.PlayerJumpedThisFrame() && !groundedPlayer)
                {
                    animator.SetTrigger("RunJumpInPlace");
                    animator.ResetTrigger("Idle");
                }
                if (groundedPlayer && isWalking == false)
                {
                    animator.ResetTrigger("RunJumpInPlace");
                    animator.SetTrigger("Idle");
                }
                if (!groundedPlayer && isWalking == true && isSprinting == false)
                {
                    animator.SetTrigger("Jump");
                    animator.ResetTrigger("ForwardWalk");
                }
                if (groundedPlayer && isWalking == true && isSprinting == false)
                {
                    animator.ResetTrigger("Jump");
                    animator.SetTrigger("ForwardWalk");
                }
                if (!groundedPlayer && isSprinting == true)
                {
                    animator.SetTrigger("RunJump");
                    animator.ResetTrigger("ForwardRun");
                }
                if (groundedPlayer && isSprinting == true)
                {
                    animator.ResetTrigger("RunJump");
                    animator.SetTrigger("ForwardRun");
                }



                // Apply gravity
                playerVelocity.y += gravityValue * Time.deltaTime;
                Vector3 finalMove = (move * playerSpeed) + (playerVelocity.y * Vector3.up);
                controller.Move(finalMove * Time.deltaTime);
            }


            //Tidy this up later - Ask for help??

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                animator.SetTrigger("ForwardWalk");
                animator.ResetTrigger("Idle");
                isWalking = true;
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                animator.ResetTrigger("ForwardWalk");
                animator.SetTrigger("Idle");
                isWalking = false;
                isSprinting = false;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                animator.SetTrigger("WalkBack");
                animator.ResetTrigger("Idle");
                isWalking = true;
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                animator.ResetTrigger("WalkBack");
                animator.SetTrigger("Idle");
                isWalking = false;
                isSprinting = false;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //Rotation
                playerTrans.Rotate(0, -ro_speed * Time.deltaTime, 0);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                //Rotation
                playerTrans.Rotate(0, ro_speed * Time.deltaTime, 0);
            }

            if (inputManager.FirstPersonCameraTriggered()) // Switch to camera 1
            {
                ThirdPerson.Priority = 11;
                FirstPerson.Priority = 10;
            }
            if (inputManager.ThirdPersonCameraTriggered()) // Switch to camera 2
            {
                ThirdPerson.Priority = 10;
                FirstPerson.Priority = 11;
            }



        }
    }
}
