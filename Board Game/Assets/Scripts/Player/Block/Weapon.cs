using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponScriptableObject weaponScriptableObject;

    public string weaponName;
    public string weaponDescription;

    public int attackDamage;
    public int attackRange;
    public int attackCooldown;

    protected int[,] _attackGrid;
    protected int _attackGridWidth;
    protected int _attackGridLength;

    protected int _curAttackCooldown;
    protected bool _canAttack;


    public abstract void Attack(CharacterBlock userBlock);
    public void ReduceAttackCooldown(int amount) 
    { 
        if(amount < 0) { return; }
        _curAttackCooldown -= amount;
        if(_curAttackCooldown < 0) { _curAttackCooldown = 0; }
    }
    public void ResetAttackCooldown() { _curAttackCooldown = attackCooldown; }
    public void SetCanAttack(bool canAttack) { _canAttack = canAttack; }

    public void InitializeAttackGrid(int[] oneDimensionalGrid, int width, int length)
    {
        _attackGridWidth = width;
        _attackGridLength = length;
        _attackGrid = new int[length, width];
        int count = 0;
        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if(count > oneDimensionalGrid.Length - 1) { break; }
                _attackGrid[i,j] = oneDimensionalGrid[count];
                count++;
            }
        }
    }
}
