using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChertBehaviour : BaseEnemy
{
    public float minDistance = 1f;

    private void Awake()
    {
        SetState(new IdleState());
    }

    public override void Follow()
    {
        if (distance < minDistance)
            return;
        base.Follow();
    }
}
