using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage other systems and facilitate communication between them
/// </summary>
public class GameManager : MonoBehaviour
{
    public UIController ui;
    public GridController gridController;
    public TerrainPlane terrainPlane;
    public CharacterPlane characterPlane;
    public ObjectPlane objectPlane;
    public PlayerController playerController;
    public StairsManager stairsManager;
    public RemoteTriggerManager remoteTriggerManager;
    public RemoteDoorManager remoteDoorManager;

    // Preparations:
    // Grid Controller Åö
    // Level Plane Å´
    // Character Plane Å´
    // Object Plane Å´
    // UIÅ@Åö
    public int prepCount = 5;
    public int currentPrepCount;

    public int currentLevelIndex;
    public string levelFileNameFormat;
    public LevelDesign currentLevel;

    // Levels
    public delegate void LevelLoadingStarted(LevelDesign levelDesign);
    public static event LevelLoadingStarted OnLevelLoadingStarted;
    public delegate void LevelStarted();
    public static event LevelStarted OnLevelStarted;
    public delegate void LevelFinished();
    public static event LevelFinished OnLevelFinished;
    public delegate void LevelFailed();
    public static event LevelFailed OnLevelFailed;

    
    // Turns
    public delegate void PlayerTurnStarted();
    public static event PlayerTurnStarted OnPlayerTurnStarted;
    public delegate void PlayerTurnEnded();
    public static event PlayerTurnEnded OnPlayerTurnEnded;

    public delegate void AITurnStarted();
    public static event AITurnStarted OnAITurnStarted;
    public delegate void AITurnEnded();
    public static event AITurnEnded OnAITurnEnded;

    // Characters
    public delegate void CharacterRanOutOfMoves(CharacterBlock noMovesBlock);
    public static event CharacterRanOutOfMoves OnCharacterRanOutOfMoves;
    public delegate void NextMoveRequired(CharacterBlock needMovesBlock);
    public static event NextMoveRequired OnNextMoveRequired;

    // Blocks
    public delegate void BlockStartedBehaviour(Block behavingBlock);
    public static event BlockStartedBehaviour OnBlockStartedBehaviour;
    public delegate void BlockEndedBehaviour(Block behavingBlock);
    public static event BlockEndedBehaviour OnBlockEndedBehaviour;


    private void Start()
    {
        // Trigger sound effect and animation before entering the main menu
        ui.PlayGameOpeningScene();
        // Main menu at level index 0
        // Actual levels at index 1, 2, 3 , 4, etc
        CallLevelLoadingStarted(currentLevelIndex);
    }

    private void OnEnable()
    {
        // Inverse Dependency Injection
        GridController.OnGridInitialized += InitializeGridController;
        TerrainPlane.OnLevelPlaneInitialized += InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized += InitializeCharacterPlane;
        ObjectPlane.OnObjectPlaneInitialized += InitializeObjectPlane;
        UIController.OnUIControllerInitialized += InitializeUIController;

        AIController.OnAIsAreFinished += CallAITurnEnded;
        OnAITurnEnded += CallPlayerTurnStarted;
        PlayerController.OnPlayerIsFinished += CallPlayerTurnEnded;
        OnPlayerTurnEnded += CallAITurnStarted;

        // Subscribe here instead, because there can be none or many instances for AIController
        OnLevelStarted += AIController.ResetAIsStats;
        OnAITurnStarted += AIController.ResetAIsMoves;

    }

    private void OnDisable()
    {
        // Inverse Dependency Injection
        GridController.OnGridInitialized -= InitializeGridController;
        TerrainPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
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

    
    /// <summary>
    /// Start loading a level with levelIndex
    /// </summary>
    /// <param name="levelIndex"></param>
    public void CallLevelLoadingStarted(int levelIndex)
    {
        currentPrepCount = 0;
        currentLevelIndex = levelIndex;
        if (gridController == null || terrainPlane == null || characterPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }
        // Block player's view
        ui.PlayLevelTransitionScene();

        #region Data Initialization
        // Load data from saved files
        currentLevel = new LevelDesign();
        LevelDesign saved = SaveSystem.LoadLevelDesign(levelFileNameFormat + $" {levelIndex}");
        // grid size
        currentLevel.gridHeight = saved.gridHeight;
        currentLevel.gridLength = saved.gridLength;
        currentLevel.gridWidth = saved.gridWidth;
        // grid of each plane in 1 dimensional array
        currentLevel.terrainGrid = saved.terrainGrid;
        currentLevel.characterGrid = saved.characterGrid;
        currentLevel.objectGrid = saved.objectGrid;
        // block specifics
        currentLevel.rotations = saved.rotations;
        currentLevel.remoteTriggersData = new int[saved.remoteTriggersData.Length];
        saved.remoteTriggersData.CopyTo(currentLevel.remoteTriggersData, 0);
        currentLevel.remoteDoorsData = new int[saved.remoteDoorsData.Length];
        saved.remoteDoorsData.CopyTo(currentLevel.remoteDoorsData, 0);

        // Initialize planes' 3 dimensional id grid in scene
        // id is used to identify each block uniquely
        terrainPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];
        characterPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];
        objectPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];
        int count = 0;
        for (int h = 0; h < currentLevel.gridHeight; h++)
        {
            for (int l = 0; l < currentLevel.gridLength; l++)
            {
                for (int w = 0; w < currentLevel.gridWidth; w++)
                {
                    // Transfer data from 1D arrays to 3D arrays
                    terrainPlane.idGrid[h, l, w] = currentLevel.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = currentLevel.characterGrid[count];
                    objectPlane.idGrid[h, l, w] = currentLevel.objectGrid[count];
                    count++;
                }
            }
        }

        remoteTriggerManager?.InitializeRemoteTriggersData(currentLevel.remoteTriggersData);
        remoteDoorManager?.InitializeRemoteDoorsData(currentLevel.remoteDoorsData);
        #endregion

        // Start loading the level with the initialized data transfered to the corresponding planes
        if (OnLevelLoadingStarted != null)
            OnLevelLoadingStarted(currentLevel);
        gridController.InitializeGrid(currentLevel);
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

    public void CallLevelFailed()
    {
        Debug.Log("Level Failed");
        if (OnLevelFailed != null)
            OnLevelFailed();
    }

    public void CallPlayerTurnStarted()
    {
        Debug.Log("Player Turn Started");
        if(ui.tipUI != null)
            ui.tipUI.DisplayPlayerText();
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
        if (ui.tipUI != null)
            ui.tipUI.DisplayWaitingText();
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

    private void InitializeGridController(GridController gridController, LevelDesign levelDesign)
    {
        this.gridController = gridController;
        IncrementPrepCount();
    }

    private void InitializeLevelPlane(TerrainPlane plane)
    {
        terrainPlane = plane;
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
            CallLevelStarted();
    }
}
