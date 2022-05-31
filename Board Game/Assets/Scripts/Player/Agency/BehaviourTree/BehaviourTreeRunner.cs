using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    private BehaviourTree _tree;

    private void Start()
    {
        _tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var log = ScriptableObject.CreateInstance<DebugLogNode>();
        log.message = "Hello World!";

        _tree.rootNode = log;
    }

    private void Update()
    {
        _tree.Update();
    }
}
