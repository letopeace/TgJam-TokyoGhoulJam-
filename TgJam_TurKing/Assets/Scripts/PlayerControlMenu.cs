using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlMenu : MonoBehaviour
{
    public float speed;
    public float rotation;

    [SerializeField] GameObject cam;
    Rigidbody rb;
    private float Xrotate = 0f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        float xx, yy;
        xx = Input.GetAxis("Horizontal");
        yy = Input.GetAxis("Vertical");

        float gravity = rb.velocity.y;
        rb.velocity = (transform.right * xx + transform.forward * yy).normalized * speed;
        rb.velocity += gravity * Vector3.up;

        float x=0, y = 0;
        
        x += Input.GetAxis("Mouse X");
        y += Input.GetAxis("Mouse Y");

        Vector3 camRot = cam.transform.eulerAngles;
        camRot.x = Mathf.Clamp(-y, -80f, 80f);
        cam.transform.eulerAngles = camRot;

        Vector3 playerRot = transform.eulerAngles;
        playerRot.y = x;
        transform.eulerAngles = playerRot;

    }
}
