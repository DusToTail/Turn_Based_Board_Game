using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages cooldown icons and add/remove it when tracking character's cooldown increase/decrease
/// </summary>
public class RevealSkillUI : MonoBehaviour
{
    public GameObject prefabIcon;
    public RevealAreaSkill revealSkill;
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
        GameManager.OnLevelStarted += InitializeRevealSkillUI;
        RevealAreaSkill.OnCooldownDecremented += AddIcon;
        RevealAreaSkill.OnCooldownRewinded += RemoveAllIcons;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStarted -= InitializeRevealSkillUI;
        RevealAreaSkill.OnCooldownDecremented -= AddIcon;
        RevealAreaSkill.OnCooldownRewinded -= RemoveAllIcons;
    }

    private void AddIcon()
    {
        if(currentIcons == revealSkill.coolDown) { return; }
        transform.GetChild(currentIcons).GetComponent<RevealSkillIcon>().OnAdded();
        currentIcons++;
    }

    private void RemoveAllIcons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<RevealSkillIcon>().OnRemoved();
            currentIcons--;
        }
    }

    private void InitializeRevealSkillUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        currentIcons = 0;
        revealSkill = gameManager.playerController.revealSkill;
        for (int i = 0; i < revealSkill.coolDown; i++)
        {
            GameObject icon = Instantiate(prefabIcon);
            RectTransform rectTransform = icon.transform as RectTransform;
            rectTransform.SetParent(transform);
            rectTransform.anchoredPosition3D = Vector3.zero + Vector3.down * distanceInPixels * i;
            currentIcons++;
        }
    }
}
