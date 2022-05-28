using UnityEngine;

/// <summary>
/// English: Scriptable object for weapons in general
/// </summary>
[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    [TextArea(5, 20)]
    public string weaponDescription;

    public int attackDamage;
    public int attackRange;
    public int usageCost;

    public int[] attackGrid;
    public int attackGridWidth;
    public int attackGridLength;
}
