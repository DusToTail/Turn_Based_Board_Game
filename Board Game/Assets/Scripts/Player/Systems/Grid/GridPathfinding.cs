using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Static class for A* pathfinding in 3D grid with obstacles
/// </summary>
public static class GridPathfinding
{
    /// <summary>
    ///  Get the direction to the next cell in order to reach the destination with A*
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    /// <param name="gridController"></param>
    /// <param name="levelPlane"></param>
    /// <param name="characterPlane"></param>
    /// <param name="objectPlane"></param>
    /// <returns></returns>
    public static GridDirection GetImmediateDirection(Cell fromCell, Cell toCell, GridController gridController, LevelPlane levelPlane, CharacterPlane characterPlane, ObjectPlane objectPlane)
    {
        List<PathfindingCell> backtrackList = GetBacktrackPath(fromCell, toCell, gridController, levelPlane, characterPlane, objectPlane);
        if(backtrackList.Count < 2) 
        {
            Debug.Log("Pathfinding: There is no solution");
            return GridDirection.None; 
        }
        Vector3Int resultVector3Int = backtrackList[backtrackList.Count - 2].cell.gridPosition - backtrackList[backtrackList.Count - 1].cell.gridPosition ;
        resultVector3Int -= new Vector3Int(0, resultVector3Int.y, 0);
        resultVector3Int /= (int)resultVector3Int.magnitude;
        GridDirection resultDirection = GridDirection.GetDirectionFromVector3Int(resultVector3Int);
        return resultDirection;
    }

