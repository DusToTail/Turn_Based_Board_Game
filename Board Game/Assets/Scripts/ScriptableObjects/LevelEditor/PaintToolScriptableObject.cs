using UnityEngine;

[CreateAssetMenu(fileName = "PaintToolData", menuName = "LevelEditor/PaintToolAsset", order = 1)]
public class PaintToolScriptableObject : ScriptableObject
{
    public string toolName;
    public string tooltip;
    public Texture2D icon;
}
