using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A Utility class that is designed to manipulate position and to pool blocks
/// </summary>
public class BlockUtilities
{
    public static List<GameObject> blockPool  = new List<GameObject>();

    public static void PlaceTerrainBlockAtCell(GameObject block, TerrainPlane plane, Cell cell)
    {
        if(plane.GetCellAndBlockFromCell(cell).block != null) { return; }

        block.GetComponent<Block>().SnapToCell(cell);
        plane.GetCellAndBlockFromCell(cell).block = block;
    }

    public static void PlaceCharacterBlockAtCell(GameObject block, CharacterPlane plane, Cell cell)
    {
        if (plane.GetCellAndBlockFromCell(cell).block != null) { return; }

        block.GetComponent<Block>().SnapToCell(cell);
        plane.GetCellAndBlockFromCell(cell).block = block;
    }

    public static void PlaceObjectBlockAtCell(GameObject block, ObjectPlane plane, Cell cell)
    {
        if (plane.GetCellAndBlockFromCell(cell).block != null) { return; }

        block.GetComponent<Block>().SnapToCell(cell);
        plane.GetCellAndBlockFromCell(cell).block = block;
    }

    public static void MoveTerrainBlockAtCellToPool(TerrainPlane plane, Cell cell, Transform pool)
    {
        GameObject block = plane.GetCellAndBlockFromCell(cell).block;
        if(block == null) { return; }

        block.transform.parent = pool;
        block.transform.localPosition = Vector3.zero;
        block.SetActive(false);
        blockPool.Add(block);
        plane.GetCellAndBlockFromCell(cell).block = null;
    }

    public static void MoveCharacterBlockAtCellToPool(CharacterPlane plane, Cell cell, Transform pool)
    {
        GameObject block = plane.GetCellAndBlockFromCell(cell).block;
        if (block == null) { return; }

        block.transform.parent = pool;
        block.transform.localPosition = Vector3.zero;
        block.SetActive(false);
        blockPool.Add(block);
        plane.GetCellAndBlockFromCell(cell).block = null;
    }

    public static void MoveObjectBlockAtCellToPool(ObjectPlane plane, Cell cell, Transform pool)
    {
        GameObject block = plane.GetCellAndBlockFromCell(cell).block;
        if (block == null) { return; }

        block.transform.parent = pool;
        block.transform.localPosition = Vector3.zero;
        block.SetActive(false);
        blockPool.Add(block);
        plane.GetCellAndBlockFromCell(cell).block = null;
    }

    public static void DrawWireframeAtCell(Cell cell, int gridDirectionInt)
    {
        if(cell == null) { return; }
        if (Application.isPlaying) { return; }

        #if UNITY_EDITOR
        Handles.color = Color.magenta;
        Handles.DrawWireCube(cell.worldPosition, Vector3.one);
        Vector3 direction = GridDirection.GetDirectionFromInt(gridDirectionInt);
        Handles.DrawLine(cell.worldPosition, cell.worldPosition + direction);
        #endif
    }
    

    public static GridDirection GetGridDirectionFromBlockInLevelFromCursor(LayerMask mask)
    {
        RaycastHit hit;
        Vector3 origin = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        Vector3 direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized;

        #if UNITY_EDITOR
        origin = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        direction = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).direction.normalized;
        #endif

        Physics.Raycast(origin, direction, out hit, 1000f, mask, QueryTriggerInteraction.Ignore);
        if (hit.collider == null) { return GridDirection.None; }
        if (hit.collider.transform.parent == null) { return GridDirection.None; }
        if (hit.collider.transform.parent.gameObject.GetComponent<Block>() == null) { return GridDirection.None; }

        Vector3 vector3 = hit.collider.transform.localPosition.normalized;
        Vector3Int vector3Int = new Vector3Int((int)vector3.x, (int)vector3.y, (int)vector3.z);
        return GridDirection.GetDirectionFromVector3Int(vector3Int);
    }

    public static GameObject GetBlockInLevelFromCursor(LayerMask mask)
    {
        RaycastHit hit;
        Vector3 origin = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        Vector3 direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction.normalized;

        #if UNITY_EDITOR
        origin = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        direction = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).direction.normalized;
        #endif

        Physics.Raycast(origin, direction, out hit, 1000f, mask, QueryTriggerInteraction.Ignore);
        if (hit.collider == null) { return null; }
        if (hit.collider.transform.parent == null) { return null; }
        if (hit.collider.transform.parent.gameObject.GetComponent<Block>() == null) { return null; }

        return hit.collider.transform.parent.gameObject;
    }
}
