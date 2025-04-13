
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadOnCollisionEnter : MonoBehaviour
{
    public string sceneName;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Player")
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}
