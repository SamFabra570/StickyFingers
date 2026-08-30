using UnityEngine;
using UnityEngine.AI;

public class EnemyPursuitState : EnemyState
{
    private float distanceToTarget;

    public EnemyPursuitState(BaseEnemy _enemy, EnemyStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.fireEffect.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        //The back-off phase turns rotation control off — always hand the agent back with it restored.
        enemy.agent_.updateRotation = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        EnemyPerception perception = enemy.perception;

        if (perception == null)
        {
            //Should never happen — the enemy builds one in Awake. Fail towards searching, not freezing.
            stateMachine.ChangeState(enemy.searchState);
            return;
        }

        //Eyes-on. The raw sensor is the right source for WHERE, but only once perception has already
        //decided we are committed: deciding and measuring are different jobs and must not share a source.
        if (perception.HasVisual)
        {
            Vector3 playerPos = perception.VisualTarget.transform.position;
            enemy.lastKnownPlayerPosition = playerPos;
            enemy.lastSeenTime = Time.time;
            distanceToTarget = Vector3.Distance(enemy.transform.position, playerPos);

            bool cooldownActive = Time.time < enemy.lastAttackTime + enemy.attackCooldown;

            //After a hit, give the (frozen) player room to escape: back AWAY while still FACING them, so we
            //never turn our back (which would drop line of sight and make us "give up" mid-fight).
            if (cooldownActive && distanceToTarget <= enemy.attack_distance_)
            {
                FacePlayer(playerPos);

                Vector3 away = enemy.transform.position - playerPos;
                away = away.sqrMagnitude < 0.0001f ? -enemy.transform.forward : away.normalized;
                Vector3 desired = enemy.transform.position + away * enemy.postAttackBackoffDistance;

                //Only retreat to somewhere actually on the NavMesh — never path into a wall and grind against it.
                if (NavMesh.SamplePosition(desired, out NavMeshHit navHit, enemy.postAttackBackoffDistance, NavMesh.AllAreas))
                {
                    enemy.agent_.isStopped = false;
                    enemy.agent_.SetDestination(navHit.position);
                }
                else
                {
                    //Cornered against geometry: hold position instead of vibrating against the wall.
                    enemy.agent_.isStopped = true;
                }

                return;
            }

            //Approaching / attacking: let the agent steer (and rotate) toward the player normally.
            enemy.agent_.updateRotation = true;
            enemy.agent_.isStopped = false;
            enemy.agent_.SetDestination(playerPos);

            if (!cooldownActive && distanceToTarget <= enemy.attack_distance_)
            {
                stateMachine.ChangeState(enemy.attackState);
            }

            return;
        }

        //Lost visual. Whether we give up is perception's call now, not a stopwatch: awareness holds flat
        //through decayDelay, then falls, and cannot drop below the post-alert floor for a while. THAT is
        //what stops "duck behind a wall for two seconds" from being a complete reset.
        enemy.agent_.updateRotation = true;

        if (perception.Level == EnemyPerception.Awareness.Alert)
        {
            if (perception.HasLastKnownPosition)
            {
                enemy.lastKnownPlayerPosition = perception.LastKnownPosition;
                enemy.agent_.isStopped = false;
                enemy.agent_.SetDestination(perception.LastKnownPosition);
            }

            return;
        }

        stateMachine.ChangeState(enemy.searchState);
    }

    //Rotate to keep the player in front — used during back-off so the vision cone never loses them.
    private void FacePlayer(Vector3 playerPos)
    {
        enemy.agent_.updateRotation = false;

        Vector3 faceDir = playerPos - enemy.transform.position;
        faceDir.y = 0.0f;

        if (faceDir.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(faceDir);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, target, Time.deltaTime * 8.0f);
        }
    }
}
