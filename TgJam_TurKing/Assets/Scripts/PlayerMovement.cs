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
    private float timeOnWallMax = 1.5f;
    public float timeOnWall = 1.5f;


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

        //rb.velocity = Vector3.Lerp(rb.velocity, dir, 0.1f);
        rb.velocity = dir;
    }

    private void MoveOnWall()
    {
        if (!onWall)
        { return; }

        rb.useGravity = false;

        Vector3 wallRight = Vector3.Cross(Vector3.up, wallNormal).normalized;
        Vector3 wallLeft = -wallRight;

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 projectedView = Vector3.ProjectOnPlane(camForward, wallNormal).normalized;

        Vector3 wallRunDirection =
            Vector3.Dot(projectedView, wallRight) > 0 ? wallRight : wallLeft;


        float verticalInfluence = Mathf.Clamp(camForward.y, -0.3f, 0.3f);
        if (timeOnWall < 0)
        {
            verticalInfluence = -1f;
        }

        Vector3 finalVelocity = wallRunDirection * speed * Time.fixedDeltaTime + Vector3.up * verticalInfluence * wallRunClimbSpeed;
        finalVelocity.y = Mathf.Clamp(finalVelocity.y, -maxSlideSpeed, maxClimbSpeed);

        rb.velocity = finalVelocity;

        timeOnWall -= Time.deltaTime;
    }

    private void Jump()
    {
        if ((onGround || onWall) && Input.GetKeyDown(KeyCode.Space))
        {
            if (onWall)
            {
                onWall = false;
                timeOnWall = timeOnWallMax;
                Vector3 dir = wallNormal * 2 + Vector3.up;
                rb.velocity = dir * jumpForce * 2;
            }
            else
            {
                Vector3 dir = Vector3.up * 2;
                rb.velocity = dir * jumpForce;
            }
        }

    }





    private void OnCollisionStay(Collision collision)
    {
        onGround = false;

        ContactPoint[] contacts = collision.contacts;
        for (int i = 0; i < contacts.Length; i++)
        {
            if (contacts[i].normal.y > 0.66f && !onGround) 
            {
                onGround = true;
                timeOnWall = timeOnWallMax;
                wallNormal = Vector3.zero;
                onWall = false;
            }
            else if (contacts[i].normal.y > -0.66f && !onWall && wallNormal != contacts[i].normal)
            {
                onWall = true;
                wallNormal = contacts[i].normal;
            }
        }
    }

    
}

