public class EnemyStateMachine 
{
    public EnemyState _state { get; private set; }

    public void SetState(EnemyState new_state, bool force_reset = false)
    {
        if (_state != new_state || force_reset)
        {
            if (_state != null)
            {
                _state.Exit();
            }
            _state = new_state;
            _state.Initialise();
            _state.Enter();
        }
    }
}
