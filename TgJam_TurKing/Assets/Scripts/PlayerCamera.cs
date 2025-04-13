using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public GameObject cam;
	public float yMin = -90f, yMax = 90f;
	public float sensitivity = 1f; // Чувствительность мыши
	public bool canLook = true;

	private float x = 0f, y = 0f;

	private void OnEnable()
	{
		sensitivity = PlayerPrefs.GetFloat("Sens", 0.5f) * 2;
	}

	private void Update()
	{
		Look();
	}

	private void Look()
	{
		float mouseX = Input.GetAxis("Mouse X") * sensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

		x += mouseX;
		y -= mouseY;

		y = Mathf.Clamp(y, yMin, yMax);

		if (canLook)
		{
			transform.rotation = Quaternion.Euler(0f, x, 0f);
			cam.transform.localRotation = Quaternion.Euler(y, 0f, 0f);
		}
	}



	//Camera
}
