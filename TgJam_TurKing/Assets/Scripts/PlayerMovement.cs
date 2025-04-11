using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 10f;


    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        WASD();
    }

    private void WASD()
    {



        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = Vector3.ClampMagnitude((transform.right * h + transform.forward * v), 1f);
        dir = dir * speed * Time.fixedDeltaTime;
        dir.y = rb.velocity.y;

        rb.velocity = dir;
    }
}
