using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public Light playerLight;
    public Transform player;

    [SerializeField] private Vector3 offset;

    private void Update()
    {
        if(playerLight == null) { return; }
        if(player == null) { return; }
        playerLight.transform.position = player.position + offset;
    }
}
