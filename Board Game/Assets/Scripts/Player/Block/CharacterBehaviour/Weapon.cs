using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all weapons
/// </summary>
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
    public abstract void Attack(ObjectBlock userBlock);

    public void InitializeWeapon()
    {
        weaponName = data.weaponName;
        weaponDescription = data.weaponDescription;
        attackDamage = data.attackDamage;
        attackRange = data.attackRange;
        usageCost = data.usageCost;
        InitializeAttackGrid(data.attackGrid, data.attackGridWidth, data.attackGridLength);
    }

    // An attack grid's root is at the user block's cell position and the direction that it extends/grows is relative to the user's orientation
    // Example: 1x1 grid with one int element as 1 means, it will attack the cell at the user's position
    // Example: 2x1 grid with [0,0] as 0 and [1,0] as 1 means, it will attack the forward cell
    // Note: use grid with width as an odd integer
    // Notion of n x m: Height x width

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

    public int[,] GetAttackGrid() { return _attackGrid; }
}
