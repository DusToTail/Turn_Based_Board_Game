using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A controller for the game's UI canvas and its children
/// </summary>
public class UIController : MonoBehaviour
{
    public Canvas canvas;
    public GameObject clearMenu;
    public GameObject failMenu;
    public Image blackScreen;
    public HealthUI healthUI;
    public MovesUI movesUI;
    public RevealSkillUI revealSkillUI;
    public TipUI tipUI;

    public delegate void UIControllerInitialized(UIController ui);
    public static event UIControllerInitialized OnUIControllerInitialized;

    private GameManager _gameManager;
    private bool _sceneReady;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        CharacterPlane.OnCharacterPlaneInitialized += SetSceneReady;
    }

    private void OnDisable()
    {
        CharacterPlane.OnCharacterPlaneInitialized -= SetSceneReady;
    }

    public void PlayGameOpeningScene() => StartCoroutine(GameOpeningCoroutine());
    public void PlayLevelTransitionScene() => StartCoroutine(LevelTransitionCoroutine());
    public void PlayLevelClearScene() => StartCoroutine(LevelClearCoroutine());
    public void PlayLevelFailScene() => StartCoroutine(LevelFailCoroutine());

    private IEnumerator GameOpeningCoroutine()
    {
        _sceneReady = false;

        // The alpha is slowly decreased
        blackScreen.color = Color.black;
        yield return new WaitUntil(() => _sceneReady);

        float t = 1;
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            if(t < 0)
            {
                blackScreen.color = new Color(0, 0, 0, 0);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t -= 0.25f;
        }
    }

    private IEnumerator LevelTransitionCoroutine()
    {
        _sceneReady = false;

        // The alpha is slowly decreased
        blackScreen.color = Color.black;

        yield return new WaitUntil(() => _sceneReady);

        float t = 1;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (t < 0)
            {
                blackScreen.color = new Color(0, 0, 0, 0);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t -= 0.25f;
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
                blackScreen.color = new Color(0, 0, 0, 1);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t += 0.25f;
        }

        // The alpha is slowly increased
        t = 1;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (t < 0)
            {
                blackScreen.color = new Color(0, 0, 0, 0);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t -= 0.25f;
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
                blackScreen.color = new Color(0, 0, 0, 1);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t += 0.25f;
        }

        // The alpha is slowly increased
        t = 1;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (t < 0)
            {
                blackScreen.color = new Color(0, 0, 0, 0);
                break;
            }
            blackScreen.color = new Color(0, 0, 0, t);
            t -= 0.25f;
        }

        Debug.Log("UI: level fail finished");
    }

    private void SetSceneReady(CharacterPlane plane) { _sceneReady = true; }
}
