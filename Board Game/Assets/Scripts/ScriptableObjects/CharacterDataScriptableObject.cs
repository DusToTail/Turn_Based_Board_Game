using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data")]
public class CharacterDataScriptableObject : ScriptableObject
{
    public int movesPerTurn;
    public int maxHealth;
    public int visionRange;
    public WeaponScriptableObject defaultWeapon;
}
