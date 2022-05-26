using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A class that directly moves blocks (specifically characters) on the level plane
/// </summary>
public class BlockMovementController : MonoBehaviour
{
    public Transform[] transforms;
    public float upwardOffset;
    public MovementType movementType;
    public float speed;
    public GameObject currentMoveObject;
    [SerializeField] private bool displayGizmos;

    public enum MovementType
    {
        BasicHop,
        Slide,
    }

    public void InitializeMovement(Transform currentTransform, GridDirection direction, Cell fromCell, Cell toCell, MovementType type)
    {
        if(currentTransform == null) { Debug.Log("Transform to be moved is missing"); return; }
        if (direction == null) { Debug.Log("Grid Direction is missing"); return; }
        if (fromCell == null) { Debug.Log("From Cell is missing"); return; }
        if (toCell == null) { Debug.Log("To Cell is missing"); return; }
        movementType = type;
        currentMoveObject = currentTransform.gameObject;
        if (type == MovementType.BasicHop)
        {
            // Use QuadraticBezierLerp
            transforms[0].position = currentTransform.position;
            transforms[0].rotation = currentTransform.rotation;

            transforms[2].position = toCell.worldPosition;
            transforms[2].rotation = Quaternion.LookRotation(direction, currentTransform.up);

            transforms[1].position = fromCell.worldPosition + (toCell.worldPosition - fromCell.worldPosition) / 2 + currentTransform.up * upwardOffset;
            transforms[1].rotation = Quaternion.Lerp(transforms[2].rotation, transforms[0].rotation, 0.5f);

        }
        else if (type == MovementType.Slide)
        {
            // Use LinearLerp
            transforms[0].position = currentTransform.position;
            transforms[0].rotation = currentTransform.rotation;

            transforms[1].position = toCell.worldPosition;
            transforms[1].rotation = Quaternion.LookRotation(toCell.worldPosition - fromCell.worldPosition, currentTransform.up);

        }

    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }
        Gizmos.color = Color.magenta;
        for (int i = 0; i < transforms.Length; i++)
            Gizmos.DrawIcon(transforms[i].position, transforms[i].name, true);

    }
}
