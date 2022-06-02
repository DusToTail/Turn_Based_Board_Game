using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    public BehaviourTree _clone;

    private void Start()
    {
        
    }

    private void Update()
    {
    }

    public void RunTree()
    {
        Debug.Log($"Start run tree");
        InitializeClone();
        _clone.Update();
        DestroyClone();
    }

    private void InitializeClone()
    {
        _clone = tree.Clone();
        _clone.SetAI(GetComponent<AIController>());
    }

    private void DestroyClone()
    {
        Destroy(_clone.rootNode);
        Destroy(_clone);
    }

}
