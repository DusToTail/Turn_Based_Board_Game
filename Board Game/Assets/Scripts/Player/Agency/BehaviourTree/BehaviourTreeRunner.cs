using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    private BehaviourTree _clone;

    private void Start()
    {
        _clone = tree.Clone();
    }

    private void Update()
    {
    }

    public void RunTree()
    {
        _clone.Update();
    }

    private void OnDestroy()
    {
        Destroy(_clone);
    }
}
