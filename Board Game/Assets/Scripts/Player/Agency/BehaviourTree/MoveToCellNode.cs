using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCellNode : ActionNode
{
    public CharacterBlock self;
    public Cell toCell;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // Get Pathfinding and move using the direction

        return State.Success;
    }
}
