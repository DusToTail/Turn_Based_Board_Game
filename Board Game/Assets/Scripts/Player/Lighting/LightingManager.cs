using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public List<PseudoLightSource> sources = new List<PseudoLightSource>();
    public Light playerLight;
    public Transform playerBlockTransform;
    [SerializeField] private Vector3 playerLightOffset;

    private void OnEnable()
    {
        PseudoLightSource.OnLightSourceAdded += AddLightSource;
        PseudoLightSource.OnLightSourceRemoved += RemoveLightSource;
    }

    private void OnDisable()
    {
        PseudoLightSource.OnLightSourceAdded -= AddLightSource;
        PseudoLightSource.OnLightSourceRemoved -= RemoveLightSource;
    }

    private void AddLightSource(PseudoLightSource source)
        => sources.Add(source);

    private void RemoveLightSource(PseudoLightSource source)
        => sources.Remove(source);

    private void LateUpdate()
    {
        if(playerLight == null) { return; }
        if(playerBlockTransform == null) { return; }
        playerLight.transform.position = playerBlockTransform.position + playerLightOffset;
    }
    
}
