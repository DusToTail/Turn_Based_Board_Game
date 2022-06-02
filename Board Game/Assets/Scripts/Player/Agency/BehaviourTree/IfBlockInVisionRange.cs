using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfBlockInVisionRange : CompositeNode
{
    public CharacterBlock self;
    public Block target;
    public int visionRange;

    protected override void OnStart()
    {
        self = tree.AI.controlBlock;
        visionRange = self.visionRange;
        target = tree.AI.target;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Vector3Int vector = (self.cell.gridPosition - target.cell.gridPosition);
        if (Mathf.Abs(vector.x) + Mathf.Abs(vector.y) + Mathf.Abs(vector.z) > visionRange)
        {
            // Block is outside range
            Debug.Log("Node: Target is outside vision range");
            return children[0].Update();
        }
        else
        {
            // Block is inside range
            Debug.Log("Node: Target is inside vision range");
            return children[1].Update();
        }
    }
}
