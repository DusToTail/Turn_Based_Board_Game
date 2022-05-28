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
        for(int i = 0; i < attackCells.Length; i++)
        {
            Cell attackCell = attackCells[i];
            GameObject toAttackBlock = userBlock.gameManager.characterPlane.grid[attackCell.gridPosition.y, attackCell.gridPosition.z, attackCell.gridPosition.x].block;


            // Animation and Sound effect

            // Trigger the occupants' TriggerHit method.
            toAttackBlock.GetComponent<CharacterBlock>().TakeDamage(attackDamage);
            _curAttackCooldown--;
        }

    }

    

}
