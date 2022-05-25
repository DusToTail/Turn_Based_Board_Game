using UnityEngine;
using System.Linq;

/// <summary>
/// English: A container for prefabs
/// </summary>
public class BlockIDContainer : MonoBehaviour
{
    public GameObject[] blocks;

    public GameObject GetCopyFromID(int id)
    {
        GameObject prefab = blocks.Where(x => x.GetComponent<Block>().id == id).DefaultIfEmpty(blocks[0]).First();
        if(prefab == null) { return null; }
        return Instantiate(prefab); 
    }
}
