using UnityEngine;

public class EnemyMageStateMachine
{
    public EnemyMageState _CurrentState;

    public void ChangeState(EnemyMageState newState)
    {
        _CurrentState.Exit();
        _CurrentState = newState;
        _CurrentState.Enter();
    }

    public void InitializeStateMachine(EnemyMageState initialState)
    {
        _CurrentState = initialState;
        _CurrentState.Enter();
    }
}
