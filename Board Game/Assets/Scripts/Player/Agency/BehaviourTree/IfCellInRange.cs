using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfCellInRange : CompositeNode
{
    public Block self;
    public Cell cell;
    public int range;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Vector3Int vector = (self.cell.gridPosition - cell.gridPosition);
        if (Mathf.Abs(vector.x) + Mathf.Abs(vector.y) + Mathf.Abs(vector.z) > range)
        {
            // Block is outside range
            return children[0].Update();
        }
        else
        {
            // Block is inside range
            return children[1].Update();
        }
    }
}
