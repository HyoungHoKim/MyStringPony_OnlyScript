using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proto_Move : MonoBehaviour
{

    public float m_Speed = 12f;
    public float m_TurnSpeed = 180f;
    public float m_PitchRange = 0.2f;


    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;

    private Animator anim;
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashSide = Animator.StringToHash("Side");

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        anim = transform.Find("AdamKnight_Horse").GetComponent<Animator>();
    }

    private void Start()
    {
        m_MovementAxisName = "Vertical";
        m_TurnAxisName = "Horizontal";
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        if (Input.GetKeyDown("e"))
        {
            m_Speed -= 2f;
        }

        if (Input.GetKeyDown("q"))
        {
            m_Speed += 2;
        }
    }

    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }


    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        
        anim.SetFloat(hashSpeed, m_MovementInputValue * m_Speed);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);

        anim.SetFloat(hashSide, m_TurnInputValue * m_Speed);
    }
}

