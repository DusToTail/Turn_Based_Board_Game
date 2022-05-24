using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "LevelEditor/Block Asset")]
public class BlockScriptableObject : ScriptableObject
{
    public GameObject prefab;
}
