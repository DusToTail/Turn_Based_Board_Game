using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A basic weapon does not have any abilities, and utilizes only damage and attack grid
/// </summary>
public class BasicWeapon : BaseWeapon
{
    public override void Attack(CharacterBlock userBlock)
    {
        GameObject[] toAttackBlocks = GetBlocksToAttackFromCharacter(userBlock);
        for (int i = 0; i < toAttackBlocks.Length; i++)
        {
            if(toAttackBlocks[i].GetComponent<CharacterBlock>() != null)
                AttackCharacter(userBlock, toAttackBlocks[i]);
            else if(toAttackBlocks[i].GetComponent<ObjectBlock>() != null)
                AttackObject(userBlock, toAttackBlocks[i]);
        }
    }

    public override void Attack(ObjectBlock userBlock)
    {
        GameObject[] toAttackBlocks = GetBlocksToAttackFromObject(userBlock);

        for (int i = 0; i < toAttackBlocks.Length; i++)
        {
            if(toAttackBlocks[i].GetComponent<CharacterBlock>() != null)
                AttackCharacter(userBlock,toAttackBlocks[i]);
        }
    }

    private GameObject[] GetBlocksToAttackFromCharacter(CharacterBlock attacker)
    {
        Cell[] attackCells = attacker.gameManager.gridController.GetCellsFromCellWithDirectionAnd2DGrid(attacker.cell, attacker.forwardDirection, _attackGrid);
        List<GameObject> toAttackBlocks = new List<GameObject>();

        // Reset and start counting victims
        attacker.attackedEntityCount = 0;
        attacker.curAttackedEntityCount = 0;

        // Count attacked victims or objects
        for (int i = 0; i < attackCells.Length; i++)
        {
            Cell attackCell = attackCells[i];
            GameObject toAttackCharacter = attacker.gameManager.characterPlane.grid[attackCell.gridPosition.y, attackCell.gridPosition.z, attackCell.gridPosition.x].block;
            if (toAttackCharacter != null)
            {
                toAttackBlocks.Add(toAttackCharacter);
                attacker.attackedEntityCount++;
            }
            GameObject toAttackObject = attacker.gameManager.objectPlane.grid[attackCell.gridPosition.y, attackCell.gridPosition.z, attackCell.gridPosition.x].block;
            if (toAttackObject != null && toAttackObject.GetComponent<ObjectBlock>().activationBehaviour.GetComponent<IDestroyableOnAttacked>() != null)
            {
                toAttackBlocks.Add(toAttackObject);
                attacker.attackedEntityCount++;
            }
        }

        return toAttackBlocks.ToArray();
    }

    private GameObject[] GetBlocksToAttackFromObject(ObjectBlock attacker)
    {
        Cell[] attackCells = attacker.gameManager.gridController.GetCellsFromCellWithDirectionAnd2DGrid(attacker.cell, attacker.forwardDirection, _attackGrid);
        List<GameObject> toAttackBlocks = new List<GameObject>();

        // Count to be attacked victims or objects
        for (int i = 0; i < attackCells.Length; i++)
        {
            Cell attackCell = attackCells[i];
            GameObject toAttackBlock = attacker.gameManager.characterPlane.GetCellAndBlockFromCell(attackCell).block;
            if (toAttackBlock == null) { continue; }
            toAttackBlocks.Add(toAttackBlock);
            attacker.activationBehaviour.GetComponent<IDamageOnActivation>().attackedCharacterCount++;
        }
        return toAttackBlocks.ToArray();
    }

    private void AttackCharacter(CharacterBlock attacker, GameObject victim)
    {
        // Trigger the characters' TakeDamage method.
        if (victim.GetComponent<CharacterBlock>() != null)
        {
            victim.GetComponent<CharacterBlock>().TakeDamage(attacker, attackDamage);
            Debug.Log($"Attacked {victim.name} at cell {victim.GetComponent<CharacterBlock>().cell.gridPosition}");
        }
    }
    private void AttackCharacter(ObjectBlock attacker, GameObject victim)
    {
        // Trigger the characters' TakeDamage method.
        if (victim.GetComponent<CharacterBlock>() != null)
        {
            victim.GetComponent<CharacterBlock>().TakeDamage(attacker, attackDamage);
            Debug.Log($"Attacked {victim.name} at cell {victim.GetComponent<CharacterBlock>().cell.gridPosition}");
        }
    }

    private void AttackObject(CharacterBlock attacker, GameObject victim)
    {
        // Trigger the obstacles' OnAttacked method.
        if (victim.GetComponent<ObjectBlock>() != null)
        {
            ObjectBlock block = victim.GetComponent<ObjectBlock>();
            block.activationBehaviour.GetComponent<IDestroyableOnAttacked>().OnAttacked(block, attacker);
            Debug.Log($"Attacked {victim.name} at cell {block.cell.gridPosition}");
        }
    }
}
