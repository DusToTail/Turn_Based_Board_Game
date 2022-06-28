using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteTriggerBehaviour : MonoBehaviour, IRemoteActivationOnTrigger
{
    public GameManager gameManager;
    public ObjectBlock toBeTriggeredBlock;
    public AudioHandler audioHandler;
    public IRemoteActivation toBeTriggered { get; set; }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        toBeTriggered = toBeTriggeredBlock.activationBehaviour.GetComponent<IRemoteActivation>();
    }

    public void OnTriggered(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        objectBlock.isFinished = false;
        StartCoroutine(TriggerObjectCoroutine(objectBlock, userBlock));
    }

    private IEnumerator TriggerObjectCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        audioHandler.Play("Trigger");
        yield return new WaitForSeconds(1);
        toBeTriggered.OnTriggered(toBeTriggeredBlock, objectBlock, userBlock);

        Debug.Log($"{objectBlock.name} is triggered. Waiting for {toBeTriggeredBlock.name} to be triggered");
        yield return new WaitUntil(() => toBeTriggeredBlock.isFinished);
        objectBlock.isFinished = true;
    }
}
