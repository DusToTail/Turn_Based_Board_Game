using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Manage the information of all stairs
/// </summary>
[ExecuteAlways]
public class StairsManager : MonoBehaviour
{
    public ObjectPlane objectPlane;
    public List<StairBehaviour> stairs = new List<StairBehaviour>();
    public int[] stairsData;

    private void OnEnable()
    {
        ObjectPlane.OnObjectPlaneInitialized += InitializeStairsList;
    }

    private void OnDisable()
    {
        ObjectPlane.OnObjectPlaneInitialized -= InitializeStairsList;
    }

    public void InitializeStairsData(int[] data)
    {
        if(data == null) { Debug.Log("No Stair Data"); return; }
        stairsData = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            stairsData[i] = data[i];

        }
    }

    private void InitializeStairsList(ObjectPlane plane)
    {
        objectPlane = plane;
        stairs.Clear();
        stairs.TrimExcess();
        for(int h = 0; h < plane.grid.GetLength(0); h++)
        {
            for(int l = 0; l < plane.grid.GetLength(1); l++)
            {
                for(int w = 0; w < plane.grid.GetLength(2); w++)
                {
                    if(plane.grid[h, l, w].block != null)
                    {
                        if(plane.grid[h,l,w].block.GetComponentInChildren<StairBehaviour>() != null)
                        {
                            stairs.Add(plane.grid[h, l, w].block.GetComponentInChildren<StairBehaviour>());
                            //Debug.Log($"Stairs Manager added: {plane.grid[h, l, w].block.name}");
                        }
                    }
                }
            }
        }

        if(stairsData != null)
        {
            if(stairsData.Length > 0)
            {
                for (int i = 0; i < stairs.Count; i++)
                {
                    Vector3Int startGridPosition = new Vector3Int(stairsData[6 * i], stairsData[6 * i + 1], stairsData[6 * i + 2]);
                    Vector3Int endGridPosition = new Vector3Int(stairsData[6 * i + 3], stairsData[6 * i + 4], stairsData[6 * i + 5]);
                    stairs[i].startBlock = objectPlane.grid[startGridPosition.y, startGridPosition.z, startGridPosition.x].block.GetComponent<Block>();
                    stairs[i].endBlock = objectPlane.grid[endGridPosition.y, endGridPosition.z, endGridPosition.x].block.GetComponent<Block>();
                }
            }
        }

        Debug.Log($"Stairs Manager initialized with {stairs.Count} stairs");
    }


}
