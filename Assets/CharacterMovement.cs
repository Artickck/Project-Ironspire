using System;
using System.Net;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isGrounded;
    private Vector3 velocity;
    private float xRot;
    private float cameraYaw;


    private Animator mAnimator;

    [SerializeField] public Camera playerCamera;
    [SerializeField] private CharacterController m_Movement;
    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float jumpforce;
    [SerializeField] private float sensitivity;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.5f, -4f);
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDist;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private AudioSource goblinLeap;


    void Awake()
    {
        controls = new PlayerControls();

        GetInputs();
    }
    void Start()
    {
        goblinLeap = GetComponent<AudioSource>();
        m_Movement = GetComponent<CharacterController>();
        mAnimator = GetComponent<Animator>();

        SetCursorState(true);
    }

    void Update()
    {       
        Debug.Log($"isGrounded: {isGrounded}, velocity.y: {velocity.y}");

        movePlayer();
        movePlayerCamera();
        updateAnimator();
    }

    private void GetInputs()
    {   
        //InputActions
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
        
        controls.Player.Jump.performed += ctx => Jump();
    }

    private void movePlayer()
    {       
        //GroundCheck
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundLayer);
        
        Vector3 camForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = playerCamera.transform.right;

        Vector3 move = camForward * moveInput.y + camRight * moveInput.x;

        if (move.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), 10f * Time.deltaTime);

        Vector3 velocityMove = move.normalized * speed;
        velocityMove.y = velocity.y;
        m_Movement.Move(velocityMove * Time.deltaTime);
        
        if(!isGrounded)
            velocity.y += gravity * Time.deltaTime;

        if(isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    private void Jump()
    {   
        if(isGrounded)
        {
            velocity.y = jumpforce;
            if(goblinLeap) goblinLeap.Play();
            if (mAnimator != null)
                mAnimator.SetTrigger("jump");
        }
    }

    //Orbit Camera
    private void movePlayerCamera()
    {
        xRot -= lookInput.y * sensitivity;
        xRot = Mathf.Clamp(xRot, 0f, 80f);
        cameraYaw += lookInput.x * sensitivity;

        float radius = cameraOffset.magnitude;
        float pitchRad = Mathf.Deg2Rad * xRot;
        float yawRad = Mathf.Deg2Rad * cameraYaw;

        Vector3 offset = new Vector3(
            radius * Mathf.Cos(pitchRad) * Mathf.Sin(yawRad),
            radius * Mathf.Sin(pitchRad),
            radius * Mathf.Cos(pitchRad) * Mathf.Cos(yawRad)
        );

        Vector3 targetPosition = transform.position + offset;

        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, targetPosition, Time.deltaTime * 10f);
        playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);
    }
    
    private void updateAnimator()
    {
        if (mAnimator != null)
        {
            float animSpeed = moveInput.magnitude;
            mAnimator.SetFloat("speed", animSpeed);

            mAnimator.SetBool("isGrounded", isGrounded);
        }
    }

    private void SetCursorState(bool state)
    {   
        Cursor.visible = !state;
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;            
    }

    void OnEnable()
    {
        if (controls == null)
            controls = new PlayerControls();

        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }
}
