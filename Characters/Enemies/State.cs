using UnityEngine;

public abstract class State
{
    protected bool _isComplete;
    protected float _startTime;
    public float _time => Time.time - _startTime;

    public virtual void Enter() { }
    public virtual void StateUpdate() { }
    public virtual void StateFixedUpdate() { }
    public virtual void Exit() { }

    public void Initialise()
    {
        _isComplete = false;
        _startTime = Time.time;
    }

    //getters and setters
    public bool GetIsComplete() 
    { 
        return _isComplete;
    }   
    protected void SetIsComplete(bool is_complete) 
    { 
        _isComplete = is_complete;
    }
}
