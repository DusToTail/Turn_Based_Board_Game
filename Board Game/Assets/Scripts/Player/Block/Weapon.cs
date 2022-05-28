using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponScriptableObject data;

    public string weaponName;
    public string weaponDescription;

    public int attackDamage;
    public int attackRange;
    public int usageCost;

    protected int[,] _attackGrid;
    protected int _attackGridWidth;
    protected int _attackGridLength;


    public abstract void Attack(CharacterBlock userBlock);

    public void InitializeWeapon()
    {
        weaponName = data.weaponName;
        weaponDescription = data.weaponDescription;
        attackDamage = data.attackDamage;
        attackRange = data.attackRange;
        usageCost = data.usageCost;
        InitializeAttackGrid(data.attackGrid, data.attackGridWidth, data.attackGridLength);
    }
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
