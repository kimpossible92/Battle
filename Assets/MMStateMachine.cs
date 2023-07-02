using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MMStateChangeEvent<T> where T : struct, IComparable, IConvertible, IFormattable
{
    public GameObject Target;
    public MMStateMachine<T> TargetStateMachine;
    public T NewState;
    public T PreviousState;

    public MMStateChangeEvent(MMStateMachine<T> stateMachine)
    {
        Target = stateMachine.Target;
        TargetStateMachine = stateMachine;
        NewState = stateMachine.CurrentState;
        PreviousState = stateMachine.PreviousState;
    }
}
public interface MMIStateMachine
{
    bool TriggerEvents { get; set; }
}
public class MMStateMachine<T> : MMIStateMachine where T : struct, IComparable, IConvertible, IFormattable
{
    public bool TriggerEvents { get; set; }
    public GameObject Target;
    public T CurrentState { get; protected set; }
    public T PreviousState { get; protected set; }
    public MMStateMachine(GameObject target, bool triggerEvents)
    {
        this.Target = target;
        this.TriggerEvents = triggerEvents;
    }
    public virtual void ChangeState(T newState)
    {
        if (newState.Equals(CurrentState))
        {
            return;
        }
        PreviousState = CurrentState;
        CurrentState = newState;

        if (TriggerEvents)
        {
            MMEventManager.TriggerEvent(new MMStateChangeEvent<T>(this));
        }
    }
    public virtual void RestorePreviousState()
    {
        CurrentState = PreviousState;

        if (TriggerEvents)
        {
            MMEventManager.TriggerEvent(new MMStateChangeEvent<T>(this));
        }
    }
}
