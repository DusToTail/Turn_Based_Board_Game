using UnityEngine;

[CreateAssetMenu(fileName = "MoveToolData", menuName = "LevelEditor/MoveToolAsset", order = 1)]
public class MoveToolScriptableObject : ScriptableObject
{
    public string toolName;
    public string tooltip;
    public Texture2D icon;
}
