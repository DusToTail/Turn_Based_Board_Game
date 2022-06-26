using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object block in general is able to be interacted by players and does not necessarily have cube mesh
/// Can be added with object behaviours to add functionality (however, in general, limits to 1 behaviour per object)
/// </summary>
public class ObjectBlock : Block
{
    public GameObject activationBehaviour;
    public GameManager gameManager;
    public bool isPassable;
    public bool isFinished;

    private void OnEnable()
    {
        GameManager.OnBlockStartedBehaviour += ActivateBeforeBehaviourAtCurrentCell;
        GameManager.OnBlockEndedBehaviour += ActivateAfterBehaviourAtCurrentCell;

        GameManager.OnLevelFinished += StopBehaviour;
        GameManager.OnAITurnStarted += ResetFinishedState;
        GameManager.OnPlayerTurnStarted += ResetFinishedState;
    }

    private void OnDisable()
    {
        GameManager.OnBlockStartedBehaviour -= ActivateBeforeBehaviourAtCurrentCell;
        GameManager.OnBlockEndedBehaviour -= ActivateAfterBehaviourAtCurrentCell;

        GameManager.OnLevelFinished -= StopBehaviour;
        GameManager.OnAITurnStarted -= ResetFinishedState;
        GameManager.OnPlayerTurnStarted -= ResetFinishedState;

    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void ActivateAfterBehaviourAtCurrentCell(Block currentBlock)
    {
        if(currentBlock.cell.gridPosition != cell.gridPosition) { return; }
        if(currentBlock.GetType() != typeof(CharacterBlock)) { return; }
        Debug.Log($"{currentBlock.name} landed on {name} at cell {cell.gridPosition}");
        ActivateOnStepped((CharacterBlock)currentBlock);
    }

    public void ActivateBeforeBehaviourAtCurrentCell(Block currentBlock)
    {
        if (currentBlock.cell.gridPosition != cell.gridPosition) { return; }
        if (currentBlock.GetType() != typeof(CharacterBlock)) { return; }
        Debug.Log($"{currentBlock.name} landed on {name} at cell {cell.gridPosition}");
    }

    public void ActivateOnStepped(CharacterBlock userBlock)
    {
        if(activationBehaviour == null) { return; }
        if(activationBehaviour.GetComponent<IActivationOnStep>() != null)
            activationBehaviour.GetComponent<IActivationOnStep>().OnStepped(this, userBlock);
    }

    public void ActivateOnTriggered(CharacterBlock userBlock)
    {
        if (activationBehaviour == null) { return; }
        if (activationBehaviour.GetComponent<IActivationOnTrigger>() != null)
            activationBehaviour.GetComponent<IActivationOnTrigger>().OnTriggered(this, userBlock);
    }

    private void StopBehaviour() => StopAllCoroutines();
    private void ResetFinishedState() => isFinished = false;
}
