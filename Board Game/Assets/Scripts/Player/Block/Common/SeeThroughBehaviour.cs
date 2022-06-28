using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughBehaviour : MonoBehaviour
{
    [SerializeField] private Material opaque;
    [SerializeField] private Material transparent;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Block target;
    [SerializeField] private Block self;

    private void OnEnable()
    {
        PlayerController.OnPlayerIsInitialized += InitializeTarget;
    }
    private void OnDisable()
    {
        PlayerController.OnPlayerIsInitialized -= InitializeTarget;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer(TagsAndLayers.SEETHROUGH_LAYER))
        {
            if(self.cell.gridPosition.y > target.cell.gridPosition.y)
                meshRenderer.sharedMaterial = transparent;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(TagsAndLayers.SEETHROUGH_LAYER))
        {
            if (self.cell.gridPosition.y > target.cell.gridPosition.y)
                meshRenderer.sharedMaterial = opaque;
        }
    }
    private void InitializeTarget(PlayerController playerController)
    {
        target = playerController.playerBlock;
    }
}
