using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThridPersonController : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private InputReaderSO inputReader;
    
    [Header("Movement")] 
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityScale;

    [Header("Ground Detection")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask whatIsGround;

    private CharacterController controller;
    private Camera cam;

    private bool isGrounded;
    private Vector2 inputVector;
    private Vector3 verticalMovement;

    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
        cam = Camera.main;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        inputReader.OnJumpStarted += Jump;
        inputReader.OnMoveEvent += UpdateMovement;
    }
    
    private void OnDisable()
    {
        inputReader.OnJumpStarted -= Jump;
        inputReader.OnMoveEvent -= UpdateMovement;
    }

    private void UpdateMovement(Vector2 input)
    {
        inputVector = input;
    }
    
    private void Jump()
    {
        if (isGrounded)
        {
            verticalMovement.y = Mathf.Sqrt(-2 * gravityScale * jumpHeight);
        }
    }

    void Update()
    {
        GroundCheck();
        ApplyGravity();
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        // Rotar el cuerpo con la cámara
        
        // Calcular movimiento
        Vector3 movement = Vector3.zero;
        
        if (inputVector.magnitude > 0)
        {
            // Calcular ángulo basado en input y cámara
            float angleToRotate = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            
            // Calcular velocidad (respeta magnitud del joystick)
            float speed = movementSpeed * inputVector.magnitude;
            
            // Rotar Vector3.forward al ángulo calculado
            Vector3 direction = Quaternion.Euler(0, angleToRotate, 0) * Vector3.forward;
            
            transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            
            movement = direction * speed;
        }
        
        // Aplicar movimiento
        controller.Move((movement + verticalMovement) * Time.deltaTime);
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
        if (feet != null)
        {
            Gizmos.DrawSphere(feet.position, detectionRadius);
        }
    }
}
