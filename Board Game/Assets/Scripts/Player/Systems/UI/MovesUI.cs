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

    [SerializeField]
    private float distanceInPixels;
    private int currentIcons;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        GameManager.OnLevelStarted += InitializeMovesUI;
        GameManager.OnBlockEndedBehaviour += DecrementMove;
        GameManager.OnPlayerTurnStarted += ResetMove;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStarted -= InitializeMovesUI;
        GameManager.OnBlockEndedBehaviour -= DecrementMove;
        GameManager.OnPlayerTurnStarted -= ResetMove;
    }

    private void DecrementMove(Block trackingBlock)
    {
        if (trackingBlock != trackingCharacter) { return; }
        if (currentIcons == 0) { return; }
        transform.GetChild(currentIcons - 1).GetComponent<MoveIcon>().OnRemoved();
        currentIcons--;
    }

    private void ResetMove()
    {
        currentIcons = transform.childCount;
        for(int i = 0; i < currentIcons; i++)
            transform.GetChild(i).GetComponent<MoveIcon>().OnAdded();
    }

    private void InitializeMovesUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        currentIcons = 0;
        trackingCharacter = gameManager.playerController.playerBlock;
        for (int i = 0; i < trackingCharacter.curMovesLeft; i++)
        {
            GameObject icon = Instantiate(prefabIcon);
            RectTransform rectTransform = icon.transform as RectTransform;
            rectTransform.SetParent(transform);
            rectTransform.anchoredPosition3D = Vector3.zero + Vector3.left * distanceInPixels * i;
            currentIcons++;
        }
    }
}
