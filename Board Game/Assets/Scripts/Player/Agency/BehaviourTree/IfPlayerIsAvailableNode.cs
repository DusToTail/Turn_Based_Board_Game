using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfPlayerIsAvailableNode : CompositeNode
{
    public PlayerController controller;

    protected override void OnStart()
    {
        controller = FindObjectOfType<PlayerController>();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (controller == null)
        {
            // There is no player
            Debug.Log("Node: There is no player");
            return children[0].Update();
        }
        else
        {
            // There is player
            Debug.Log("Node: There is player");
            return children[1].Update();
        }
    }
}
