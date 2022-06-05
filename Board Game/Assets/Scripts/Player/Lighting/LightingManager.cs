using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public Light playerLight;
    public Transform playerBlockTransform;

    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        if(playerLight == null) { return; }
        if(playerBlockTransform == null) { return; }

        playerLight.transform.position = playerBlockTransform.position + offset;
    }
}
