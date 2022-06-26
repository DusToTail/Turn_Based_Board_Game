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
}
