using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Static class for A* pathfinding in 3D grid with obstacles
/// </summary>
public static class GridPathfinding
{
    public static GridDirection GetImmediateDirection(Cell fromCell, Cell toCell, GridController gridController, TerrainPlane levelPlane, CharacterPlane characterPlane, ObjectPlane objectPlane)
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

    public static List<PathfindingCell> GetBacktrackPath(Cell fromCell, Cell toCell, GridController gridController, TerrainPlane terrainPlane, CharacterPlane characterPlane, ObjectPlane objectPlane)
    {
        PathfindingCell[,,] grid = ReturnNewGrid(gridController);
        PriorityQueue<PathfindingCell> queue = ReturnNewPriorityQueue(grid, fromCell, toCell);
        List<PathfindingCell> backtrackList = new List<PathfindingCell>();

        while (queue.Count > 0)
        {
            PathfindingCell current = queue.Dequeue();
            Debug.Log(current.cell.gridPosition);
            //Debug.DrawLine(current.cell.worldPosition, current.cell.worldPosition + Vector3.up, Color.blue, 10);
            if(DestinationIsReached(current, fromCell, toCell, backtrackList)) { return backtrackList; }

            foreach (GridDirection direction in GridDirection.AllDirections)
            {
                if (direction == GridDirection.Up || direction == GridDirection.Down) { continue; }
                Cell neighborCell = gridController.GetCellFromCellWithDirection(current.cell, direction);
                ObjectBlock objectBlock = objectPlane.GetBlockFromCell(neighborCell);
                CharacterBlock characterBlock = characterPlane.GetBlockFromCell(neighborCell);
                Block terrainBlockMid = terrainPlane.GetBlockFromCell(neighborCell);

                Cell belowCell = gridController.GetCellFromCellWithDirection(neighborCell, GridDirection.Down);
                ObjectBlock belowObject = objectPlane.GetBlockFromCell(belowCell);
                Block terrainBlockBelow = terrainPlane.GetBlockFromCell(belowCell);
                PathfindingCell toBeEnqueued = GetPathfindingCellFromCell(grid, neighborCell);

                if (ExistsCharacter(characterBlock)) { continue; }
                if (ExistsObject(queue,grid, current,toCell, objectBlock)) { continue; }
                if (ExistsObject(queue,grid, current,toCell, belowObject)) { continue; }
                if(ExistsTerrain(queue,grid,toBeEnqueued,toCell, terrainBlockMid)) { continue; }
                else
                    if(!ExistsTerrain(queue,grid,toBeEnqueued,toCell, terrainBlockBelow)) { continue; }

                toBeEnqueued = PathfindingCell.CheckCell(current, toBeEnqueued, toCell);
                if(toBeEnqueued != null) { queue.Enqueue(toBeEnqueued.gCost, toBeEnqueued); }
            }
            GridPathfindingDebugger.SetGrid(grid);
        }
        Debug.Log("Pathfinding: There is no solution");
        return backtrackList;
    }

    private static bool ExistsTerrain(PriorityQueue<PathfindingCell> queue, PathfindingCell[,,] grid, PathfindingCell checkCell, Cell destination, Block terrain)
    {
        if (terrain != null)
            return true;
        return false;
    }

    private static bool ExistsCharacter(CharacterBlock characterBlock)
    {
        if (characterBlock != null)
        {
            return true;
        }
        return false;
    }

    private static bool ExistsObject(PriorityQueue<PathfindingCell> queue, PathfindingCell[,,] grid, PathfindingCell currentCell, Cell destination, ObjectBlock objectBlock)
    {
        if (objectBlock != null)
        {
            if (!objectBlock.isPassable && objectBlock.activationBehaviour.GetComponent<StairBehaviour>() == null)
                return true;
            StairBehaviour stair = objectBlock.activationBehaviour.GetComponent<StairBehaviour>();
            if (stair != null)
            {
                if (stair.UsageCaseInt(objectBlock, currentCell.cell) != -1)
                {
                    Cell start = currentCell.cell;
                    PathfindingCell stairStart = GetPathfindingCellFromCell(grid, start);
                    Cell end = stair.GetCellAtOtherEnd(objectBlock, start);
                    PathfindingCell stairEnd = GetPathfindingCellFromCell(grid, end);
                    PathfindingCell result = PathfindingCell.CheckCell(stairStart, stairEnd, destination);
                    if(result!= null)
                    {
                        queue.Enqueue(result.gCost, result);
                        return true;
                    }
                }
                return true;
            }
        }
        return false;
    }
    private static bool DestinationIsReached(PathfindingCell checkCell, Cell start, Cell end, List<PathfindingCell> returnPathIfTrue)
    {
        if (checkCell.cell == end)
        {
            returnPathIfTrue.Add(checkCell);
            while (returnPathIfTrue[returnPathIfTrue.Count - 1].cell != start)
            {
                returnPathIfTrue.Add(returnPathIfTrue[returnPathIfTrue.Count - 1].parent);
            }
            return true;
        }
        return false;
    }
    private static PathfindingCell GetPathfindingCellFromCell(PathfindingCell[,,] grid, Cell cell)
    {
        PathfindingCell result = null;
        result = grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x];
        return result;
    }
    private static PriorityQueue<PathfindingCell> ReturnNewPriorityQueue(PathfindingCell[,,] pathfindingGrid, Cell fromCell, Cell toCell)
    {
        PriorityQueue<PathfindingCell> queue = new PriorityQueue<PathfindingCell>(true);
        PathfindingCell root = GetPathfindingCellFromCell(pathfindingGrid, fromCell);
        root.CalculateCosts(toCell, 0, true);
        root.parent = null;
        queue.Enqueue(root.fCost, root);
        return queue;
    }
    private static PathfindingCell[,,] ReturnNewGrid(GridController gridController)
    {
        PathfindingCell[,,] pathfindingGrid = new PathfindingCell[gridController.gridSize.y, gridController.gridSize.z, gridController.gridSize.x];
        for (int h = 0; h < gridController.gridSize.y; h++)
        {
            for (int l = 0; l < gridController.gridSize.z; l++)
            {
                for (int w = 0; w < gridController.gridSize.x; w++)
                {
                    pathfindingGrid[h, l, w] = new PathfindingCell(gridController.grid[h, l, w]);
                }
            }
        }
        return pathfindingGrid;
    }

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

        public static PathfindingCell CheckCell(PathfindingCell current, PathfindingCell next, Cell destination)
        {
            if ((current.gCost + next.selfCost) < next.gCost)
            {
                next.CalculateCosts(destination, current.gCost);
                next.parent = current;
                return next;
            }
            return null;
        }

    }

}
