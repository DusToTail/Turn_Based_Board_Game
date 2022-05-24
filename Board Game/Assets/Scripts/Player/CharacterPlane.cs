using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: A plane made out of character blocks, used to keep track of all character in the level
/// </summary>
public class CharacterPlane : MonoBehaviour
{
    public delegate void CharacterPlaneInitialized(CharacterPlane characterPlane);
    public static event CharacterPlaneInitialized OnCharacterPlaneInitialized;

    public CellAndBlock[,,] grid { get; private set; }


    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGrid;
    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGrid;
    }

    private void InitializeGrid(GridController controller)
    {
        // Clear past childs in the grid
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Debug.Log($"Destroying {transform.GetChild(i).gameObject.name}");
            if (Application.isPlaying) { Destroy(transform.GetChild(i).gameObject); }
            else { DestroyImmediate(transform.GetChild(i).gameObject); }
        }

        grid = new CellAndBlock[controller.gridSize.y, controller.gridSize.z, controller.gridSize.x];
        for (int h = 0; h < controller.gridSize.y; h++)
        {
            for (int l = 0; l < controller.gridSize.z; l++)
            {
                for (int w = 0; w < controller.gridSize.x; w++)
                {
                    Cell cell = controller.grid[h, l, w];
                    CellAndBlock cellAndBlock = new CellAndBlock(cell, null);
                    grid[h, l, w] = cellAndBlock;
                }
            }
        }
        Debug.Log($"Character grid initialized");
        if (OnCharacterPlaneInitialized != null)
            OnCharacterPlaneInitialized(this);
    }
}
