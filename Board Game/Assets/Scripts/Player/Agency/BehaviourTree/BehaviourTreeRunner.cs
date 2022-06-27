using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    public BehaviourTree _clone;
    private bool _characterIsFinished = false;

    private void OnEnable()
    {
        GameManager.OnCharacterRanOutOfMoves += SetBehaviourIsFinished;
        GameManager.OnNextMoveRequired += SetBehaviourIsFinished;
    }

    private void OnDisable()
    {
        GameManager.OnCharacterRanOutOfMoves -= SetBehaviourIsFinished;
        GameManager.OnNextMoveRequired -= SetBehaviourIsFinished;
    }

    public IEnumerator RunTree()
    {
        Debug.Log($"Start run tree");
        _characterIsFinished = false;
        InitializeClone();
        _clone.Update();
        yield return new WaitUntil(() => _characterIsFinished == true);
        DestroyClone();
    }

    private void SetBehaviourIsFinished(CharacterBlock user)
    {
        if(user == GetComponent<AIController>().controlBlock)
            _characterIsFinished = true;
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
