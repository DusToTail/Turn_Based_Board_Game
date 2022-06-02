using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfBlockInAttackCells : CompositeNode
{
    public CharacterBlock self;
    public Block target;
    public int[,] attackGrid;

    protected override void OnStart()
    {
        self = tree.AI.controlBlock;
        target = tree.AI.target;
        attackGrid = self.weaponHandler.weapon.GetAttackGrid();
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Cell[] attackCells = self.gameManager.gridController.GetCellsFromCellWithDirectionAnd2DGrid(self.cell, self.forwardDirection, attackGrid);

        if (!System.Array.Exists(attackCells, x => x == target.cell))
        {
            // Block is outside range
            Debug.Log("Node: Target is outside attack cells");
            return children[0].Update();
        }
        else
        {
            // Block is inside range
            Debug.Log("Node: Target is inside attack cells");
            return children[1].Update();
        }
    }
}
