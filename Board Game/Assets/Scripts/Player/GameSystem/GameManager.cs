using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIController ui;
    public GridController gridController;
    public LevelPlane levelPlane;
    public CharacterPlane characterPlane;
    public ObjectPlane objectPlane;
    public PlayerController playerController;
    public StairsManager stairsManager;
    // Need to prepare grid controller
    // Need to prepare level plane
    // Need to prepare character plane
    public int prepCount = 5;
    public int currentPrepCount;

    public string levelFileNameFormat;
    public LevelDesign currentLevel;
    public delegate void LevelLoadingStarted(LevelDesign levelDesign);
    public static event LevelLoadingStarted OnLevelLoadingStarted;

    public delegate void LevelStarted();
    public static event LevelStarted OnLevelStarted;
    public delegate void LevelFinished();
    public static event LevelFinished OnLevelFinished;

    public delegate void BlockStartedBehaviour(Block behavingBlock);
    public static event BlockStartedBehaviour OnBlockStartedBehaviour;
    public delegate void BlockEndedBehaviour(Block behavingBlock);
    public static event BlockEndedBehaviour OnBlockEndedBehaviour;

    public delegate void PlayerTurnStarted();
    public static event PlayerTurnStarted OnPlayerTurnStarted;
    public delegate void PlayerTurnEnded();
    public static event PlayerTurnEnded OnPlayerTurnEnded;

    public delegate void AITurnStarted();
    public static event AITurnStarted OnAITurnStarted;
    public delegate void AITurnEnded();
    public static event AITurnEnded OnAITurnEnded;

    public delegate void CharacterRanOutOfMoves(CharacterBlock noMovesBlock);
    public static event CharacterRanOutOfMoves OnCharacterRanOutOfMoves;
    public delegate void NextMoveRequired(CharacterBlock needMovesBlock);
    public static event NextMoveRequired OnNextMoveRequired;

    public delegate void CharacterChangedPosition(CharacterBlock movedBlock, Cell toCell);
    public static event CharacterChangedPosition OnCharacterChangedPosition;

    private void Start()
    {
        // Trigger sound effect and animation before entering the main menu
        ui.PlayGameOpeningScene();
        // Main menu at level index 0
        // Actual levels at index 1, 2, 3 , 4, etc
        CallLevelLoadingStarted(0);
    }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized += InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized += InitializeCharacterPlane;
        ObjectPlane.OnObjectPlaneInitialized += InitializeObjectPlane;
        UIController.OnUIControllerInitialized += InitializeUIController;

        AIController.OnAIsAreFinished += CallAITurnEnded;
        OnAITurnEnded += CallPlayerTurnStarted;
        PlayerController.OnPlayerIsFinished += CallPlayerTurnEnded;
        OnPlayerTurnEnded += CallAITurnStarted;

        OnLevelStarted += AIController.ResetAIsStats;
        OnAITurnStarted += AIController.ResetAIsMoves;

    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializeCharacterPlane;
        ObjectPlane.OnObjectPlaneInitialized -= InitializeObjectPlane;
        UIController.OnUIControllerInitialized -= InitializeUIController;

        AIController.OnAIsAreFinished -= CallAITurnEnded;
        OnAITurnEnded -= CallPlayerTurnStarted;
        PlayerController.OnPlayerIsFinished -= CallPlayerTurnEnded;
        OnPlayerTurnEnded -= CallAITurnStarted;

        OnLevelStarted -= AIController.ResetAIsStats;
        OnAITurnStarted -= AIController.ResetAIsMoves;
    }

    

    public void CallLevelLoadingStarted(int levelIndex)
    {
        currentPrepCount = 0;

        if (gridController == null || levelPlane == null || characterPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }
        ui.PlayLevelTransitionScene();
        if(levelIndex == 0) { ui.gameTitle.SetActive(true); }
        else { ui.gameTitle.SetActive(false); }
        currentLevel = new LevelDesign();
        LevelDesign saved = SaveSystem.LoadLevelDesign(levelFileNameFormat + $" {levelIndex}");
        currentLevel.gridHeight = saved.gridHeight;
        currentLevel.gridLength = saved.gridLength;
        currentLevel.gridWidth = saved.gridWidth;
        currentLevel.terrainGrid = saved.terrainGrid;
        currentLevel.characterGrid = saved.characterGrid;
        currentLevel.objectGrid = saved.objectGrid;

        levelPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];
        characterPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];
        objectPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];

        int count = 0;
        for (int h = 0; h < currentLevel.gridHeight; h++)
        {
            for (int l = 0; l < currentLevel.gridLength; l++)
            {
                for (int w = 0; w < currentLevel.gridWidth; w++)
                {
                    levelPlane.idGrid[h, l, w] = currentLevel.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = currentLevel.characterGrid[count];
                    objectPlane.idGrid[h, l, w] = currentLevel.objectGrid[count];
                    count++;
                }
            }
        }

        if(stairsManager != null)
        {
            currentLevel.stairsData = new int[saved.stairsData.Length];
            saved.stairsData.CopyTo(currentLevel.stairsData,0);
            stairsManager.InitializeStairsData(currentLevel.stairsData);
        }

        Vector3Int gridSize = new Vector3Int(currentLevel.gridWidth, currentLevel.gridHeight, currentLevel.gridLength);
        gridController.InitializeGrid(gridSize);
    }

    public void CallLevelStarted()
    {
        Debug.Log("Level Started");
        if (OnLevelStarted != null)
            OnLevelStarted();
    }

    public void CallLevelFinished()
    {
        Debug.Log("Level Finished");
        if (OnLevelFinished != null)
            OnLevelFinished();

    }

    public void CallPlayerTurnStarted()
    {
        Debug.Log("Player Turn Started");
        if (OnPlayerTurnStarted != null)
            OnPlayerTurnStarted();

    }

    public void CallPlayerTurnEnded()
    {
        Debug.Log("Player Turn Ended");
        if (OnPlayerTurnEnded != null)
            OnPlayerTurnEnded();
    }

    public void CallAITurnStarted()
    {
        Debug.Log("AI Turn Started");
        if (OnAITurnStarted != null)
            OnAITurnStarted();
    }

    public void CallAITurnEnded()
    {
        Debug.Log("AI Turn Ended");
        if (OnAITurnEnded != null)
            OnAITurnEnded();
    }

    public void CallCharacterRanOutOfMoves(CharacterBlock noMovesBlock)
    {
        Debug.Log($"{noMovesBlock.name} ran out of moves");
        if (OnCharacterRanOutOfMoves != null)
            OnCharacterRanOutOfMoves(noMovesBlock);
    }

    public void CallNextMoveRequired(CharacterBlock needMovesBlock)
    {
        Debug.Log($"{needMovesBlock.name} need more moves");
        if (OnNextMoveRequired != null)
            OnNextMoveRequired(needMovesBlock);
    }

    public void CallCharacterChangedPosition(CharacterBlock block, Cell toCell)
    {
        Debug.Log($"{block.name}'s position is updated in Character Plane");
        characterPlane.UpdateCharacterPosition(block, toCell);

        // Currently not used Event
        if (OnCharacterChangedPosition != null)
            OnCharacterChangedPosition(block, toCell);
    }

    public void CallBlockStartedBehaviour(Block behavingBlock)
    {
        Debug.Log($"{behavingBlock.name}'s behaviour started");
        if (OnBlockStartedBehaviour != null)
            OnBlockStartedBehaviour(behavingBlock);
    }

    public void CallBlockEndedBehaviour(Block behavingBlock)
    {
        Debug.Log($"{behavingBlock.name}'s behaviour ended");
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

    private void InitializeObjectPlane(ObjectPlane plane)
    {
        objectPlane = plane;
        IncrementPrepCount();
    }

    private void InitializeUIController(UIController ui)
    {
        this.ui = ui;
        IncrementPrepCount();
    }

    private void IncrementPrepCount()
    {
        currentPrepCount++;
        if (currentPrepCount == prepCount)
        {
            // Grid controller
            // Level Plane
            // Character Plane
            // Object Plane
            // Transition
            CallLevelStarted();
        }
    }
}
