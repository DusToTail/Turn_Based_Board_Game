using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroy the game object containing this behvaiour when there is no more children under it
/// </summary>
public class EffectSelfDestruct : MonoBehaviour
{
    private void Update()
    {
        if(transform.childCount == 0) { Destroy(gameObject); }
    }
}
