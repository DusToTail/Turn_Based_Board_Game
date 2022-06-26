using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBehaviour : MonoBehaviour, IActivationOnTrigger
{
    public GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnTriggered(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        objectBlock.isFinished = false;
        StartCoroutine(GoalEventCoroutine(objectBlock, userBlock));
    }

    private IEnumerator GoalEventCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        // trigger some ending scene before level finished
        yield return new WaitForSeconds(1);
        gameManager.CallBlockEndedBehaviour(objectBlock);
        objectBlock.isFinished = true;
        gameManager.CallLevelFinished();
    }

    
}
