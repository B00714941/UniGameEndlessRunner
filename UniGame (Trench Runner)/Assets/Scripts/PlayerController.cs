using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Cinemachine;

namespace Runner.Player
{

    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {

        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private LayerMask turnLayer;
        [SerializeField]
        private LayerMask obstacleLayer;
        [SerializeField]
        private float initialPlayerSpeed = 4f;
        [SerializeField]
        private float maxPlayerSpeed = 20f;
        [SerializeField]
        private float playerSpeedIncreaseRate = .1f;
        [SerializeField]
        private float jumpHeight = 1.0f;
        [SerializeField]
        private float initialGravityValue = -9.81f;
        [SerializeField]
        private UnityEvent<Vector3> turnEvent;
        [SerializeField]
        private UnityEvent<int> gameOverEvent;
        [SerializeField]
        private UnityEvent<int> scoreUpdateEvent;
        [SerializeField]
        private AnimationClip slideAnimationClip;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private float scoreMultiplier = 10f;

        private float gravity;
        public float playerSpeed;
        private Vector3 movementDirection = Vector3.forward;
        private Vector3 playerVelocity;

        private PlayerInput playerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;
        private InputAction cameraAction;
        public bool ChangeCameraPressed { get; private set; } = false;        

        private CharacterController controller;
        private int slidingAnimationId;

        private bool sliding = false;
        private float score = 0;

        public CinemachineVirtualCamera ThirdPerson;
        public CinemachineVirtualCamera FirstPerson;

        private CinemachineBrain _cinemachineBrain;


        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            turnAction = playerInput.actions["Turn"];
            jumpAction = playerInput.actions["Jump"];
            slideAction = playerInput.actions["Slide"];
            cameraAction = playerInput.actions["ChangeCamera"];

            slidingAnimationId = Animator.StringToHash("Sliding");

        }

        private void OnEnable()
        {
            turnAction.performed += PlayerTurn;
            slideAction.performed += PlayerSlide;
            jumpAction.performed += PlayerJump;
            cameraAction.performed += PlayerCamera;

        }

        private void OnDisable()
        {
            turnAction.performed -= PlayerTurn;
            slideAction.performed -= PlayerSlide;
            jumpAction.performed -= PlayerJump;
            cameraAction.performed -= PlayerCamera;

        }



        private void Start()
        {
            playerSpeed = initialPlayerSpeed;
            gravity = initialGravityValue;
            _cinemachineBrain = GetComponent<CinemachineBrain>();

        }

        private void PlayerCamera(InputAction.CallbackContext context)
        {

        }

        private void PlayerTurn(InputAction.CallbackContext context)
        {
            Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
            if (!turnPosition.HasValue)
            {
                GameOver();
                return;
            }

            Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
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
        }

        private void Turn(float turnValue, Vector3 turnPosition)
        {
            Vector3 tempPlayerPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
            controller.enabled = false;
            transform.position = tempPlayerPosition;
            controller.enabled = true;

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
            transform.rotation = targetRotation;
            movementDirection = transform.forward.normalized;
        }


        private void PlayerSlide(InputAction.CallbackContext context)
        {
            if (!sliding && IsGrounded())
            {
                StartCoroutine(Slide());
            }

        }

        private IEnumerator Slide()
        {

            sliding = true;
            Vector3 originalControllerCenter = controller.center;
            Vector3 newControllerCenter = originalControllerCenter;

            controller.height /= 2;
            newControllerCenter.y -= controller.height / 2;
            controller.center = newControllerCenter;

            
            animator.Play(slidingAnimationId);
            yield return new WaitForSeconds(slideAnimationClip.length / animator.speed);
     
            controller.height *= 2;
            controller.center = originalControllerCenter;
            sliding = false;

        }


        private void PlayerJump(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
                controller.Move(playerVelocity * Time.deltaTime);
            }

        }

        private void Update()
        {

            if (!IsGrounded(20f))
            {
                GameOver();
                return;
            }

            controller.Move(transform.forward * playerSpeed * Time.deltaTime);

            if (IsGrounded() && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            score += scoreMultiplier * Time.deltaTime;
            scoreUpdateEvent.Invoke((int)score);

            if (playerSpeed < maxPlayerSpeed)
            {
                playerSpeed += Time.deltaTime * playerSpeedIncreaseRate;
                gravity = initialGravityValue - playerSpeed;

                if (animator.speed < 1.25f)
                {
                    animator.speed += (1 / playerSpeed) * Time.deltaTime;
                }
            }

            if (Input.GetKeyDown(KeyCode.C)) // Switch to camera 1
            {
                ThirdPerson.Priority = 11;
                FirstPerson.Priority = 10;
            }
            else if (Input.GetKeyDown(KeyCode.V)) // Switch to camera 2
            {
                ThirdPerson.Priority = 10;
                FirstPerson.Priority = 11;
            }

            //ChangeCameraPressed = PlayerControls.ChangeCamera.WasPressedThisFrame();

        }

        private bool IsGrounded(float length = .2f)
        {
            Vector3 raycastOriginFirst = transform.position;
            raycastOriginFirst.y -= controller.height / 2f;
            raycastOriginFirst.y += .1f;

            Vector3 raycastOriginSecond = raycastOriginFirst;
            raycastOriginFirst -= transform.forward * .2f;
            raycastOriginSecond += transform.forward * .2f;

            //Debug.DrawLine(raycastOriginFirst, Vector3.down, Color.green, 2f);
            //Debug.DrawLine(raycastOriginSecond, Vector3.down, Color.red, 2f);

            if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
            {
                return true;
            }
            return false;

        }

        private void GameOver()
        {
            Debug.Log("Game Over");
            gameOverEvent.Invoke((int)score);
            gameObject.SetActive(false);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
            {
                GameOver();
            }

        }


    }
}
