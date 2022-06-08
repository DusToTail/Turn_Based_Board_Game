using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: Manage the information of all remote triggers
/// </summary>
[ExecuteAlways]
public class RemoteTriggerManager : MonoBehaviour
{
    public ObjectPlane objectPlane;
    public List<RemoteTriggerBehaviour> remoteTriggers = new List<RemoteTriggerBehaviour>();
    public int[] remoteTriggersData;

    private void OnEnable()
    {
        ObjectPlane.OnObjectPlaneInitialized += InitializeRemoteTriggersList;
    }

    private void OnDisable()
    {
        ObjectPlane.OnObjectPlaneInitialized -= InitializeRemoteTriggersList;
    }

    public void InitializeRemoteTriggersData(int[] data)
    {
        if (data == null) { Debug.Log("No remote trigger data"); return; }
        Debug.Log("Initialize remote trigger data");
        remoteTriggersData = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            remoteTriggersData[i] = data[i];
        }
    }

    private void InitializeRemoteTriggersList(ObjectPlane plane)
    {
        objectPlane = plane;
        remoteTriggers.Clear();
        remoteTriggers.TrimExcess();
        for (int h = 0; h < plane.grid.GetLength(0); h++)
        {
            for (int l = 0; l < plane.grid.GetLength(1); l++)
            {
                for (int w = 0; w < plane.grid.GetLength(2); w++)
                {
                    if (plane.grid[h, l, w].block != null)
                    {
                        if (plane.grid[h, l, w].block.GetComponentInChildren<RemoteTriggerBehaviour>() != null)
                        {
                            remoteTriggers.Add(plane.grid[h, l, w].block.GetComponentInChildren<RemoteTriggerBehaviour>());
                            //Debug.Log($"Remote Trigger Manager added: {plane.grid[h, l, w].block.name}");
                        }
                    }
                }
            }
        }

        if (remoteTriggersData != null)
        {
            if (remoteTriggersData.Length > 0)
            {
                for (int i = 0; i < remoteTriggers.Count; i++)
                {
                    Vector3Int toBeTriggeredBlockPosition = new Vector3Int(remoteTriggersData[3 * i], remoteTriggersData[3 * i + 1], remoteTriggersData[3 * i + 2]);
                    remoteTriggers[i].toBeTriggeredBlock = objectPlane.grid[toBeTriggeredBlockPosition.y, toBeTriggeredBlockPosition.z, toBeTriggeredBlockPosition.x].block.GetComponent<ObjectBlock>();
                }
            }
        }

        Debug.Log($"Remote Trigger Manager initialized with {remoteTriggers.Count} triggers");
    }

}
