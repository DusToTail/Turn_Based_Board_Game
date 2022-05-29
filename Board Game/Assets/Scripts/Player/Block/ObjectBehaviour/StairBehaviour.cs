using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairBehaviour : MonoBehaviour, IActivationOnStep
{
    public Vector3Int startCellGridPosition;
    public Vector3Int endCellGridPosition;

    public GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnStepped(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        StartCoroutine(MoveOnStairCoroutine(objectBlock, userBlock));
    }

    private IEnumerator MoveOnStairCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        Vector3Int directionV3Int = endCellGridPosition - startCellGridPosition;
        for(int i = 0; i < Mathf.Abs(directionV3Int.y); i++)
        {
            float t = 0;
            GridDirection direction = GridDirection.GetDirectionFromVector3Int(new Vector3Int(directionV3Int.x, 0, directionV3Int.z));
            Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(userBlock.cell,direction);
            if(directionV3Int.y > 0)
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Up);
            else if(directionV3Int.y < 0)
                toCell = gameManager.gridController.GetCellFromCellWithDirection(toCell, GridDirection.Down);

            userBlock.movementController.InitializeMovement(userBlock.transform, direction, userBlock.cell, toCell, BlockMovementController.MovementType.BasicHop);
            userBlock.CallOnPositionUpdated(toCell);

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
    }

}
