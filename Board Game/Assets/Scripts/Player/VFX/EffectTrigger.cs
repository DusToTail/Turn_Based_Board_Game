using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for spawning effect into the scene
/// </summary>
public abstract class EffectTrigger : MonoBehaviour
{
    public GameObject prefab;

    public abstract void Trigger();
}
