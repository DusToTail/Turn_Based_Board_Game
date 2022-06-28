using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteDoorBehaviour : MonoBehaviour, IRemoteActivation
{
    public GameManager gameManager;
    public AudioHandler audioHandler;
    public GameObject model;
    public bool isOpen;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnTriggered(ObjectBlock self, ObjectBlock trigger, CharacterBlock userBlock)
    {
        self.isFinished = false;
        if(isOpen)
            StartCoroutine(CloseDoor(self, trigger, userBlock));
        else
            StartCoroutine(OpenDoor(self, trigger, userBlock));
    }

    public void SetPassableBool(bool isPassable)
    {
        GetComponentInParent<ObjectBlock>().isPassable = isPassable;
        isOpen = isPassable;
    }

    private IEnumerator OpenDoor(ObjectBlock self, ObjectBlock trigger, CharacterBlock userBlock)
    {
        // Trigger animation
        audioHandler.Play("Open");
        yield return new WaitForSeconds(3);
        model.SetActive(false);
        SetPassableBool(true);
        yield return null;
        self.isFinished = true;
    }

    private IEnumerator CloseDoor(ObjectBlock self, ObjectBlock trigger, CharacterBlock userBlock)
    {
        // Trigger animation
        audioHandler.Play("Close");
        yield return new WaitForSeconds(3);
        model.SetActive(true);
        SetPassableBool(false);
        yield return null;
        self.isFinished = true;
    }

    
}
