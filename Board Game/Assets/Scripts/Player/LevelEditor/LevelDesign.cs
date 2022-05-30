using UnityEngine;

/// <summary>
/// English: A template for designing level
/// </summary>
public class LevelDesign
{
    #region Description
    // Each level is comprised of a base grid, a grid for terrain, a grid for characters
    // Base grid will be processed by a grid controller
    // Terrain grid will be processed by a level plane
    // Character grid will be processed by a character plane
    // Object grid will be processed by an object plane
    // Each block will have an integer id
    #endregion
    public int gridHeight;
    public int gridLength;
    public int gridWidth;
    public int[] terrainGrid;
    public int[] characterGrid;
    public int[] objectGrid;

    public int[] stairsData;

}
