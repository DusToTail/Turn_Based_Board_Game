using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridController gridController;
    public LevelPlane levelPlane;
    public CharacterPlane characterPlane;
    public PlayerController playerController;
    // Need to prepare grid controller
    // Need to prepare level plane
    // Need to prepare character plane
    public int prepCount = 3;
    public int currentPrepCount;

    public string fileName;
    public LevelDesign currentLevel;
    public delegate void LevelLoadingStarted(LevelDesign levelDesign);
    public event LevelLoadingStarted OnLevelLoadingStarted;

    public delegate void LevelStarted();
    public event LevelStarted OnLevelStarted;
    public delegate void LevelFinished();
    public event LevelFinished OnLevelFinished;

    public delegate void BlockStartedBehaviour(Block behavingBlock);
    public event BlockStartedBehaviour OnBlockStartedBehaviour;
    public delegate void BlockEndedBehaviour(Block behavingBlock);
    public event BlockEndedBehaviour OnBlockEndedBehaviour;

    public delegate void PlayerTurnStarted();
    public event PlayerTurnStarted OnPlayerTurnStarted;
    public delegate void PlayerTurnEnded();
    public event PlayerTurnEnded OnPlayerTurnEnded;

    public delegate void AITurnStarted();
    public event AITurnStarted OnAITurnStarted;
    public delegate void AITurnEnded();
    public event AITurnEnded OnAITurnEnded;

    public delegate void CharacterRanOutOfMoves(CharacterBlock noMovesBlock);
    public event CharacterRanOutOfMoves OnCharacterRanOutOfMoves;
    public delegate void NextMoveRequired(CharacterBlock needMovesBlock);
    public event NextMoveRequired OnNextMoveRequired;

    public delegate void CharacterChangedPosition(CharacterBlock movedBlock, Cell toCell);
    public event CharacterChangedPosition OnCharacterChangedPosition;

    private void Start()
    {
        CallLevelLoadingStarted();
    }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized += InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized += InitializeCharacterPlane;

        AIController.OnAIsAreFinished += CallAITurnEnded;
        OnAITurnEnded += CallPlayerTurnStarted;
        PlayerController.OnPlayerIsFinished += CallPlayerTurnEnded;
        OnPlayerTurnEnded += CallAITurnStarted;

    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializeCharacterPlane;

        AIController.OnAIsAreFinished -= CallAITurnEnded;
        OnAITurnEnded -= CallPlayerTurnStarted;
        PlayerController.OnPlayerIsFinished -= CallPlayerTurnEnded;
        OnPlayerTurnEnded -= CallAITurnStarted;

    }

    

    public void CallLevelLoadingStarted()
    {
        if (gridController == null || levelPlane == null || characterPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }

        currentLevel = new LevelDesign();
        LevelDesign saved = SaveSystem.LoadLevelDesign(fileName);
        currentLevel.gridHeight = saved.gridHeight;
        currentLevel.gridLength = saved.gridLength;
        currentLevel.gridWidth = saved.gridWidth;
        currentLevel.terrainGrid = saved.terrainGrid;
        currentLevel.characterGrid = saved.characterGrid;

        levelPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];
        characterPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];

        int count = 0;
        for (int h = 0; h < currentLevel.gridHeight; h++)
        {
            for (int l = 0; l < currentLevel.gridLength; l++)
            {
                for (int w = 0; w < currentLevel.gridWidth; w++)
                {
                    levelPlane.idGrid[h, l, w] = currentLevel.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = currentLevel.characterGrid[count];
                    count++;
                }
            }
        }

        Vector3Int gridSize = new Vector3Int(currentLevel.gridWidth, currentLevel.gridHeight, currentLevel.gridLength);
        gridController.InitializeGrid(gridSize);
    }

    public void CallLevelStarted()
    {
        if (OnLevelStarted != null)
            OnLevelStarted();
    }

    public void CallLevelFinished()
    {
        if(OnLevelFinished != null)
            OnLevelFinished();
    }

    public void CallPlayerTurnStarted()
    {
        if (OnPlayerTurnStarted != null)
            OnPlayerTurnStarted();
    }

    public void CallPlayerTurnEnded()
    {
        if (OnPlayerTurnEnded != null)
            OnPlayerTurnEnded();
    }

    public void CallAITurnStarted()
    {
        if (OnAITurnStarted != null)
            OnAITurnStarted();
    }

    public void CallAITurnEnded()
    {
        if (OnAITurnEnded != null)
            OnAITurnEnded();
    }

    public void CallCharacterRanOutOfMoves(CharacterBlock noMovesBlock)
    {
        if(OnCharacterRanOutOfMoves != null)
            OnCharacterRanOutOfMoves(noMovesBlock);
    }

    public void CallNextMoveRequired(CharacterBlock needMovesBlock)
    {
        if(OnNextMoveRequired != null)
            OnNextMoveRequired(needMovesBlock);
    }

    public void CallCharacterChangedPosition(CharacterBlock block, Cell toCell)
    {
        characterPlane.UpdateCharacterPosition(block, toCell);

        // Currently not used Event
        if(OnCharacterChangedPosition != null)
            OnCharacterChangedPosition(block, toCell);
    }

    public void CallBlockStartedBehaviour(Block behavingBlock)
    {
        if (OnBlockStartedBehaviour != null)
            OnBlockStartedBehaviour(behavingBlock);
    }

    public void CallBlockEndedBehaviour(Block behavingBlock)
    {
        if (OnBlockEndedBehaviour != null)
            OnBlockEndedBehaviour(behavingBlock);
    }

    private void InitializeGridController(GridController gridController)
    {
        this.gridController = gridController;
        IncrementPrepCount();
    }

    private void InitializeLevelPlane(LevelPlane plane)
    {
        levelPlane = plane;
        IncrementPrepCount();

    }

    private void InitializeCharacterPlane(CharacterPlane plane)
    {
        characterPlane = plane;
        IncrementPrepCount();

    }

    private void IncrementPrepCount()
    {
        currentPrepCount++;
        if (currentPrepCount == prepCount)
        {
            CallLevelStarted();
        }
    }
}
