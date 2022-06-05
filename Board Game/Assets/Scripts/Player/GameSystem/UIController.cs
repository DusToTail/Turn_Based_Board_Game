using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Canvas canvas;
    public GameObject gameTitle;
    public Image blackScreen;

    public delegate void UIControllerInitialized(UIController ui);
    public static event UIControllerInitialized OnUIControllerInitialized;

    private GameManager gameManager;

    private bool sceneReady;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        CharacterPlane.OnCharacterPlaneInitialized += SetSceneReady;
    }

    private void OnDisable()
    {
        CharacterPlane.OnCharacterPlaneInitialized -= SetSceneReady;
    }

    public void PlayGameOpeningScene()
    {
        StartCoroutine(GameOpeningCoroutine());
    }

    public void PlayLevelTransitionScene()
    {
        StartCoroutine(LevelTransitionCoroutine());
    }

    private IEnumerator GameOpeningCoroutine()
    {
        sceneReady = false;
        gameTitle.SetActive(false);

        // The screen lighting intensity is slowly increased
        blackScreen.color = Color.black;
        yield return new WaitUntil(() => sceneReady);
        gameTitle.SetActive(true);
        float t = 1;
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            if(t < 0)
            {
                t = 0;
                blackScreen.color = new Color(0, 0, 0, t);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t -= 0.2f;
        }
    }

    private IEnumerator LevelTransitionCoroutine()
    {
        sceneReady = false;

        // The screen lighting intensity is slowly increased
        blackScreen.color = Color.black;

        yield return new WaitUntil(() => sceneReady);

        float t = 1;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (t < 0)
            {
                t = 0;
                blackScreen.color = new Color(0, 0, 0, t);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t -= 0.2f;
        }

        OnUIControllerInitialized(this);
        Debug.Log("UI Controller Initialized");
    }

    private void SetSceneReady(CharacterPlane plane)
    {
        sceneReady = true;
    }
}
