using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCellNode : ActionNode
{
    public CharacterBlock self;
    public Cell toCell;

    protected override void OnStart()
    {
        self = tree.AI.controlBlock;
        toCell = tree.AI.target.cell;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // Get direction from pathfinding
        Cell fromCell = self.cell;
        GridController gridController = self.gameManager.gridController;
        LevelPlane levelPlane = self.gameManager.levelPlane;
        CharacterPlane characterPlane = self.gameManager.characterPlane;
        ObjectPlane objectPlane = self.gameManager.objectPlane;
        GridDirection direction = GridPathfinding.GetImmediateDirection(fromCell, toCell, gridController, levelPlane, characterPlane, objectPlane);
        Debug.Log($"ImmediateDirection is {direction.direction}, current forwardDirection is {self.forwardDirection.direction}");
        // Get rotation by comparing
        if(direction == self.forwardDirection)
        {
            // Move forward
            Debug.Log("Node: Move forward");
            self.MoveFoward();
        }
        else if(direction == -self.forwardDirection.direction)
        {
            // backward, so turn left
            Debug.Log("Node: Rotate left");
            self.RotateHorizontally(Block.Rotations.Left);
        }
        else if(Vector3.SignedAngle(self.forwardDirection, direction, Vector3.up) > 0)
        {
            // turn left
            Debug.Log("Node: Rotate left");
            self.RotateHorizontally(Block.Rotations.Left);
        }
        else if(Vector3.SignedAngle(self.forwardDirection, direction, Vector3.up) < 0)
        {
            // turn right
            Debug.Log("Node: Rotate right");
            self.RotateHorizontally(Block.Rotations.Right);
        }
        else
        {
            // skip
            Debug.Log("Node: Skip action");
            self.SkipAction();
        }
        return State.Success;
    }
}
