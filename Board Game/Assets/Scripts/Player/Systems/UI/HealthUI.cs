using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages health icons and add/remove it when tracking character's heal increase/decrease
/// </summary>
public class HealthUI : MonoBehaviour
{
    public GameObject prefabIcon;
    public CharacterBlock trackingCharacter;
    public GameManager gameManager;

    [SerializeField] private float distanceInPixels;
    private int _currentIcons;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        GameManager.OnLevelLoadingStarted += DestroyAllIconsOnLevelLoaded;
        GameManager.OnLevelStarted += InitializeHealthUI;
        CharacterBlock.OnDamageTaken += DecrementHealth;
    }

    private void OnDisable()
    {
        GameManager.OnLevelLoadingStarted -= DestroyAllIconsOnLevelLoaded;
        GameManager.OnLevelStarted -= InitializeHealthUI;
        CharacterBlock.OnDamageTaken -= DecrementHealth;
    }

    private void DecrementHealth(CharacterBlock trackingBlock, int damageAmount)
    {
        if(trackingBlock != trackingCharacter) { return; }
        for(int i = 0; i < damageAmount; i++)
        {
            if(_currentIcons == 0) { continue; }
            transform.GetChild(_currentIcons - 1).GetComponent<HealthIcon>().OnRemoved();
            _currentIcons--;
        }
    }

    private void DestroyAllIconsOnLevelLoaded(LevelDesign level) => DestroyAllIcons();
    private void DestroyAllIcons()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
        _currentIcons = 0;
    }

    private void InitializeHealthUI()
    {
        _currentIcons = 0;
        DestroyAllIcons();
        trackingCharacter = gameManager.playerController.playerBlock;
        for(int i = 0; i < trackingCharacter.CurHealth; i++)
        {
            GameObject icon = Instantiate(prefabIcon);
            RectTransform rectTransform = icon.transform as RectTransform;
            rectTransform.SetParent(transform);
            rectTransform.anchoredPosition3D = Vector3.zero + Vector3.right * distanceInPixels * i;
            _currentIcons++;
        }
    }
}
