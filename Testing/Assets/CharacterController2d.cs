using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovement : MonoBehaviour
{
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    Rigidbody2D rigidbody2d;
    [SerializeField] float speed = 2f;
    Vector2 motionVector;
    public Vector2 lastMotionVector;
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        motionVector = new Vector2(horizontal, vertical);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);

        if(horizontal != 0 || vertical !=0) 
        {
            lastMotionVector = new Vector2(horizontal,vertical).normalized;

            animator.SetFloat("LastHorizontal", horizontal);
            animator.SetFloat("LastVertical", vertical);
        }


        if (animator.GetFloat("Horizontal") == 0 && animator.GetFloat("Vertical") == 0)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }

        Sprint();
    }


private void FixedUpdate()
    {
      
        Move();
        

    }

    private void Move()
    {
     
        rigidbody2d.linearVelocity = motionVector * speed;
  


    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) == true)
        {
            Debug.Log($"Speed ShiftDown: {speed}");
            speed = 4f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) == true)
        {
            Debug.Log($"Speed ShiftUp: {speed}");
            speed = 2f;
        }
    }
}