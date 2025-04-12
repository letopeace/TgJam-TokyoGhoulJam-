

using UnityEngine;

public class FioletovyiChert : BaseEnemy
{
	public float minDistance = 1f;

	private float rayDistance = 4f;

	private void Awake()
	{
		SetState(new IdleState());
	}

	private void Update()
	{
		base.Update();

		Debug.DrawRay(transform.position + Vector3.up * 1.5f, (transform.forward + Vector3.down) * rayDistance, Color.green);
	}

	public override void Follow()
	{
		if (PlatformDetect())
		{
			Vector3 directionAwayFromPlayer = transform.position - mainCamera.position;
			directionAwayFromPlayer.y = 0f;
			transform.rotation = Quaternion.LookRotation(directionAwayFromPlayer, Vector3.up);

			if (!staned) rb.velocity = transform.forward * speed * Time.deltaTime;
		}
		

	}

	private bool PlatformDetect()
	{
		Ray ray = new Ray(transform.position + Vector3.up * 1.5f, transform.forward + Vector3.down);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, rayDistance))
		{
			Debug.Log("Мы попали в: " + hit.collider.name);
			return true;
		}
		else
		{
			Debug.Log("Ничего не найдено");
			return false;
		}
	}
}
