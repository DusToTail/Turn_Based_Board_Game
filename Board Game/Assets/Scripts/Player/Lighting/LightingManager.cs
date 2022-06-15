using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public Light playerLight;
    public Light surroundingLight;
    public Transform playerBlockTransform;

    [SerializeField] private Vector3 playerLightOffset;
    [SerializeField] private Vector3 surroundingLightOffset;


    private void LateUpdate()
    {
        if(playerLight == null) { return; }
        if(playerBlockTransform == null) { return; }

        playerLight.transform.position = playerBlockTransform.position + playerLightOffset;
        surroundingLight.transform.position = playerBlockTransform.position + surroundingLightOffset;
    }
}
