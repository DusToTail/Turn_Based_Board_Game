using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelClearUI : MonoBehaviour
{
    public GameObject[] options;
    private int _currentIndex;
    private int _previousIndex;
    private GameManager _gameManager;

    private void OnEnable()
    {
        GameManager.OnLevelStarted += SetInactive;
        GameManager.OnLevelFinished += SetActive;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStarted -= SetInactive;
        GameManager.OnLevelFinished -= SetActive;
    }

    private void Start()
    {
        if(options != null && options.Length > 0)
        {
            _previousIndex = 0;
            _currentIndex = 0;
            options[0].transform.Find("Arrow").gameObject.SetActive(true);
            for(int i = 1; i < options.Length; i++)
                options[i].transform.Find("Arrow").gameObject.SetActive(false);
        }
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (!options[_currentIndex].gameObject.activeSelf) { return; }

        if(Input.GetKeyUp(KeyCode.A))
            MoveToOption(true);
        else if(Input.GetKeyUp(KeyCode.D))
            MoveToOption(false);

        if(Input.GetKeyDown(KeyCode.Space))
            LoadLevel();
    }

    private void SetActive()
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);
        Debug.Log("Level Clear UI ON");
    }

    private void SetInactive()
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        Debug.Log("Level Clear UI OFF");
    }

    private void LoadLevel()
    {
        if (_currentIndex == 0)
            _gameManager.CallLevelLoadingStarted(_gameManager.loadLevelIndex);
        else
            _gameManager.CallLevelLoadingStarted(0);
        SetInactive();
    }

    private void MoveToOption(bool toLeft)
    {
        if(toLeft)
        {
            if(_previousIndex != _currentIndex) { _previousIndex = _currentIndex; }
            _currentIndex -= 1;
            if(_currentIndex < 0) { _currentIndex = 0; }

            options[_previousIndex].transform.Find("Arrow").gameObject.SetActive(false);
            options[_currentIndex].transform.Find("Arrow").gameObject.SetActive(true);
        }
        else
        {
            if (_previousIndex != _currentIndex) { _previousIndex = _currentIndex; }
            _currentIndex += 1;
            if (_currentIndex > options.Length - 1) { _currentIndex = options.Length - 1; }

            options[_previousIndex].transform.Find("Arrow").gameObject.SetActive(false);
            options[_currentIndex].transform.Find("Arrow").gameObject.SetActive(true);
        }
    }

}
