
public interface EnemyState
{
    public void Enter(BaseEnemy enemy);
    public void Update(BaseEnemy enemy);
    public void Exit(BaseEnemy enemy);

}



public class IdleState : EnemyState
{

    public void Enter(BaseEnemy enemy)
    {
        enemy.animator.SetBool("Idle", true);
        enemy.DebugLog("IdleState");
    }

    public void Exit(BaseEnemy enemy)
    {
        enemy.animator.SetBool("Idle", false);
    }

    public void Update(BaseEnemy enemy)
    {
        if (enemy.FarDetect())
        {
            enemy.DebugLog("FarDetect");
            enemy.SetState(new ShootState());
        }
        else if (enemy.Detect())
        {
            enemy.SetState(new MeleeState());
        }
    }
}

public class ShootState : EnemyState
{
    public void Enter(BaseEnemy enemy)
    {
        enemy.animator.SetBool("Idle", true);
        enemy.DebugLog("ShootState");
    }

    public void Exit(BaseEnemy enemy)
    {
        enemy.animator.SetBool("Idle", false);
    }

    public void Update(BaseEnemy enemy)
    {
        if (!enemy.FarDetect() && !enemy.Detect() && !enemy.Near())
        {
            enemy.SetState(new IdleState());
        }
        else if (enemy.Detect())
        {
            enemy.SetState(new MeleeState());
        }
        else
        {
            enemy.Shoot();
        }
    }
}

public class MeleeState : EnemyState
{

    public void Enter(BaseEnemy enemy)
    {
        enemy.animator.SetBool("Running", true);
        enemy.DebugLog("MeleeState");
    }

    public void Exit(BaseEnemy enemy)
    {
        enemy.animator.SetBool("Running", false);
    }

    public void Update(BaseEnemy enemy)
    {
        if (enemy.FarDetect())
        {
            enemy.SetState(new ShootState());
        }
        else if (enemy.Near())
        {
            enemy.Follow();
            enemy.Attack();
        }
        else
        {
            enemy.Follow();
        }
    }
}


public class Damaged : EnemyState
{
    public void Enter(BaseEnemy enemy)
    {
        enemy.animator.SetTrigger("Damaged");
        enemy.DebugLog("Damaged");
    }

    public void Exit(BaseEnemy enemy)
    {

    }

    public void Update(BaseEnemy enemy)
    {

    }
}

public class Death : EnemyState
{
    public void Enter(BaseEnemy enemy)
    {
        enemy.Death();
        enemy.DebugLog("Death");
    }

    public void Exit(BaseEnemy enemy)
    {
    }

    public void Update(BaseEnemy enemy)
    {
    }
}