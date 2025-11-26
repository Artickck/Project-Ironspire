using System;
using System.Net;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 PlayerMovementInput;
    private Vector2 PlayerMouseInput;
    private float xRot;

    private Animator mAnimator;

    [SerializeField] private Transform playerCamera;
    [SerializeField] private CharacterController m_Movement;
    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float jumpforce;
    [SerializeField] private float sensitivity;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private AudioSource goblinLeap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goblinLeap = GetComponent<AudioSource>();
        m_Movement = GetComponent<CharacterController>();
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
       
        movePlayer();
        movePlayerCamera();
        moveAnime();
    }

    private void moveAnime()
    {
        if (mAnimator != null)
        {
            float animSpeed = new Vector3(m_Movement.velocity.x, 0, m_Movement.velocity.z).magnitude;
            mAnimator.SetFloat("speed", animSpeed);
        }
    }

    private void movePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(PlayerMovementInput);
        Debug.Log(m_Movement.isGrounded);
            if (m_Movement.isGrounded)
            {
                velocity.y = -1f;
                mAnimator.SetBool("jumping", false);
                if (Input.anyKeyDown) Console.WriteLine("Key Pressed");
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity.y = jumpforce;
                    mAnimator.SetBool("jumping", true);
                    goblinLeap.Play();
                }
            }   

        else velocity.y -= gravity * Time.deltaTime;

        m_Movement.Move(moveVector * speed * Time.deltaTime);
        m_Movement.Move(velocity * Time.deltaTime);
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            //gourded = two
            ggourded = two;
        }
    }

    private void movePlayerCamera()
    {
        xRot -= PlayerMouseInput.y * sensitivity;
        transform.Rotate(0f, PlayerMouseInput.x * sensitivity, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }


}
