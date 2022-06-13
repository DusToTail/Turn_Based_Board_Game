using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An extension for handling weapons in a character/object block
/// </summary>
public class WeaponHandler : MonoBehaviour
{
    public Weapon weapon;

    public void UseWeapon(CharacterBlock userBlock)
    {
        weapon.Attack(userBlock);
    }

    public void UseWeapon(ObjectBlock userBlock)
    {
        weapon.Attack(userBlock);
    }
}
