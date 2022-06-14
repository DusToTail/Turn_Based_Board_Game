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

    [SerializeField]
    private float distanceInPixels;
    private int currentHearts;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        GameManager.OnLevelStarted += InitializeHealthUI;
        CharacterBlock.OnDamageTaken += DecrementHealth;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStarted -= InitializeHealthUI;
        CharacterBlock.OnDamageTaken -= DecrementHealth;
    }

    private void DecrementHealth(CharacterBlock trackingBlock, int damageAmount)
    {
        if(trackingBlock != trackingCharacter) { return; }
        for(int i = 0; i < damageAmount; i++)
        {
            if(currentHearts == 0) { continue; }
            transform.GetChild(currentHearts - 1).GetComponent<HealthIcon>().OnRemoved();
            currentHearts--;
        }
    }

    private void IncrementHealth(CharacterBlock trackingBlock, int healAmount)
    {
        transform.GetChild(trackingCharacter.curHealth - 1).GetComponent<HealthIcon>().OnAdded();
    }

    private void InitializeHealthUI()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        currentHearts = 0;
        trackingCharacter = gameManager.playerController.playerBlock;
        for(int i = 0; i < trackingCharacter.curHealth; i++)
        {
            GameObject icon = Instantiate(prefabIcon);
            RectTransform rectTransform = icon.transform as RectTransform;
            rectTransform.SetParent(transform);
            rectTransform.anchoredPosition3D = Vector3.zero + Vector3.right * distanceInPixels * i;
            currentHearts++;
        }
    }
}
