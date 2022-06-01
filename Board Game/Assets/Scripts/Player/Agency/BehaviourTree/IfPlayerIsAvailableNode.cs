using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfPlayerIsAvailableNode : CompositeNode
{
    public PlayerController controller;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (controller == null)
        {
            // There is no player
            return children[0].Update();
        }
        else
        {
            // There is player
            return children[1].Update();
        }
    }
}
