using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages move icons and add/remove it when tracking character's move (action point) increase/decrease
/// </summary>
public class MovesUI : MonoBehaviour
{
    public GameObject prefabIcon;
    public CharacterBlock trackingCharacter;
    public GameManager gameManager;

    public bool isFinished { get; set; }

    [SerializeField] private float distanceInPixels;
    private int _currentIcons;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        GameManager.OnLevelLoadingStarted += DestroyAllIconsOnLevelLoaded;
        GameManager.OnLevelStarted += InitializeMovesUI;
        GameManager.OnLevelFailed += RemoveAllIcons;
        GameManager.OnNextMoveRequired += DecrementMove;
        GameManager.OnCharacterRanOutOfMoves += DecrementMove;
        GameManager.OnPlayerTurnStarted += ResetMove;
    }

    private void OnDisable()
    {
        GameManager.OnLevelLoadingStarted -= DestroyAllIconsOnLevelLoaded;
        GameManager.OnLevelStarted -= InitializeMovesUI;
        GameManager.OnLevelFailed -= RemoveAllIcons;
        GameManager.OnNextMoveRequired -= DecrementMove;
        GameManager.OnCharacterRanOutOfMoves -= DecrementMove;
        GameManager.OnPlayerTurnStarted -= ResetMove;
    }

    private void DecrementMove(CharacterBlock trackingBlock)
    {
        if (trackingBlock != trackingCharacter) { return; }
        if (_currentIcons == 0) { return; }
        isFinished = false;
        transform.GetChild(_currentIcons - 1).GetComponent<MoveIcon>().OnRemoved();
        _currentIcons--;
    }

    private void ResetMove()
    {
        isFinished = false;
        _currentIcons = transform.childCount;
        for(int i = 0; i < _currentIcons; i++)
            transform.GetChild(i).GetComponent<MoveIcon>().OnAdded();
    }

    private void RemoveAllIcons()
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<MoveIcon>().OnRemoved();
        _currentIcons = 0;
    }

    private void DestroyAllIconsOnLevelLoaded(LevelDesign level)
        => DestroyAllIcons();

    private void DestroyAllIcons()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        _currentIcons = 0;
    }

    private void InitializeMovesUI()
    {
        trackingCharacter = gameManager.playerController.playerBlock;
        for (int i = 0; i < trackingCharacter.CurMovesLeft; i++)
        {
            GameObject icon = Instantiate(prefabIcon);
            RectTransform rectTransform = icon.transform as RectTransform;
            rectTransform.SetParent(transform);
            rectTransform.anchoredPosition3D = Vector3.zero + Vector3.left * distanceInPixels * i;
            _currentIcons++;
        }
        isFinished = true;
    }
}