    /// <summary>
    /// Get a list of cells that construct the path towards the destination (backward)
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    /// <param name="gridController"></param>
    /// <param name="levelPlane"></param>
    /// <param name="characterPlane"></param>
    /// <param name="objectPlane"></param>
    /// <returns></returns>
    public static List<PathfindingCell> GetBacktrackPath(Cell fromCell, Cell toCell, GridController gridController, LevelPlane levelPlane, CharacterPlane characterPlane, ObjectPlane objectPlane)
    {
        // Create closed list
        PathfindingCell[,,] pathfindingGrid = new PathfindingCell[gridController.gridSize.y, gridController.gridSize.z, gridController.gridSize.x];
        for (int h = 0; h < gridController.gridSize.y; h++)
        {
            for (int l = 0; l < gridController.gridSize.z; l++)
            {
                for (int w = 0; w < gridController.gridSize.x; w++)
                {
                    pathfindingGrid[h, l, w] = new PathfindingCell(gridController.grid[h, l, w]);
                    //Debug.Log($"Created Pathfinding Cell at {pathfindingGrid[h, l, w].cell.gridPosition}");
                }
            }
        }
        

        // Create open list (priority queue) and add the starting point
        PriorityQueue<PathfindingCell> queue = new PriorityQueue<PathfindingCell>(true);
        PathfindingCell root = pathfindingGrid[fromCell.gridPosition.y, fromCell.gridPosition.z, fromCell.gridPosition.x];
        root.CalculateCosts(toCell, 0, true);
        root.parent = null;
        queue.Enqueue(root.fCost, root);
        //Debug.Log($"Pathfinding enqueued {root.cell.gridPosition} with {root.fCost} fCost, {root.gCost} gCost, {root.hCost} hCost");

        // Create backtrack list to return
        List<PathfindingCell> backtrackList = new List<PathfindingCell>();
        int count = 0;
        while (queue.Count > 0)
        {
            PathfindingCell current = queue.Dequeue();
            //Debug.Log($"Pathfinding processing Cell {current.cell.gridPosition}");

            // If destination is reached, construct backtrack path and return
            {
                if (current.cell == toCell)
                {
                    //Debug.Log($"Pathfinding reached destination at {current.cell.gridPosition}");
                    backtrackList.Add(current);
                    while (backtrackList[backtrackList.Count - 1].cell != fromCell)
                    {
                        backtrackList.Add(backtrackList[backtrackList.Count - 1].parent);
                    }
                    return backtrackList;
                }
            }
            

            // Enqueue neighboring cell for each direction excluding up, down, none
            foreach (GridDirection direction in GridDirection.AllDirections)
            {
                // Direction validation
                if (direction == GridDirection.Up || direction == GridDirection.Down) { continue; }

                Cell neighborCell = gridController.GetCellFromCellWithDirection(current.cell, direction);

                // Terrain, Object (such as stair) validation
                if (objectPlane.grid[neighborCell.gridPosition.y, neighborCell.gridPosition.z, neighborCell.gridPosition.x].block != null)
                {
                    GameObject objectBlock = objectPlane.grid[neighborCell.gridPosition.y, neighborCell.gridPosition.z, neighborCell.gridPosition.x].block;

                    // Handle impassable object
                    if (!objectBlock.GetComponent<ObjectBlock>().isPassable)
                        continue;
                    // Handle elevation (stair)
                    if (objectBlock.GetComponentInChildren<StairBehaviour>() != null)
                    {
                        Cell startStairCell = objectBlock.GetComponentInChildren<StairBehaviour>().startBlock.cell;
                        PathfindingCell startStairPathfindingCell = pathfindingGrid[startStairCell.gridPosition.y, startStairCell.gridPosition.z, startStairCell.gridPosition.x];
                        Cell endStairCell = objectBlock.GetComponentInChildren<StairBehaviour>().endBlock.cell;
                        PathfindingCell endStairPathfindingCell = pathfindingGrid[endStairCell.gridPosition.y, endStairCell.gridPosition.z, endStairCell.gridPosition.x];
                        if((current.gCost + endStairPathfindingCell.selfCost) < endStairPathfindingCell.gCost)
                        {
                            startStairPathfindingCell.CalculateCosts(toCell, current.gCost);
                            startStairPathfindingCell.parent = current;
                            queue.Enqueue(startStairPathfindingCell.fCost, startStairPathfindingCell);
                            //Debug.Log($"Pathfinding enqueued {startStairPathfindingCell.cell.gridPosition} with {startStairPathfindingCell.fCost} fCost, {startStairPathfindingCell.gCost} gCost, {startStairPathfindingCell.hCost} hCost, {startStairPathfindingCell.parent.cell.gridPosition} as parent");

                            endStairPathfindingCell.CalculateCosts(toCell, current.gCost);
                            endStairPathfindingCell.parent = startStairPathfindingCell;
                            queue.Enqueue(endStairPathfindingCell.fCost, endStairPathfindingCell);
                            //Debug.Log($"Pathfinding enqueued {endStairPathfindingCell.cell.gridPosition} with {endStairPathfindingCell.fCost} fCost, {endStairPathfindingCell.gCost} gCost, {endStairPathfindingCell.hCost} hCost, {endStairPathfindingCell.parent.cell.gridPosition} as parent");
                        }
                    }
                }
                else if (levelPlane.grid[neighborCell.gridPosition.y, neighborCell.gridPosition.z, neighborCell.gridPosition.x] != null)
                    continue;
                else if(levelPlane.grid[neighborCell.gridPosition.y, neighborCell.gridPosition.z, neighborCell.gridPosition.x] == null)
                {
                    Cell belowCell = gridController.GetCellFromCellWithDirection(neighborCell, GridDirection.Down);
                    if (levelPlane.grid[belowCell.gridPosition.y, belowCell.gridPosition.z, belowCell.gridPosition.x] == null)
                        continue;
                }

                PathfindingCell neighborPathfindingCell = pathfindingGrid[neighborCell.gridPosition.y, neighborCell.gridPosition.z, neighborCell.gridPosition.x];

                // gCost validation
                if((current.gCost + neighborPathfindingCell.selfCost) >= neighborPathfindingCell.gCost) { continue; }

                neighborPathfindingCell.CalculateCosts(toCell, current.gCost);
                neighborPathfindingCell.parent = current;
                queue.Enqueue(neighborPathfindingCell.fCost, neighborPathfindingCell);
                //Debug.Log($"Pathfinding enqueued {neighborPathfindingCell.cell.gridPosition} with {neighborPathfindingCell.fCost} fCost, {neighborPathfindingCell.gCost} gCost, {neighborPathfindingCell.hCost} hCost, {neighborPathfindingCell.parent.cell.gridPosition} as parent");
                
            }
            
            count++;
        }
        //Debug.Log($"Iteration count: {count}");
        Debug.Log("Pathfinding: There is no solution");

        return backtrackList;
    }

    /// <summary>
    /// Implementation of an A* node
    /// </summary>
    public class PathfindingCell
    {
        public PathfindingCell parent;
        public Cell cell;
        public int selfCost;
        public int hCost;
        public int gCost;
        public int fCost;

        public PathfindingCell(Cell cell)
        {
            parent = null;
            this.cell = cell;
            selfCost = 1;
            hCost = 0;
            gCost = int.MaxValue;
            fCost = 0;
        }

        public void CalculateCosts(Cell toCell, int gCost, bool isRoot = false)
        {
            Vector3Int toVector = toCell.gridPosition - cell.gridPosition;
            hCost = (int)toVector.magnitude;
            if (isRoot)
                this.gCost = 0;
            else
                this.gCost = gCost + selfCost;
            fCost = gCost + hCost;

        }

    }

}
