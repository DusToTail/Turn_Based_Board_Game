using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that modify nearby objects' layers to NearLight layer, which allows being shone by the attached light component
/// </summary>
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Light))]
public class PseudoLightSource : MonoBehaviour
{
    public delegate void LightSourceAdded(PseudoLightSource source);
    public delegate void LightSourceRemoved(PseudoLightSource source);
    public static event LightSourceAdded OnLightSourceAdded;
    public static event LightSourceRemoved OnLightSourceRemoved;
    [SerializeField] private int rangeInCells;
    private float _actualRange;
    private SphereCollider _collider;
    private Light _light;

    private void OnEnable()
    {
        if(OnLightSourceAdded != null)
            OnLightSourceAdded(this);
    }

    private void OnDisable()
    {
        if(OnLightSourceRemoved != null)
            OnLightSourceRemoved(this);
    }

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _light = GetComponent<Light>();
    }

    private void Start()
    {
        _actualRange = rangeInCells * Mathf.Sqrt(FindObjectOfType<GridController>().cellSize.magnitude);
        _collider.radius = _actualRange;
        _light.range = _actualRange * _actualRange;
        _collider.isTrigger = true;
        _light.cullingMask = LayerMask.GetMask(TagsAndLayers.NEARLIGHT_LAYER, TagsAndLayers.DISCOVERED_LAYER);
        TagsAndLayers.ChangeLayersRecursively(transform.parent.Find("Model").gameObject, TagsAndLayers.NEARLIGHT_LAYER);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag(TagsAndLayers.MODEL_TAG))
        {
            TagsAndLayers.ChangeLayersRecursively(other.gameObject, TagsAndLayers.NEARLIGHT_LAYER);
            //Debug.Log($"{other.transform.parent.name} is near light source {transform.parent.gameObject.name}");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(TagsAndLayers.MODEL_TAG))
        {
            if(other.gameObject.layer != LayerMask.NameToLayer(TagsAndLayers.NEARLIGHT_LAYER))
            TagsAndLayers.ChangeLayersRecursively(other.gameObject, TagsAndLayers.NEARLIGHT_LAYER);
            //Debug.Log($"{other.transform.parent.name} is near light source {transform.parent.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(TagsAndLayers.MODEL_TAG))
        {
            // Non-player character block and object / terrain
            GameObject parent = other.gameObject.transform.parent.gameObject;
            if(parent.layer == LayerMask.NameToLayer(TagsAndLayers.CHARACTER_LAYER))
            {
                if(!parent.CompareTag(TagsAndLayers.PLAYER_TAG))
                    TagsAndLayers.ChangeLayersRecursively(other.gameObject, TagsAndLayers.IGNORECAMERA_LAYER);
            }
            else
                TagsAndLayers.ChangeLayersRecursively(other.gameObject, TagsAndLayers.DISCOVERED_LAYER);
            //Debug.Log($"{other.transform.parent.name} is off light source {transform.parent.gameObject.name}");
        }
    }
}
