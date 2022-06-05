using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelBehaviour : MonoBehaviour, IActivationOnTrigger
{
    public int levelIndex = 0;
    public GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnTriggered(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        StartCoroutine(StartNewGameCoroutine(objectBlock, userBlock));
    }

    private IEnumerator StartNewGameCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        // Trigger sound effect and loading screen animation
        // Wait for the screen animation to fully cover the entire screen
        yield return new WaitForSeconds(1f);

        // Trigger game loading
        gameManager.CallLevelLoadingStarted(levelIndex);
        Debug.Log($"{objectBlock} called level {levelIndex} loading started");



    }
}
