using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for triggering animation events
/// </summary>
public abstract class AnimationEventTrigger : MonoBehaviour
{
    public GameObject prefab;
    protected List<GameObject> _effects = new List<GameObject>();

    public abstract void Trigger();
    public bool CheckIfEffectsAreAllDestroyed()
    {
        _effects.TrimExcess();
        return _effects.Count == 0;
    }
}
