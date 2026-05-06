using System;
using UnityEngine;

public class ShoulderPersonController : MonoBehaviour
{
    [Header("ScriptableObjects")] 
        [SerializeField] private InputReaderSO inputReader;
        
        [Header("Movement")] 
        [SerializeField] private float movementSpeed;
        [SerializeField] private float jumpHeight;
        [SerializeField] private float gravityScale;
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private float movementSmoothFactor = 0.3f;
    
    
        [Header("Ground Detection")]
        [SerializeField] private Transform feet;
        [SerializeField] private float detectionRadius;
        [SerializeField] private LayerMask whatIsGround;
    
        private CharacterController controller;
    
        private bool isGrounded;
        
    
        private Vector2 inputVector; 
        private Vector3 horizontalMovement; 
        private Vector3 verticalMovement;
        private Vector3 totalMovement;
        

        //No hay que darle valor.
        private float currentRotationVelocity;
        
        private float currentSpeed;
        private float currentXSpeed;
        private float currentYSpeed;
    
        private float speedVelocity;
        private float speedXVelocity;
        private float speedYVelocity;
    
        private Camera cam;


        public event Action<float, float> OnMoving;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            cam = Camera.main;
            Cursor.lockState = CursorLockMode.Locked;
        }
    
        private void OnEnable()
        {
            inputReader.OnJumpStarted += Jump;
            inputReader.OnMoveEvent += UpdateMovement;
        }
    
        private void UpdateMovement(Vector2 input)
        {
            inputVector = input;
        }
        
        private void Jump()
        {
            verticalMovement.y= Mathf.Sqrt(-2 * gravityScale * jumpHeight);
        }
    
    
        void Update()
        {
            GroundCheck(); 
            ApplyGravity();
            MoveAndRotate(); 
        }
    
        private void MoveAndRotate()
        {
            //Mi cuerpo se rota con la cámara: Siempre estoy pendiente de la rotación de la cámara.
            transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);

            //Calcula la velocidad objetivo
            float targetSpeed = movementSpeed * inputVector.magnitude;

            
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, movementSmoothFactor);
            
            Vector3 movement = Vector3.zero;

            //Si hay movimiento....
            if (inputVector.sqrMagnitude > 0 )
            {
                float angleToRotate = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;

                Vector3 direction = Quaternion.Euler(0, angleToRotate, 0) * Vector3.forward;

                movement = direction * currentSpeed;
                
            }
            controller.Move((movement + verticalMovement) * Time.deltaTime);
            
            UpdateMovementInfo();
        }

        private void UpdateMovementInfo()
        {
            currentXSpeed = Mathf.SmoothDamp(currentXSpeed, inputVector.x, ref speedXVelocity, movementSmoothFactor);
            
            currentYSpeed = Mathf.SmoothDamp(currentYSpeed, inputVector.y, ref speedYVelocity, movementSmoothFactor);
    
            OnMoving?.Invoke(currentXSpeed, currentYSpeed);
        }


        private void ApplyGravity()
        {
            if (isGrounded && verticalMovement.y < 0)
            {
                verticalMovement.y = -2f;
            }
            else
            {
                verticalMovement.y += gravityScale * Time.deltaTime;
            }
        }
    
        private void GroundCheck()
        {
            isGrounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(feet.position, detectionRadius);
        }
}
