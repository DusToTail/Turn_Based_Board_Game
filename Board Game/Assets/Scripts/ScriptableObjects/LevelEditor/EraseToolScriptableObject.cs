using UnityEngine;

[CreateAssetMenu(fileName = "EraseToolData", menuName = "LevelEditor/EraseToolAsset", order = 1)]
public class EraseToolScriptableObject : ScriptableObject
{
    public string toolName;
    public string tooltip;
    public Texture2D icon;
}
