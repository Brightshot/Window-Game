using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovement;

[RequireComponent(typeof(Rigidbody2D),typeof(InputHandeler))]
public class PlayerMovement : MonoBehaviour
{

    #region Variables

    private Vector2 input;
    public static Vector2 PlayerVelocity { get; private set; }
    public Transform Visuals;

    #endregion

    #region Components

    private Rigidbody2D m_Rigidbody;
    private InputHandeler inputHandler;  //Script Handling all input events

    #endregion

    [Space]

    #region GroundCheckVariables
    [Header("Ground Check")]
    public Vector2 detectionScale = new Vector2( 0.5f , 0.25f ) ;  //Scale of overlap box detecting ground
    public Vector2 offset = new Vector2(0,-0.5f);  //overlap box offset from player position 
    [Space]
    public LayerMask GroundLayer;   //The Layers to be detected by overlap box
    public bool lastOnGround { get; private set; } //is player on ground after a time?

    public static bool isGrounded;
    [SerializeField, Space, Range(0, 1f)]
    private float cayoteTime = 0.2f;
    #endregion

    [Space]

    #region MovementVariables

    [Header("Movement")]
    public float acceleration=8;
    public float deceleration=6;
    public float MaxSpeed=12;

    [Range(0,1f)]
    public float velocityPower=0.3f;

    #endregion

    #region Events
    public delegate void M_Jump();
    public static M_Jump jump;
    #endregion

    [Space]

    [Header("Jump")]
    public float JumpForce=5;

    [Tooltip("The Amount of time the jump function should be true. \nThe Higher the value the further away from the ground jump can be called"),Range(0,0.6f)]
    public float BufferTime=0.2f;

    public static bool Aiming;
    public static Vector2 AimDirection;


    private void Start()
    {
        inputHandler = GetComponent<InputHandeler>();
        m_Rigidbody = GetComponent<Rigidbody2D>();  //Rigidbody
    }

    private void Update()
    {
        Actions();
        InputHandle();
        CheckGround();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Actions()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CallKey("jump", BufferTime);
        }

        /*if (InputHandeler.InputAxis.y > 0.9f)
        {
            CallKey("jump", BufferTime);  // if up is pressed
        }*/

        if (lastOnGround == true && KeyEvents.pressed("jump"))
        {
            Jump();
            KeyEvents.KeysCalled.Remove("jump");
        }


    }

    Vector2 aim;    //stores the scale of player when aiming
    private void InputHandle()
    {
        input = InputHandeler.InputAxis;
        PlayerVelocity = m_Rigidbody.velocity;

        if (Mathf.Abs(AimDirection.x) > 0.01f)
        {
            aim = new Vector3(1 * Mathf.Sign(AimDirection.x), 1);
        }

        if ((Mathf.Abs(input.x) > 0.05) && !Aiming)
        {
            Visuals.localScale = new Vector3(1 * Mathf.Sign(input.x), 1);
        } 
        else if (Aiming)
        {
            if(Mathf.Abs(AimDirection.x) != 0)
            {
                Visuals.localScale = aim;
            }
        }
    }

    private void Movement()
    { 
        float targetSpeed = input.x * MaxSpeed;     //get the speed the player wants to move towards
        float speed_difference = targetSpeed - m_Rigidbody.velocity.x;
        float speed = (Mathf.Abs(input.x) > 0.1f) ? acceleration : deceleration;    //if moving forward accelerate else decelerate 
        float movementSpeed =  Mathf.Pow(Mathf.Abs(speed_difference) * speed,velocityPower) * Mathf.Sign(speed_difference); 

        m_Rigidbody.AddForce(transform.right * movementSpeed * 10,ForceMode2D.Force);
    }

    private bool jumped;
    void Jump()
    {
        StartCoroutine("ResetJump");
        jump?.Invoke();
        m_Rigidbody.velocity = new Vector2(m_Rigidbody.velocity.x , 0);
        m_Rigidbody.AddForce(transform.up.normalized * JumpForce, ForceMode2D.Impulse);
        lastOnGround = false;
    }


    IEnumerator ResetJump()
    {
        jumped = true;
        yield return new WaitForSeconds(0.3f);
        jumped = false;
    }

    //Ground Detection
    bool has_checked=false;
    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapBox((Vector2)transform.position + offset, detectionScale, 90,GroundLayer);
        if (isGrounded && !jumped) lastOnGround = true;


        if (!isGrounded && !has_checked)
        {
            has_checked = true;
            StartCoroutine(CheckIfOnGround());
        }
        else if (isGrounded)
        {
            has_checked = false;
        }
    }

    #region Numerators

    private IEnumerator CheckIfOnGround()
    {
        yield return new WaitForSeconds(cayoteTime);
        lastOnGround = false;
    }

    #endregion

    #region EditorFunctions

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + offset, detectionScale);
    }

    void CallKey(string key,float delay)
    {
        StartCoroutine(KeyEvents.KeyPressed(key,delay));
    }

    #endregion
}