using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : Weapon
{
    public void Start()
    {
        InitializeWeapon();
    }

    public override void Attack(CharacterBlock userBlock)
    {
        Cell[] attackCells = userBlock.gameManager.gridController.GetCellsFromCellWithDirectionAnd2DGrid(userBlock.cell, userBlock.forwardDirection, _attackGrid);
        for(int i = 0; i < attackCells.Length; i++)
        {
            Debug.Log($"Planning to attack {attackCells[i].gridPosition}");
        }
        for(int i = 0; i < attackCells.Length; i++)
        {
            Cell attackCell = attackCells[i];
            GameObject toAttackBlock = userBlock.gameManager.characterPlane.grid[attackCell.gridPosition.y, attackCell.gridPosition.z, attackCell.gridPosition.x].block;
            if(toAttackBlock == null) { continue; }
            // Animation and Sound effect

            // Trigger the occupants' TriggerHit method.
            toAttackBlock.GetComponent<CharacterBlock>().TakeDamage(attackDamage);
            Debug.Log($"Attacked cell {attackCell.gridPosition}");
        }
    }


    

}
