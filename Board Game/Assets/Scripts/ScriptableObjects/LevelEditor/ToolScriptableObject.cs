using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolData", menuName = "LevelEditor/Tool Data Asset")]
public class ToolScriptableObject : ScriptableObject
{
    public enum ToolType
    {
        Paint,
        Erase,
        Move,
        Clear
    }

    public GameObject paintPrefab;
    public ToolType toolType;


}
