using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FinalArena : MonoBehaviour
{
    public Transform[] point;

    public GameObject[] Enemies;

    public float minColdown;
    public float maxColdown;

    public float Timer;
    public GameObject text;

    bool isStarted;

    List<GameObject> AllMosters = new List<GameObject>();

    private void OnCollisionEnter(Collision collision)
    {
        if(collision != null && collision.gameObject != null && collision.gameObject.tag == "Player")
        {
            isStarted = true;
            text.transform.DOScale(Vector3.one, 1f);
            StartTrail();
        }  
    }

    private void Update()
    {
        if(isStarted)
        {
            Timer -= Time.deltaTime;
            text.GetComponent<Text>().text = ((int)Timer).ToString();
        }
        if(Timer < 0)
        {
            text.transform.DOScale(Vector3.zero, 1f);
            EndTrail();    
        }
    }

    void EndTrail()
    {
        isStarted = false;

    }

    async Task StartTrail()
    {
        
        await Task.Delay((int)Random.Range(minColdown, maxColdown));

        if (isStarted)
        {
            GameObject moster = Instantiate(Enemies[Random.Range(0, Enemies.Length)], point[Random.Range(0, point.Length)].position, Quaternion.identity);

            AllMosters.Add(moster);
        }
        else return;
    }
}
