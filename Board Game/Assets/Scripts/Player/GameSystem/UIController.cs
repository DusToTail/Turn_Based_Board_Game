using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Canvas canvas;

    public GameObject clearMenu;
    public GameObject failMenu;

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

    public void PlayLevelClearScene()
    {
        StartCoroutine(LevelClearCoroutine());
    }

    public void PlayLevelFailScene()
    {
        StartCoroutine(LevelFailCoroutine());
    }

    private IEnumerator GameOpeningCoroutine()
    {
        sceneReady = false;

        // The alpha is slowly decreased
        blackScreen.color = Color.black;
        yield return new WaitUntil(() => sceneReady);

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

        // The alpha is slowly decreased
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

    private IEnumerator LevelClearCoroutine()
    {

        // The alpha is slowly decreased
        float t = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (t >1)
            {
                t = 1;
                blackScreen.color = new Color(0, 0, 0, t);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t += 0.2f;
        }

        // Show a scene where the statue is with the prize, surrounded by stalking eyes in the dark
        // The alpha is slowly increased
        t = 1;
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

        Debug.Log("UI: level clear finished");
    }

    private IEnumerator LevelFailCoroutine()
    {

        // The alpha is slowly decreased
        float t = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (t > 1)
            {
                t = 1;
                blackScreen.color = new Color(0, 0, 0, t);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t += 0.2f;
        }

        // Show a scene where the statue is broken, surrounded by stalking eyes in the dark
        // The alpha is slowly increased
        t = 1;
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

        Debug.Log("UI: level fail finished");
    }

    private void SetSceneReady(CharacterPlane plane)
    {
        sceneReady = true;
    }
}
