using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplotionEffect : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int eventCount = ps.GetCollisionEvents(other, collisionEvents);
        for (int i = 0; i < eventCount; i++)
        {
            Vector3 collisionPos = collisionEvents[i].intersection;
            Debug.Log("Particle collided at: " + collisionPos);
        }
    }
}
