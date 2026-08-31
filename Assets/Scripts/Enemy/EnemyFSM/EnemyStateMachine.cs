using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState _CurrentState;

    public void ChangeState(EnemyState newState)
    {
        //States are shared instances owned by the enemy, so asking to re-enter the state we are already
        //in would fire Exit()+Enter() on the SAME object and flicker its animator bool for a frame.
        //Doing nothing is the correct response to "go to where you already are".
        if (newState == null || newState == _CurrentState)
            return;

        _CurrentState?.Exit();
        _CurrentState = newState;
        _CurrentState.Enter();
    }

    public void InitializeStateMachine(EnemyState initialState)
    {
        if (initialState == null)
            return;

        _CurrentState = initialState;
        _CurrentState.Enter();
    }
}
