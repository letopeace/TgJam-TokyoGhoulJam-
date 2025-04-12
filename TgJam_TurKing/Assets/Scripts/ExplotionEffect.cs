using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ExplotionEffect : MonoBehaviour
{
    public GameObject BloodEffect;
    void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int eventCount = ps.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < eventCount; i++)
        {
            GameObject effect = Instantiate(BloodEffect, collisionEvents[i].intersection, Quaternion.identity);
            Vector3 euler = Quaternion.FromToRotation(-Vector3.forward, collisionEvents[i].normal).eulerAngles;
            var main = effect.GetComponent<ParticleSystem>().main;
            main.startRotationX = Mathf.Deg2Rad * euler.x;
            main.startRotationY = Mathf.Deg2Rad * euler.y;
            main.startRotationZ = Mathf.Deg2Rad * euler.z;
            effect.GetComponent<ParticleSystem>().Play();

            //Debug.DrawRay(collisionEvents[i].intersection, collisionEvents[i].normal, Color.red, 10f);
        }
    }
}
