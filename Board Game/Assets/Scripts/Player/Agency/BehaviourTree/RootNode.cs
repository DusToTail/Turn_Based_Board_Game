using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    [HideInInspector] public Node child;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return child.Update();
    }

    public override Node Clone(BehaviourTree tree)
    {
        RootNode node = Instantiate(this);
        node.tree = tree;
        node.child = child.Clone(tree);
        return node;
    }

    public override void Destroy()
    {
        child.Destroy();
        Destroy(this);
    }
}
