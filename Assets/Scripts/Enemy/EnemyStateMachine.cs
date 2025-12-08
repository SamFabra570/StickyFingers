using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState _CurrentState;

    public void ChangeState(EnemyState newState)
    {
        _CurrentState.Exit();
        _CurrentState = newState;
        _CurrentState.Enter();
    }

    public void InitializeStateMachine(EnemyState initialState)
    {
        _CurrentState = initialState;
        _CurrentState.Enter();
    }
}
