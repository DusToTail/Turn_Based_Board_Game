using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    private int _current;
    protected override void OnStart()
    {
        _current = 0;
    }

    protected override void OnStop()
    {
        throw new System.NotImplementedException();
    }

    protected override State OnUpdate()
    {
        var child = children[_current];
        switch(child.Update())
        {
            case State.Running:
                return State.Running;

            case State.Failure:
                return State.Failure;

            case State.Success:
                _current++;
                break;  
        }

        return _current == children.Count ? State.Success : State.Failure;
    }
}
