using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void RunTree()
    {
        tree = tree.Clone();
        tree.Update();
    }
}
