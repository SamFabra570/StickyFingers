using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(BaseEnemy _enemy, EnemyStateMachine _stateMachine, Animator _animController, string _animName)
        : base(_enemy, _stateMachine, _animController, _animName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent_.stoppingDistance = enemy.attack_distance_;
        enemy.agent_.isStopped = true;
        enemy.fireEffect.SetActive(false);

        TryHit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //Hold in Attack for the anim duration so the player sees the strike, then hand control back to Pursuit. Pursuit handles the cooldown/back-off from there.
        if (Time.time - startTime >= enemy.attackAnimHold)
        {
            stateMachine.ChangeState(new EnemyPursuitState(enemy, stateMachine, animationController, "Pursuit"));
        }
    }

    private void TryHit()
    {
        if (enemy.sight_sensor_.detected_object_ == null)
            return;

        float distance = Vector3.Distance(enemy.transform.position, enemy.sight_sensor_.detected_object_.transform.position);
        if (distance > enemy.attack_distance_)
            return;

        PlayerController player = enemy.sight_sensor_.detected_object_.GetComponentInParent<PlayerController>();
        if (player == null)
            return;

        player.FreezeMovement(2);

        InventorySystem inventory = GameObject.Find("InventoryContainer").GetComponent<InventoryContainer>().inventorySystem;
        InventoryItemData stolen = PickRandomItem(inventory);
        if (stolen != null)
        {
            inventory.Remove(stolen, PopupUI.PopupType.Stolen);
            //UIManager.Instance.ShowItemStolen(stolen);
        }

        enemy.lastAttackTime = Time.time;
    }

    //Picks a random item currently held in the inventory, or null if empty.
    private InventoryItemData PickRandomItem(InventorySystem inventory)
    {
        if (inventory == null || inventory.inventory == null || inventory.inventory.Count == 0)
            return null;

        int idx = Random.Range(0, inventory.inventory.Count);
        return inventory.inventory[idx]?.data;
    }
}
