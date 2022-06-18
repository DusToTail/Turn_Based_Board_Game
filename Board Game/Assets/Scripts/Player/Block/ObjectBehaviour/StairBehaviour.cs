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
        // Validation check
        Vector3Int directionV3Int = (endBlock.cell.gridPosition - startBlock.cell.gridPosition);
        if(directionV3Int.magnitude == 0) 
        {
            gameManager.CallBlockEndedBehaviour(objectBlock);
            objectBlock.isFinished = true;
            return;
        }
        Vector3Int normalizedDirection = new Vector3Int(directionV3Int.x, 0, directionV3Int.z);
        normalizedDirection = normalizedDirection / (int)normalizedDirection.magnitude;

        GridDirection direction = GridDirection.GetDirectionFromVector3Int(new Vector3Int(normalizedDirection.x, 0, normalizedDirection.z));
        if (!direction.Equals(userBlock.forwardDirection)) 
        {
            gameManager.CallBlockEndedBehaviour(objectBlock);
            objectBlock.isFinished = true;
            return; 
        }

        // Start behaviour
        objectBlock.isFinished = false; // prevent character block's coroutine from ending
        StartCoroutine(MoveOnStairCoroutine(objectBlock, userBlock));
    }

    private IEnumerator MoveOnStairCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        Vector3Int directionV3Int = (endBlock.cell.gridPosition - startBlock.cell.gridPosition);

        for (int i = 0; i < Mathf.Abs(directionV3Int.y); i++)
        {
            float t = 0;
            Vector3Int normalizedDirection = new Vector3Int(directionV3Int.x, 0, directionV3Int.z);
            normalizedDirection = normalizedDirection / (int)normalizedDirection.magnitude;
            GridDirection direction = GridDirection.GetDirectionFromVector3Int(new Vector3Int(normalizedDirection.x, 0, normalizedDirection.z));
            Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(userBlock.cell,direction);
            if(directionV3Int.y > 0)
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Up);
            else if(directionV3Int.y < 0)
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Down);

            // Initialize trajectory
            userBlock.movementController.InitializeMovement(userBlock.transform, direction, userBlock.cell, toCell, BlockMovementController.MovementType.Slide);
            // Update position in character plane
            userBlock.CallOnPositionUpdated(toCell);

            if (userBlock.movementController.movementType == BlockMovementController.MovementType.BasicHop)
            {
                // Initialize local trajectory to be used
                Transform one = userBlock.movementController.transform.GetChild(0);
                Transform second = userBlock.movementController.transform.GetChild(1);
                Transform third = userBlock.movementController.transform.GetChild(2);
                if(i % 1 == 0)
                    userBlock.animator.SetTrigger("Jump_L");
                else
                    userBlock.animator.SetTrigger("Jump_R");
                userBlock.animator.SetFloat("Speed Multiplier", userBlock.movementController.speed);
                
                while (true)
                {
                    yield return null;
                    if (t >= 1)
                    {
                        t = 1;
                        MovementUtilities.MoveQuadraticBezierLerp(userBlock.transform, one, third, second, t, true);
                        break;
                    }
                    t += Time.deltaTime * userBlock.movementController.speed;
                    MovementUtilities.MoveQuadraticBezierLerp(userBlock.transform, one, third, second, t, true);
                }
                userBlock.animator.SetTrigger("Idle");
            }
            else if (userBlock.movementController.movementType == BlockMovementController.MovementType.Slide)
            {
                // Initialize local trajectory to be used
                Transform one = userBlock.movementController.transform.GetChild(0);
                Transform second = userBlock.movementController.transform.GetChild(1);
                userBlock.animator.SetTrigger("Move");
                userBlock.animator.SetFloat("Speed Multiplier", userBlock.movementController.speed);
                while (true)
                {
                    yield return null;
                    if (t >= 1)
                    {
                        t = 1;
                        MovementUtilities.MoveLinearLerp(userBlock.transform, one, second, t, true);
                        break;
                    }
                    t += Time.deltaTime * userBlock.movementController.speed;
                    MovementUtilities.MoveLinearLerp(userBlock.transform, one, second, t, true);
                }
                userBlock.animator.SetTrigger("Idle");
            }

            Debug.Log($"{userBlock.name} jumped to {toCell.gridPosition} by stair");
            
        }

        gameManager.CallBlockEndedBehaviour(objectBlock);
        objectBlock.isFinished = true;
    }

}
