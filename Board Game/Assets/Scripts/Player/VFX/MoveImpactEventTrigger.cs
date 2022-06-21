using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveImpactEventTrigger : AnimationEventTrigger
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform spawnPosition;

    public void MoveImpact() => Trigger();

    public override void Trigger()
    {
        Quaternion rotation = Quaternion.LookRotation((spawnPosition.position - pivot.position).normalized);
        GameObject effect = Instantiate(prefab, spawnPosition.position, rotation);
        _effects.Add(effect);
    }

}
