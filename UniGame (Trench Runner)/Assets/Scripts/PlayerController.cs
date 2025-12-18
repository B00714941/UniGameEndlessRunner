using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Cinemachine;
using TMPro;

namespace Runner
{

    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        //Initial SetUps
        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private LayerMask turnLayer;
        [SerializeField]
        private LayerMask obstacleLayer;
        [SerializeField]
        private float initialPlayerSpeed = 5f;
        [SerializeField]
        private float maxPlayerSpeed = 20f;
        [SerializeField]
        private float playerSpeedIncreaseRate = 2f;
        [SerializeField]
        private float jumpHeight = 1.0f;
        [SerializeField]
        private float initialGravityValue = -9.81f;
        [SerializeField]
        private UnityEvent<Vector3> turnEvent;
        [SerializeField]
        private UnityEvent<int> gameOverEvent;
        [SerializeField]
        private UnityEvent<int> gameWinEvent;
        [SerializeField]
        private UnityEvent<int> scoreUpdateEvent;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private float scoreMultiplier = 10f;
        [SerializeField]
        private AudioClip runningFX;
        [SerializeField]
        private TextMeshProUGUI gameOverscoreText;
        [SerializeField]
        private TextMeshProUGUI gameWinscoreText;
        [SerializeField]
        private TextMeshProUGUI distanceText;


        private float gravity;
        public float playerSpeed;
        private Vector3 movementDirection = Vector3.forward;
        private Vector3 playerVelocity;

        private PlayerInput playerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction cameraAction;
        public bool ChangeCameraPressed { get; private set; } = false;        

        private CharacterController controller;
        //public float fscore = 0;
        //public int score = 0;
        public int curscore = 0;
        public int curdistance = 0;
        public int maxdistance = 100;
        public int distancetravelled = 0;
        public int totalscore = 0;

        public CinemachineVirtualCamera ThirdPerson;
        public CinemachineVirtualCamera FirstPerson;

        private CinemachineBrain _cinemachineBrain;
        private AudioSource footstepsRunning;

        public int maxHealth;
        public Healthbar healthbar;
        public int curHealth;
        public GameObject healthPrefab;


        //When Game Begins
        private void Awake()
        {
            totalscore = curscore;
            distancetravelled = curdistance;
            curHealth = maxHealth;
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            turnAction = playerInput.actions["Turn"];
            jumpAction = playerInput.actions["Jump"];
            cameraAction = playerInput.actions["ChangeCamera"];
            footstepsRunning = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            turnAction.performed += PlayerTurn;
            jumpAction.performed += PlayerJump;
            cameraAction.performed += PlayerCamera;
        }

        private void OnDisable()
        {
            turnAction.performed -= PlayerTurn;
            jumpAction.performed -= PlayerJump;
            cameraAction.performed -= PlayerCamera;
        }
        
        public void Start()
         {
            totalscore = curscore;
            distancetravelled = curdistance;
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            turnAction = playerInput.actions["Turn"];
            jumpAction = playerInput.actions["Jump"];
            cameraAction = playerInput.actions["ChangeCamera"];
            footstepsRunning = GetComponent<AudioSource>();
            curHealth = maxHealth;
            playerSpeed = initialPlayerSpeed;
            gravity = initialGravityValue;
            _cinemachineBrain = GetComponent<CinemachineBrain>();
            footstepsRunning.clip = runningFX;
            footstepsRunning.Play();
         }

        private void PlayerCamera(InputAction.CallbackContext context)
        {

        }

        //Check if the player has entered a turn

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

        // Turn the player
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
            curdistance = curdistance + 10;
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


        // Main Game Movement and Update Score
        private void Update()
        {
            if (curHealth <= 0)
            {
                GameOver();
                return;
            }

            if (!IsGrounded(20f))
            {
                GameOver();
                return;
            }

            if (curdistance == maxdistance)
            {
                GameWin();
                return;
            }

            distancetravelled = curdistance;
            totalscore = curscore;

            controller.Move(transform.forward * playerSpeed * Time.deltaTime);

            if (IsGrounded() && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            /*fscore += scoreMultiplier * Time.deltaTime;
            scoreUpdateEvent.Invoke((int)fscore);
            int score = (int)fscore; */
            gameWinscoreText.text = totalscore.ToString();
            gameOverscoreText.text = totalscore.ToString();
            distanceText.text = distancetravelled.ToString();



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

        }

        //taking damage
        public void TakeDamage(int damage)
        {
            animator.SetTrigger("Collision");
            curHealth -= damage;
            curscore = curscore - 5;
            healthbar.UpdateHealth((float)curHealth/(float)maxHealth);

        }

        //adding health
        public void HealUp(int damage)
        {
            curHealth += damage;
            healthbar.UpdateHealth((float)curHealth / (float)maxHealth);
        }


        //Check if player on ground
        private bool IsGrounded(float length = .2f)
        {
            Vector3 raycastOriginFirst = transform.position;
            raycastOriginFirst.y -= controller.height / 2f;
            raycastOriginFirst.y += .1f;

            Vector3 raycastOriginSecond = raycastOriginFirst;
            raycastOriginFirst -= transform.forward * .2f;
            raycastOriginSecond += transform.forward * .2f;

            if (Physics.Raycast(raycastOriginFirst, Vector3.down, out RaycastHit hit, length, groundLayer) || Physics.Raycast(raycastOriginSecond, Vector3.down, out RaycastHit hit2, length, groundLayer))
            {
                return true;
            }
            return false;

        }

        //If Player is grounded allow JUMP!

        private void PlayerJump(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
                controller.Move(playerVelocity * Time.deltaTime);
                animator.SetTrigger("RunJump");
                //curscore = curscore + 10;

            }

        }


        //Game Over Section
        private void GameOver()
        {
            curHealth = 0;
            animator.SetTrigger("Death");
            Debug.Log("Game Over");
            gameOverEvent.Invoke((int)totalscore);
            playerSpeed = 0;
            gameObject.SetActive(false);
            footstepsRunning.Stop();
        }

        //Game Win Section
        private void GameWin()
        {
            distanceText.text = maxdistance.ToString();
            Debug.Log("Congrats!");
            gameWinEvent.Invoke((int)totalscore);
            playerSpeed = 0;
            gameObject.SetActive(false);
            footstepsRunning.Stop();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 7)
            {
                TakeDamage(17);
                //animator.SetTrigger("Collision");
                if (curHealth <= 0)
                {
                    animator.Play("Death");
                    GameOver();
                }

            }
            if (other.gameObject.layer == 8)
            {
                GameOver();
            }

            if (other.gameObject.layer == 9)
            {
                HealUp(20);
                Destroy(other.gameObject);
                //curHealth = 100;
           

            }

            if (other.gameObject.layer == 10)
            {
                curscore = curscore + 10;

            }

            if (curHealth <= 0)
            {
                GameOver();
            }

        }


    }
}
