using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> children = new List<Node>();

    public override Node Clone(BehaviourTree tree)
    {
        CompositeNode node = Instantiate(this);
        node.tree = tree;
        node.children = children.ConvertAll(c => c.Clone(tree));
        return node;
    }
    public override void Destroy()
    {
        foreach (var child in children) { child.Destroy(); }
        Destroy(this);
    }
}
