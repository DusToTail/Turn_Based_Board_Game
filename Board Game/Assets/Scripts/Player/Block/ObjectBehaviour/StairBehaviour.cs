using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairBehaviour : MonoBehaviour, IActivationOnStep
{
    public Block startBlock;
    public Block endBlock;

    public GameManager gameManager;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnStepped(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        Vector3Int directionV3Int = (endBlock.cell.gridPosition - startBlock.cell.gridPosition);
        //Debug.Log($"End to Start V3Int {directionV3Int}");
        if(directionV3Int.magnitude == 0) 
        {
            gameManager.CallBlockEndedBehaviour(objectBlock);
            objectBlock.isFinished = true;
            return;
        }
        Vector3Int normalizedDirection = new Vector3Int(directionV3Int.x, 0, directionV3Int.z);
        normalizedDirection = normalizedDirection / (int)normalizedDirection.magnitude;
        //Debug.Log($"End to Start NormalizedV3Int {normalizedDirection}");

        GridDirection direction = GridDirection.GetDirectionFromVector3Int(new Vector3Int(normalizedDirection.x, 0, normalizedDirection.z));
        //Debug.Log($"Direction: {direction.direction}");
        //Debug.Log($"User Forward Direction: {userBlock.forwardDirection.direction}");
        if (!direction.Equals(userBlock.forwardDirection)) 
        {
            gameManager.CallBlockEndedBehaviour(objectBlock);
            objectBlock.isFinished = true;
            return; 
        }
        objectBlock.isFinished = false;

        StartCoroutine(MoveOnStairCoroutine(objectBlock, userBlock));
    }

    private IEnumerator MoveOnStairCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        Vector3Int directionV3Int = (endBlock.cell.gridPosition - startBlock.cell.gridPosition);
        //Debug.Log($"End to Start V3Int {directionV3Int}");

        for (int i = 0; i < Mathf.Abs(directionV3Int.y); i++)
        {
            //Debug.Log($"{userBlock.name} before position: {userBlock.cell.gridPosition}");
            float t = 0;
            Vector3Int normalizedDirection = new Vector3Int(directionV3Int.x, 0, directionV3Int.z);
            normalizedDirection = normalizedDirection / (int)normalizedDirection.magnitude;
            GridDirection direction = GridDirection.GetDirectionFromVector3Int(new Vector3Int(normalizedDirection.x, 0, normalizedDirection.z));
            //Debug.Log($"End to Start {direction.direction}");
            Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(userBlock.cell,direction);
            if(directionV3Int.y > 0)
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Up);
            else if(directionV3Int.y < 0)
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Down);
            //Debug.Log($"ToCell of {userBlock.name} is: {toCell.gridPosition}");

            userBlock.movementController.InitializeMovement(userBlock.transform, direction, userBlock.cell, toCell, BlockMovementController.MovementType.BasicHop);
            userBlock.CallOnPositionUpdated(toCell);
            //Debug.Log($"{userBlock.name} after position: {userBlock.cell.gridPosition}");

            if (userBlock.movementController.movementType == BlockMovementController.MovementType.BasicHop)
            {
                Transform one = userBlock.movementController.transform.GetChild(0);
                Transform second = userBlock.movementController.transform.GetChild(1);
                Transform third = userBlock.movementController.transform.GetChild(2);

                while (true)
                {
                    yield return null;
                    if (t >= 1)
                    {
                        t = 1;
                        MovementUtilities.QuadraticBezierLerp(userBlock.transform, one, third, second, t, true);
                        break;
                    }
                    t += Time.deltaTime * userBlock.movementController.speed * (1 + t);
                    MovementUtilities.QuadraticBezierLerp(userBlock.transform, one, third, second, t, true);
                }
            }
            else if (userBlock.movementController.movementType == BlockMovementController.MovementType.Slide)
            {
                Transform one = userBlock.movementController.transform.GetChild(0);
                Transform second = userBlock.movementController.transform.GetChild(1);
                while (true)
                {
                    yield return null;
                    if (t >= 1)
                    {
                        t = 1;
                        MovementUtilities.LinearLerp(userBlock.transform, one, second, t, true);
                        break;
                    }
                    t += Time.deltaTime * userBlock.movementController.speed * (1 + t);
                    MovementUtilities.LinearLerp(userBlock.transform, one, second, t, true);
                }
            }

            Debug.Log($"{userBlock.name} jumped to {toCell.gridPosition} by stair");
            
        }

        gameManager.CallBlockEndedBehaviour(objectBlock);
        objectBlock.isFinished = true;
    }

}
