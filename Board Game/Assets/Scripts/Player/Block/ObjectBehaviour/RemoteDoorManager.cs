using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Manage the information of all remote doors
/// </summary>
[ExecuteAlways]
public class RemoteDoorManager : MonoBehaviour
{
    public ObjectPlane objectPlane;
    public List<RemoteDoorBehaviour> remoteDoors = new List<RemoteDoorBehaviour>();
    public int[] remoteDoorsData;

    private void OnEnable()
    {
        ObjectPlane.OnObjectPlaneInitialized += InitializeRemoteDoorsList;
    }

    private void OnDisable()
    {
        ObjectPlane.OnObjectPlaneInitialized -= InitializeRemoteDoorsList;
    }

    public void InitializeRemoteDoorsData(int[] data)
    {
        if (data == null) { Debug.Log("No remote door data"); return; }
        Debug.Log("Initialize remote door data");
        remoteDoorsData = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            remoteDoorsData[i] = data[i];

        }
    }

    private void InitializeRemoteDoorsList(ObjectPlane plane)
    {
        objectPlane = plane;
        remoteDoors.Clear();
        remoteDoors.TrimExcess();
        for (int h = 0; h < plane.grid.GetLength(0); h++)
        {
            for (int l = 0; l < plane.grid.GetLength(1); l++)
            {
                for (int w = 0; w < plane.grid.GetLength(2); w++)
                {
                    if (plane.grid[h, l, w].block != null)
                    {
                        if (plane.grid[h, l, w].block.GetComponentInChildren<RemoteDoorBehaviour>() != null)
                        {
                            remoteDoors.Add(plane.grid[h, l, w].block.GetComponentInChildren<RemoteDoorBehaviour>());
                            //Debug.Log($"Remote Door Manager added: {plane.grid[h, l, w].block.name}");
                        }
                    }
                }
            }
        }

        if (remoteDoorsData != null)
        {
            if (remoteDoorsData.Length > 0)
            {
                for (int i = 0; i < remoteDoors.Count; i++)
                {
                    if(remoteDoorsData[i] == 0)
                    {
                        remoteDoors[i].SetPassableBool(false);
                    }
                    else
                    {
                        remoteDoors[i].SetPassableBool(true);
                    }
                }
            }
        }

        Debug.Log($"Remote Door Manager initialized with {remoteDoors.Count} doors");
    }

}
