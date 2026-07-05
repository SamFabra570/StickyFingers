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

        bool sighted = enemy.sight_sensor_.detected_object_ != null;

        if (sighted)
        {
            Vector3 playerPos = enemy.sight_sensor_.detected_object_.transform.position;
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
                stateMachine.ChangeState(new EnemyAttackState(enemy, stateMachine, animationController, "Attack"));
            }
            return;
        }

        //Lost sight: keep heading to the last known position during the grace period, only fall back to Search after it expires.
        enemy.agent_.updateRotation = true;
        if (Time.time - enemy.lastSeenTime < enemy.loseSightGracePeriod)
        {
            enemy.agent_.isStopped = false;
            enemy.agent_.SetDestination(enemy.lastKnownPlayerPosition);
            return;
        }

        stateMachine.ChangeState(new EnemySearchState(enemy, stateMachine, animationController, "Search"));
    }

    //Rotate to keep the player in front — used during back-off so the vision cone never loses them.
    private void FacePlayer(Vector3 playerPos)
    {
        enemy.agent_.updateRotation = false;
        Vector3 faceDir = playerPos - enemy.transform.position;
        faceDir.y = 0f;
        if (faceDir.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(faceDir);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, target, Time.deltaTime * 8f);
        }
    }
}
