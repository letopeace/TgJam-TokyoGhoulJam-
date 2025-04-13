using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject cam;
    public float yMin = -90, yMax = 90;
    public bool canLook;

    private float x, y;

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        x += Input.GetAxis("Mouse X");
        y += Input.GetAxis("Mouse Y");

        Vector3 camRot = cam.transform.eulerAngles;
        camRot.x = Mathf.Clamp(-y, yMin, yMax);
        if(canLook) cam.transform.eulerAngles = camRot;

        Vector3 playerRot = transform.eulerAngles;
        playerRot.y = x;
        if(canLook) transform.eulerAngles = playerRot;
    }

    //Camera
}
