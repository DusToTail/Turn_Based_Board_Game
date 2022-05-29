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
        List<GameObject> toAttackBlocks = new List<GameObject>();
        for(int i = 0; i < attackCells.Length; i++)
        {
            Debug.Log($"Planning to attack {attackCells[i].gridPosition}");
        }
        userBlock.attackedCharacterCount = 0;
        userBlock.curAttackedCharacterCount = 0;
        for (int i = 0; i < attackCells.Length; i++)
        {
            Cell attackCell = attackCells[i];
            GameObject toAttackBlock = userBlock.gameManager.characterPlane.grid[attackCell.gridPosition.y, attackCell.gridPosition.z, attackCell.gridPosition.x].block;
            if(toAttackBlock == null) { continue; }
            toAttackBlocks.Add(toAttackBlock);
            userBlock.attackedCharacterCount++;
        }
        for (int i = 0; i < toAttackBlocks.Count; i++)
        {
            // Animation and Sound effect

            // Trigger the occupants' TriggerHit method.
            toAttackBlocks[i].GetComponent<CharacterBlock>().TakeDamage(userBlock, attackDamage);
            Debug.Log($"Attacked {toAttackBlocks[i].name} at cell {toAttackBlocks[i].GetComponent<CharacterBlock>().cell.gridPosition}");
        }
    }


    

}
