using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackImpactEventTrigger : AnimationEventTrigger
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private Block userBlock;

    public bool isTriggered { get; set; }

    public void AttackImpact() => Trigger();

    public override void Trigger()
    {
        Quaternion rotation = Quaternion.LookRotation((spawnPosition.position - pivot.position).normalized);
        GameObject effect = Instantiate(prefab, spawnPosition.position, rotation);
        _effects.Add(effect);

        if(userBlock is CharacterBlock characterBlock)
            weaponHandler.UseWeapon(characterBlock);
        else if(userBlock is ObjectBlock objectBlock)
            weaponHandler.UseWeapon(objectBlock);

        isTriggered = true;
    }

    
}
