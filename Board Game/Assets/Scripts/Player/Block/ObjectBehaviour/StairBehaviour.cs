using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairBehaviour : MonoBehaviour, IActivationOnCollision
{
    public GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnCollision(ObjectBlock stairObject, CharacterBlock userBlock)
    {
        var useCaseInt = UsageCaseInt(stairObject, userBlock);
        switch(useCaseInt)
        {
            case 0:
                // Move Up
                stairObject.isFinished = false; // prevent character block's coroutine from ending
                StartCoroutine(UseStairCoroutine(stairObject, userBlock, true));
                break;
            case 1:
                // Move Down
                stairObject.isFinished = false; // prevent character block's coroutine from ending
                StartCoroutine(UseStairCoroutine(stairObject, userBlock, false));
                break;
            case -1:
                // Cant Use
                stairObject.isFinished = true;
                break;
            default:
                stairObject.isFinished = true;
                break;
        }
    }

    private IEnumerator UseStairCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock, bool moveUp)
    {
        ObjectBlock finalStair = objectBlock;
        Cell toCell = objectBlock.cell;
        while (finalStair != null && finalStair.activationBehaviour.GetComponent<StairBehaviour>() != null)
        {
            if (moveUp)
            {
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Up);
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, objectBlock.forwardDirection);
            }
            else
            {
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Down);
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, -objectBlock.forwardDirection);
            }
            finalStair = gameManager.objectPlane.GetBlockFromCell(toCell);
        }

        if (!moveUp)
            toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Up);

        GridDirection direction = moveUp? objectBlock.forwardDirection : -objectBlock.forwardDirection;
        userBlock.movementController.InitializeMovement(userBlock.transform, direction, userBlock.cell, toCell, BlockMovementController.MovementType.Slide);
        userBlock.CallOnPositionUpdated(toCell);

        if (userBlock.movementController.movementType == BlockMovementController.MovementType.BasicHop)
        {
            Transform one = userBlock.movementController.transform.GetChild(0);
            Transform second = userBlock.movementController.transform.GetChild(1);
            Transform third = userBlock.movementController.transform.GetChild(2);
            userBlock.animator.SetTrigger("Jump_L");
            userBlock.animator.SetFloat("Speed Multiplier", userBlock.movementController.speed);
            float t = 0;
            while (true)
            {
                yield return null;
                if (t > 1)
                {
                    MovementUtilities.MoveQuadraticBezierLerp(userBlock.transform, one, third, second, 1, true);
                    break;
                }
                t += Time.deltaTime * userBlock.movementController.speed;
                MovementUtilities.MoveQuadraticBezierLerp(userBlock.transform, one, third, second, t, true);
            }
            userBlock.animator.SetTrigger("Idle");
        }
        else if (userBlock.movementController.movementType == BlockMovementController.MovementType.Slide)
        {
            Transform one = userBlock.movementController.transform.GetChild(0);
            Transform second = userBlock.movementController.transform.GetChild(1);
            userBlock.animator.SetTrigger("Move");
            userBlock.animator.SetFloat("Speed Multiplier", userBlock.movementController.speed);
            float t = 0;
            while (true)
            {
                yield return null;
                if (t > 1)
                {
                    MovementUtilities.MoveLinearLerp(userBlock.transform, one, second, 1, true);
                    break;
                }
                t += Time.deltaTime * userBlock.movementController.speed;
                MovementUtilities.MoveLinearLerp(userBlock.transform, one, second, t, true);
            }
            userBlock.animator.SetTrigger("Idle");
        }
        Debug.Log($"{userBlock.name} jumped to {toCell.gridPosition} by stair");
        yield return ExistsOnStepObjectCoroutine(toCell);
        objectBlock.isFinished = true;
    }


    public Cell GetCellAtOtherEnd(ObjectBlock stairBlock, Cell thisEnd)
    {
        Cell upCell = gameManager.gridController.GetCellFromCellWithDirection(stairBlock.cell, GridDirection.Up);
        upCell = gameManager.gridController.GetCellFromCellWithDirection(upCell, stairBlock.forwardDirection);
        Cell downCell = gameManager.gridController.GetCellFromCellWithDirection(stairBlock.cell, -stairBlock.forwardDirection);
        if(thisEnd == upCell) { return downCell; }
        else if(thisEnd == downCell) { return upCell; }
        throw new System.Exception($"Stair block get wrong cell. Stair: {stairBlock.cell.gridPosition} / Up: {upCell.gridPosition} / Down: {downCell.gridPosition} / this end: {thisEnd.gridPosition}");
    }

    public int UsageCaseInt(ObjectBlock stairObject, CharacterBlock userBlock)
    {
        if (stairObject.forwardDirection.Equals(userBlock.forwardDirection))
            return 0;
        else if (stairObject.forwardDirection.Equals(-userBlock.forwardDirection))
            return 1;

        return -1;
    }

    public int UsageCaseInt(ObjectBlock stairObject, Cell checkCell)
    {
        var towardDirection = GridDirection.GetDirectionFromVector3Int(stairObject.cell.gridPosition - checkCell.gridPosition);
        if (stairObject.forwardDirection.Equals(towardDirection))
            return 0;
        else if (stairObject.forwardDirection.Equals(-towardDirection))
            return 1;

        return -1;
    }

    private IEnumerator ExistsOnStepObjectCoroutine(Cell cell)
    {
        ObjectBlock onStepObject = gameManager.objectPlane.GetBlockFromCell(cell);
        if (onStepObject != null && onStepObject.activationBehaviour.GetComponent<IActivationOnStep>() != null)
        {
            Debug.Log($"{gameObject.name} waiting for {onStepObject.name} to finish");
            yield return new WaitUntil(() => onStepObject.isFinished == true);
            yield break;
        }
        Debug.Log($"There is no object at {cell.gridPosition} to wait");
    }
}
