using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State 
    {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    public BehaviourTree tree;

    public State Update()
    {
        if(!started)
        {
            OnStart();
            started = true;
        }
        state = OnUpdate();

        if(state == State.Failure || state == State.Success)
        {
            OnStop();
            started = false;
        }

        return state;
    }

    public virtual void Destroy()
    {
        Destroy(this);
    }

    public virtual Node Clone(BehaviourTree tree)
    {
        Node node = Instantiate(this);
        node.tree = tree;
        return node;
    }
    public virtual bool SetTree(BehaviourTree tree)
    {
        this.tree = tree;
        return true;
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();


}
