using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridController gridController { get; private set; }
    public LevelPlane levelPlane { get; private set; }
    public CharacterPlane characterPlane { get; private set; }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized += InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized += InitializeCharacterPlane;

    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializeCharacterPlane;
    }

    private void InitializeGridController(GridController gridController)
    {
        this.gridController = gridController;
    }

    private void InitializeLevelPlane(LevelPlane plane)
    {
        levelPlane = plane;
    }

    private void InitializeCharacterPlane(CharacterPlane plane)
    {
        characterPlane = plane;
    }
}
