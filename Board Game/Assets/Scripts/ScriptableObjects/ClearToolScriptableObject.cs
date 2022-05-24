using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ClearToolData", menuName = "LevelEditor/ClearToolAsset", order = 1)]
public class ClearToolScriptableObject : ScriptableObject
{
    public string toolName;
    public string tooltip;
    public Texture2D icon;
}
