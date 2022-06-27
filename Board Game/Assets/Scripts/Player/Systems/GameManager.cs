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

    public LevelDesign currentLevelDesign;

    public const string levelFileNameFormat = "Level";
    public int loadLevelIndex = 0;
    // Preparations:
    // Grid Controller Åö
    // Level Plane Å´
    // Character Plane Å´
    // Object Plane Å´
    // UIÅ@Åö
    public const int prepCount = 5;
    private int _currentPrepCount;

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

    private void Start()
    {
        ui.PlayGameOpeningScene();
        // Main menu at level index 0
        // Actual levels at index 1, 2, 3 , 4, etc
        CallLevelLoadingStarted(loadLevelIndex);
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

    
    public void CallLevelLoadingStarted(int levelIndex)
    {
        _currentPrepCount = 0;
        loadLevelIndex = levelIndex;
        if (gridController == null || terrainPlane == null || characterPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }
        // Block player's view
        ui.PlayLevelTransitionScene();

        #region Data Initialization
        // Load data from saved files
        currentLevelDesign = new LevelDesign();
        LevelDesign saved = SaveSystem.LoadLevelDesign(levelFileNameFormat + $" {levelIndex}");
        // grid size
        currentLevelDesign.gridHeight = saved.gridHeight;
        currentLevelDesign.gridLength = saved.gridLength;
        currentLevelDesign.gridWidth = saved.gridWidth;
        // grid of each plane in 1 dimensional array
        currentLevelDesign.terrainGrid = saved.terrainGrid;
        currentLevelDesign.characterGrid = saved.characterGrid;
        currentLevelDesign.objectGrid = saved.objectGrid;
        // block specifics
        currentLevelDesign.rotations = saved.rotations;
        currentLevelDesign.remoteTriggersData = new int[saved.remoteTriggersData.Length];
        saved.remoteTriggersData.CopyTo(currentLevelDesign.remoteTriggersData, 0);
        currentLevelDesign.remoteDoorsData = new int[saved.remoteDoorsData.Length];
        saved.remoteDoorsData.CopyTo(currentLevelDesign.remoteDoorsData, 0);

        // Initialize planes' 3 dimensional id grid in scene
        // id is used to identify each block uniquely
        terrainPlane.idGrid = new int[currentLevelDesign.gridHeight, currentLevelDesign.gridLength, currentLevelDesign.gridWidth];
        characterPlane.idGrid = new int[currentLevelDesign.gridHeight, currentLevelDesign.gridLength, currentLevelDesign.gridWidth];
        objectPlane.idGrid = new int[currentLevelDesign.gridHeight, currentLevelDesign.gridLength, currentLevelDesign.gridWidth];
        int count = 0;
        for (int h = 0; h < currentLevelDesign.gridHeight; h++)
        {
            for (int l = 0; l < currentLevelDesign.gridLength; l++)
            {
                for (int w = 0; w < currentLevelDesign.gridWidth; w++)
                {
                    // Transfer data from 1D arrays to 3D arrays
                    terrainPlane.idGrid[h, l, w] = currentLevelDesign.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = currentLevelDesign.characterGrid[count];
                    objectPlane.idGrid[h, l, w] = currentLevelDesign.objectGrid[count];
                    count++;
                }
            }
        }

        remoteTriggerManager?.InitializeRemoteTriggersData(currentLevelDesign.remoteTriggersData);
        remoteDoorManager?.InitializeRemoteDoorsData(currentLevelDesign.remoteDoorsData);
        #endregion

        // Start loading the level with the initialized data transfered to the corresponding planes
        if (OnLevelLoadingStarted != null)
            OnLevelLoadingStarted(currentLevelDesign);
        gridController.InitializeGrid(currentLevelDesign);
    }

    public void CallLevelStarted()
    {
        Debug.Log("Game Manager: Level Started");
        if (OnLevelStarted != null)
            OnLevelStarted();
    }

    public void CallLevelFinished()
    {
        Debug.Log("Game Manager: Level Finished");
        if (OnLevelFinished != null)
            OnLevelFinished();
    }

    public void CallLevelFailed()
    {
        Debug.Log("Game Manager: Level Failed");
        if (OnLevelFailed != null)
            OnLevelFailed();
    }
    public void CallPlayerTurnStarted()
    {
        Debug.Log("Game Manager: Player Turn Started");
        if(ui.tipUI != null)
            ui.tipUI.DisplayPlayerText();
        if (OnPlayerTurnStarted != null)
            OnPlayerTurnStarted();
    }

    public void CallPlayerTurnEnded()
    {
        Debug.Log("Game Manager: Player Turn Ended");
        if (OnPlayerTurnEnded != null)
            OnPlayerTurnEnded();
    }

    public void CallAITurnStarted()
    {
        Debug.Log("Game Manager: AI Turn Started");
        if (ui.tipUI != null)
            ui.tipUI.DisplayWaitingText();
        if (OnAITurnStarted != null)
            OnAITurnStarted();
    }

    public void CallAITurnEnded()
    {
        Debug.Log("Game Manager: AI Turn Ended");
        if (OnAITurnEnded != null)
            OnAITurnEnded();
    }

    public void CallCharacterRanOutOfMoves(CharacterBlock noMovesBlock)
    {
        Debug.Log($"Game Manager: {noMovesBlock.name} ran out of moves");
        if (OnCharacterRanOutOfMoves != null)
            OnCharacterRanOutOfMoves(noMovesBlock);
    }

    public void CallCharacterRequiresNextMove(CharacterBlock needMovesBlock)
    {
        Debug.Log($"Game Manager: {needMovesBlock.name} need more moves");
        if (OnNextMoveRequired != null)
            OnNextMoveRequired(needMovesBlock);
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
        _currentPrepCount++;
        if (_currentPrepCount == prepCount)
            CallLevelStarted();
    }
}
