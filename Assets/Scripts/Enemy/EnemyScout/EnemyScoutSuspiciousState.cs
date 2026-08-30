using UnityEngine;
using UnityEngine.AI;

//The rung of the ladder that did not exist. Before this, a guard went from whistling on his route to
//full pursuit in a single frame, because the only question ever asked was "is the ray clear right now".
//
//Suspicious is what a person actually does with a half-signal: stop, turn towards it, look. If the
//feeling firms up, commit. If it does not, shrug and walk on. The player gets a window to react, and
//the guard stops reading as omniscient.
public class EnemyScoutSuspiciousState : EnemyScoutState
{
    //How long the guard stands and looks before deciding to walk over and check.
    private const float LookDuration = 1.25f;

    private bool _walking;

    public EnemyScoutSuspiciousState(BaseScoutEnemy _enemy, EnemyScoutStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fireEffect.SetActive(false);
        _walking = false;

        //Stand still for the first beat. Taking rotation off the agent lets us turn towards the stimulus
        //without the NavMeshAgent fighting us for the transform.
        enemy.agent_.isStopped = true;
        enemy.agent_.updateRotation = false;
    }

    public override void Exit()
    {
        base.Exit();

        //Always hand rotation and movement back, whichever branch we leave through.
        enemy.agent_.updateRotation = true;
        enemy.agent_.isStopped = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        EnemyPerception perception = enemy.perception;

        if (perception == null)
        {
            stateMachine.ChangeState(enemy.patrolState);
            return;
        }

        //Certainty won — commit to the chase.
        if (perception.Level == EnemyPerception.Awareness.Alert)
        {
            stateMachine.ChangeState(enemy.pursuitState);
            return;
        }

        //Whatever it was, it did not hold up. Note this is the perception's decision, not a timer:
        //awareness had to decay all the way back past the exit threshold to get here.
        if (perception.Level == EnemyPerception.Awareness.Unaware)
        {
            enemy.currentTarget = null;
            stateMachine.ChangeState(enemy.patrolState);
            return;
        }

        if (!perception.HasLastKnownPosition)
            return;

        FaceTarget(perception.LastKnownPosition);

        //Beat two: walk over and have a proper look.
        if (!_walking && Time.time - startTime >= LookDuration)
        {
            _walking = true;
            enemy.agent_.updateRotation = true;

            //Only investigate somewhere actually reachable — never path into a wall and grind on it.
            if (NavMesh.SamplePosition(perception.LastKnownPosition, out NavMeshHit navHit, 3.0f, NavMesh.AllAreas))
            {
                enemy.agent_.isStopped = false;
                enemy.agent_.SetDestination(navHit.position);
            }
        }
    }

    //Turn to face the stimulus during the looking beat. Once we start walking the agent steers again.
    private void FaceTarget(Vector3 position)
    {
        if (_walking)
            return;

        Vector3 direction = position - enemy.transform.position;
        direction.y = 0.0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion target = Quaternion.LookRotation(direction);
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, target, Time.deltaTime * 6.0f);
    }
}
