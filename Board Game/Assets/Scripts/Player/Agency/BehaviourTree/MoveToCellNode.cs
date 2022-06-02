using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCellNode : ActionNode
{
    public CharacterBlock self;
    public Cell toCell;

    protected override void OnStart()
    {
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
        // Get rotation by comparing
        if(direction == self.forwardDirection)
        {
            // Move forward
            self.MoveFoward();
        }
        else if(direction == -self.forwardDirection.direction)
        {
            // backward, so turn left
            self.RotateHorizontally(Block.Rotations.Left);
        }
        else if(Vector3.SignedAngle(self.forwardDirection, direction, Vector3.up) > 0)
        {
            // turn left
            self.RotateHorizontally(Block.Rotations.Left);
        }
        else if(Vector3.SignedAngle(self.forwardDirection, direction, Vector3.up) < 0)
        {
            // turn right
            self.RotateHorizontally(Block.Rotations.Right);
        }
        else
        {
            // skip
            self.SkipAction();
        }
        return State.Success;
    }
}
