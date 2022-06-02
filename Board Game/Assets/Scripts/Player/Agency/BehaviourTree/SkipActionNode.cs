using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipActionNode : ActionNode
{
    public CharacterBlock self;

    protected override void OnStart()
    {
        self = tree.AI.controlBlock;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Debug.Log("Node: Skip action");
        self.SkipAction();
        return State.Success;
    }

}
