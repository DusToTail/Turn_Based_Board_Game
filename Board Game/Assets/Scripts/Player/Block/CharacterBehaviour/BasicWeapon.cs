using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A basic weapon does not have any abilities, and utilizes only damage and attack grid
/// </summary>
public class BasicWeapon : Weapon
{
    public override void Attack(CharacterBlock userBlock)
    {
        // Get cells with the attack grid
        Cell[] attackCells = userBlock.gameManager.gridController.GetCellsFromCellWithDirectionAnd2DGrid(userBlock.cell, userBlock.forwardDirection, _attackGrid);
        List<GameObject> toAttackBlocks = new List<GameObject>();

        // Reset and start counting victims (used to make the user block coroutine to wait)
        userBlock.attackedEntityCount = 0;
        userBlock.curAttackedEntityCount = 0;

        // Count attacked victims
        for (int i = 0; i < attackCells.Length; i++)
        {
            Cell attackCell = attackCells[i];
            GameObject toAttackCharacter = userBlock.gameManager.characterPlane.grid[attackCell.gridPosition.y, attackCell.gridPosition.z, attackCell.gridPosition.x].block;
            if(toAttackCharacter != null)
            {
                toAttackBlocks.Add(toAttackCharacter);
                userBlock.attackedEntityCount++;
            }
            GameObject toAttackObject = userBlock.gameManager.objectPlane.grid[attackCell.gridPosition.y, attackCell.gridPosition.z, attackCell.gridPosition.x].block;
            if(toAttackObject != null && toAttackObject.GetComponent<ObjectBlock>().activationBehaviour.GetComponent<IDestroyableOnAttacked>() != null)
            {
                toAttackBlocks.Add(toAttackObject);
                userBlock.attackedEntityCount++;
            }
        }

        // Attack victims (curAttackedEntityCount will be incremented in the corresponding TakeDamage/OnAttacked method)
        for (int i = 0; i < toAttackBlocks.Count; i++)
        {
            // Animation and Sound effect

            // Trigger the characters' TakeDamage method.
            if(toAttackBlocks[i].GetComponent<CharacterBlock>() != null)
            {
                toAttackBlocks[i].GetComponent<CharacterBlock>().TakeDamage(userBlock, attackDamage);
                Debug.Log($"Attacked {toAttackBlocks[i].name} at cell {toAttackBlocks[i].GetComponent<CharacterBlock>().cell.gridPosition}");
            }
            // Trigger the obstacles' OnAttacked method.
            if (toAttackBlocks[i].GetComponent<ObjectBlock>() != null)
            {
                ObjectBlock target = toAttackBlocks[i].GetComponent<ObjectBlock>();
                target.activationBehaviour.GetComponent<IDestroyableOnAttacked>().OnAttacked(target, userBlock);
                Debug.Log($"Attacked {toAttackBlocks[i].name} at cell {target.cell.gridPosition}");
            }
            
        }
    }

    public override void Attack(ObjectBlock userBlock)
    {
        // Get cells with the attack grid
        Cell[] attackCells = userBlock.gameManager.gridController.GetCellsFromCellWithDirectionAnd2DGrid(userBlock.cell, userBlock.forwardDirection, _attackGrid);
        List<GameObject> toAttackBlocks = new List<GameObject>();

        // Count attacked victims
        for (int i = 0; i < attackCells.Length; i++)
        {
            Cell attackCell = attackCells[i];
            GameObject toAttackBlock = userBlock.gameManager.characterPlane.grid[attackCell.gridPosition.y, attackCell.gridPosition.z, attackCell.gridPosition.x].block;
            if (toAttackBlock == null) { continue; }
            toAttackBlocks.Add(toAttackBlock);
            userBlock.activationBehaviour.GetComponent<IDamageOnActivation>().attackedCharacterCount++;

        }

        // Attack victims (curAttackedCharacterCount will be incremented in the corresponding TakeDamage method of each character)
        for (int i = 0; i < toAttackBlocks.Count; i++)
        {
            // Animation and Sound effect

            // Trigger the occupants' TriggerHit method.
            toAttackBlocks[i].GetComponent<CharacterBlock>().TakeDamage(userBlock, attackDamage);
            Debug.Log($"Attacked {toAttackBlocks[i].name} at cell {toAttackBlocks[i].GetComponent<CharacterBlock>().cell.gridPosition}");
        }
    }

}