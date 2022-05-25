using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public static List<AIController> AIs = new List<AIController>();
    public static int completedAICount = 0;

    public delegate void AIsAreFinished();
    public static event AIsAreFinished OnAIsAreFinished;

    public int actionSortingID;
    public CharacterBlock controlBlock;
    public bool isFinished;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        AIs.Add(this);
    }

    private void OnDisable()
    {
        AIs.Remove(this);
    }

    public void RotateSelf(Block.Rotations rotation)
    {
        controlBlock.RotateHorizontally(rotation);
    }

    public void MoveForward()
    {
        controlBlock.MoveFoward();
    }

    public static void ResetAIs()
    {
        foreach (AIController ai in AIs)
        {
            ai.isFinished = false;
        }
    }

    public static int GetCompletedAICount()
    {
        int count = 0;
        foreach (AIController ai in AIs)
        {
            if (ai.isFinished)
                count++;
        }
        return count;
    }

    public static void IncrementCompletedAICount()
    {
        completedAICount++;
        if(completedAICount >= AIs.Count)
        {
            if(OnAIsAreFinished != null)
                OnAIsAreFinished();
        }
    }
}
