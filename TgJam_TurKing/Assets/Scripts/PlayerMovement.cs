using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 10f, jumpForce = 1.5f;
    public float wallRunClimbSpeed = 10f, maxClimbSpeed = 10f, maxSlideSpeed = 10f;

    public bool onGround;
    public bool onWall;


    private Rigidbody rb;
    private Vector3 wallNormal;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        WASD();
    }

    private void WASD()
    {
        if (!onGround && onWall)
        {
            MoveOnWall();
            return;
        }

        rb.useGravity = true;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = Vector3.ClampMagnitude((transform.right * h + transform.forward * v), 1f);
        dir = dir * speed * Time.fixedDeltaTime;
        dir.y = rb.velocity.y * 1.01f;

        rb.velocity = dir;
    }

    private void MoveOnWall()
    {
        rb.useGravity = false;
        Vector3 camForward = Camera.main.transform.forward;

        // Вычисляем движение по стене
        Vector3 wallMoveDir = Vector3.ProjectOnPlane(camForward, wallNormal).normalized;

        float verticalInfluence = Mathf.Clamp(Camera.main.transform.forward.y, -1f, 1f);

        Vector3 finalVelocity = wallMoveDir * speed + Vector3.up * verticalInfluence * wallRunClimbSpeed;
        finalVelocity.y = Mathf.Clamp(finalVelocity.y, -maxSlideSpeed, maxClimbSpeed);

        rb.velocity = finalVelocity;
    }

    private void Jump()
    {
        if (onGround && Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 dir = transform.up + Vector3.up;
            rb.velocity = dir * jumpForce;
        }
    }



    

    private void OnCollisionStay(Collision collision)
    {
        onGround = false;
        onWall = false;

        ContactPoint[] contacts = collision.contacts;
        for (int i = 0; i < contacts.Length; i++)
        {
            if (contacts[i].normal.y > 0.66f) 
            {
                onGround = true;
            }
            else if (contacts[i].normal.y > -0.66f)
            {
                onWall = true;
                wallNormal = contacts[i].normal;
            }
        }
    }

    
}

