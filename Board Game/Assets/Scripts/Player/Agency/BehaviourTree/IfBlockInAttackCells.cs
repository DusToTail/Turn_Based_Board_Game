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
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        attackGrid = self.weaponHandler.weapon.GetAttackGrid();
        Cell[] attackCells = self.gameManager.gridController.GetCellsFromCellWithDirectionAnd2DGrid(self.cell, self.forwardDirection, attackGrid);

        if (!System.Array.Exists(attackCells, x => x == target.cell))
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
