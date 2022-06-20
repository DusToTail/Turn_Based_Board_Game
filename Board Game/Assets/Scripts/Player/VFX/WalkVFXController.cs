using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkVFXController : EffectTrigger
{
    [SerializeField] private Transform pivot;
    [SerializeField] private Transform spawnPosition;
    public override void Trigger()
    {
        GameObject effect = Instantiate(prefab, spawnPosition, true);
        effect.transform.position = spawnPosition.position;

        effect.transform.rotation = Quaternion.LookRotation((spawnPosition.position - pivot.position).normalized);

    }
}
