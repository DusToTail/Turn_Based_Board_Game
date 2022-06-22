using System;
using UnityEngine;

/// <summary>
/// Contain strings for game object's layers and tags
/// </summary>
public class TagsAndLayers
{
    public static readonly string SELECTABLE_LAYER = "Selectable";
    public static readonly string PLAYER_LAYER = "Player";
    public static readonly string CHARACTER_LAYER = "Character";
    public static readonly string TERRAIN_LAYER = "Terrain";
    public static readonly string OBJECT_LAYER = "Object";

    public static readonly string IGNORECAMERA_LAYER = "IgnoreCamera";
    public static readonly string DISCOVERED_LAYER = "Discovered";
    public static readonly string UNDISCOVERED_LAYER = "Undiscovered";
    public static readonly string NEARLIGHT_LAYER = "NearLight";



    public static readonly string CHARACTER_TAG = "Character";
    public static readonly string TERRAIN_TAG = "Terrain";
    public static readonly string OBJECT_TAG = "Object";
    public static readonly string PLAYER_TAG = "Player";
    public static readonly string ENEMY_TAG = "Enemy";
    public static readonly string MODEL_TAG = "Model";

    public static void ChangeLayersRecursively(GameObject target, string layerName)
    {
        target.layer = LayerMask.NameToLayer(layerName);
        for(int i = 0; i < target.transform.childCount; i++)
        {
            ChangeLayersRecursively(target.transform.GetChild(i).gameObject, layerName);
        }
    }
}
