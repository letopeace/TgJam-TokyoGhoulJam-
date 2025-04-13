using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwichScene : MonoBehaviour
{
    public int scneneNumber;

    public void Swich()
    {
        if(scneneNumber == -1)
        {
            Application.Quit();
            return;
        }
        SceneManager.LoadScene(scneneNumber);

    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject != null && other.gameObject.tag == "Player")
        {
            Swich();
        }
	}
}
