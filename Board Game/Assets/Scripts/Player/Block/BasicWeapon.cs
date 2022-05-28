using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : Weapon
{
    public override void Attack(CharacterBlock userBlock)
    {
        if (!_canAttack) { return; }
        Cell userCell = userBlock.cell;
        GridDirection userDirection = userBlock.forwardDirection;
        Cell[] attackCells = userBlock.gameManager.gridController.GetCellsFromCellWithDirectionAnd2DGrid(userBlock.cell, userBlock.forwardDirection, _attackGrid);
        for (int l = 0; l < _attackGridLength; l++)
        {
            for(int w = 0; w < _attackGridWidth; w++)
            {
                if(_attackGrid[l,w] == 0) { continue; }

                GameObject toAttackBlock= userBlock.gameManager.characterPlane.grid[userCell.gridPosition.y, l, w].block;
                toAttackBlock.GetComponent<CharacterBlock>().TakeDamage(attackDamage);
                _curAttackCooldown--;

                // Animation and Sound effect

                // Trigger the occupants' TriggerHit method.
            }
        }
    }

    

}
