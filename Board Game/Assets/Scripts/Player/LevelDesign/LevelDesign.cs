using UnityEngine;

/// <summary>
/// English: A template for designing level
/// </summary>
public class LevelDesign
{
    #region GridAndPlaneDescription
    /*
     * Each level is comprised of a base grid, a grid for terrain, a grid for characters
     * Base grid will be processed by a grid controller
     * Terrain grid will be processed by a level plane
     * Character grid will be processed by a character plane
     * Object grid will be processed by an object plane
     * Each block will have an integer id
     */
    public int gridHeight;
    public int gridLength;
    public int gridWidth;
    public int[] terrainGrid;
    public int[] characterGrid;
    public int[] objectGrid;
    #endregion



    #region BlockSpecificDescription
    /*
     * Rotations: 1 = Forward / -1 = Backward / 2 = Left / -2 = Right / 3 = Up / -3 = Down
     * Remote Triggers Data: the Vector3Int grid position of the object block to be triggered
     * Remote Doors Data: 1 = true / 0 = false
     * 
     * 
     * 
     * 
     */
    public int[] rotations;
    public int[] remoteTriggersData;
    public int[] remoteDoorsData;
    #endregion
    
}
