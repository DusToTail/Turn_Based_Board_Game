using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBlock : Block
{
    public GameObject activationBehaviour;
    public bool isFinished;

    private void OnEnable()
    {
        GameManager.OnBlockStartedBehaviour += ActivateBefore;
        GameManager.OnBlockEndedBehaviour += ActivateAfter;
    }

    private void OnDisable()
    {
        GameManager.OnBlockStartedBehaviour -= ActivateBefore;
        GameManager.OnBlockEndedBehaviour -= ActivateAfter;
    }

    public void ActivateAfter(Block currentBlock)
    {
        if(currentBlock.cell.gridPosition != cell.gridPosition) { return; }
        if(currentBlock.GetType() != typeof(CharacterBlock)) { return; }
        Debug.Log($"{currentBlock.name} landed on {name} at cell {cell.gridPosition}");
        Activate((CharacterBlock)currentBlock);
    }

    public void ActivateBefore(Block currentBlock)
    {
        if (currentBlock.cell.gridPosition != cell.gridPosition) { return; }
        if (currentBlock.GetType() != typeof(CharacterBlock)) { return; }
        Debug.Log($"{currentBlock.name} landed on {name} at cell {cell.gridPosition}");

    }

    public void Activate(CharacterBlock userBlock)
    {
        if(activationBehaviour == null) { return; }
        if(activationBehaviour.GetComponent<IActivationOnStep>() == null) { return; }
        activationBehaviour.GetComponent<IActivationOnStep>().OnStepped(this, userBlock);
    }
}
