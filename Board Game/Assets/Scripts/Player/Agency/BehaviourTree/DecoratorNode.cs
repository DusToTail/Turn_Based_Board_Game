using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecoratorNode : Node
{
    [HideInInspector] public Node child;

    public override Node Clone(BehaviourTree tree)
    {
        DecoratorNode node = Instantiate(this);
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
