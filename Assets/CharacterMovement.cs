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


    private Animator mAnimator;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private CharacterController m_Movement;
    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float jumpforce;
    [SerializeField] private float sensitivity;
    [SerializeField] private float gravity = -9.81f;
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
        playerCamera = Camera.main;

        SetCursorState(true);
    }

    void Update()
    {   
        isGrounded = m_Movement.isGrounded;

        movePlayer();
        movePlayerCamera();
        updateAnimator();
    }

    private void GetInputs()
    {
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
        
        controls.Player.Jump.canceled += ctx => Jump();
    }

    private void movePlayer()
    {
        Vector3 move = transform.TransformDirection(new Vector3(moveInput.x, 0f, moveInput.y));
        m_Movement.Move(move * speed * Time.deltaTime);

        if(isGrounded)
        {
            if(velocity.y < 0)
                velocity.y = -1f;
            
            mAnimator.SetBool("jumping", false);
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        m_Movement.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if(isGrounded)
        {
            velocity.y = jumpforce;
            mAnimator.SetBool("jumping", true);
            if(goblinLeap) goblinLeap.Play();
        }
    }

    private void movePlayerCamera()
    {
        xRot -= lookInput.y * sensitivity;
        xRot = Mathf.Clamp(xRot, -80f, 80f);

        transform.Rotate(0f,lookInput.x * sensitivity, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }
    
    private void updateAnimator()
    {
        if (mAnimator != null)
        {
            float animSpeed = new Vector3(m_Movement.velocity.x, 0, m_Movement.velocity.z).magnitude;
            mAnimator.SetFloat("speed", animSpeed);
        }
    }

    private void SetCursorState(bool state)
    {   
        Cursor.visible = !state;
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;            
    }
}
