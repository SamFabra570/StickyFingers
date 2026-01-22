using UnityEngine;

public class EnemyScoutStateMachine
{
    public EnemyScoutState _CurrentState;

    public void ChangeState(EnemyScoutState newState)
    {
        _CurrentState.Exit();
        _CurrentState = newState;
        _CurrentState.Enter();
    }

    public void InitializeStateMachine(EnemyScoutState initialState)
    {
        _CurrentState = initialState;
        _CurrentState.Enter();
    }
}