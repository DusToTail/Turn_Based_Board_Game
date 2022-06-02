using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNode : ActionNode
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
        Debug.Log("Node: Attack forward");
        self.Attack();
        return State.Success;
    }
}
