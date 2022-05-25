using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public static List<AIController> AIs = new List<AIController>();
    public static List<AIController> finishedAIs = new List<AIController>();
    public static int completedAICount = 0;

    public delegate void AIsAreFinished();
    public static event AIsAreFinished OnAIsAreFinished;

    public delegate void AIIsFinished(AIController finishedAI);
    public static event AIIsFinished OnAIIsFinished;

    public int actionSortingID;
    public CharacterBlock controlBlock;
    public bool isFinished;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.OnAITurnStarted += ResetAIs;
        _gameManager.OnCharacterRanOutOfMoves += CallAIIsFinished;
        _gameManager.OnNextMoveRequired += ContinueToMoveIfAllowed;

        controlBlock = GetComponent<CharacterBlock>();
    }

    private void OnDestroy()
    {
        _gameManager.OnAITurnStarted -= ResetAIs;
        _gameManager.OnCharacterRanOutOfMoves -= CallAIIsFinished;
        _gameManager.OnNextMoveRequired -= ContinueToMoveIfAllowed;
    }

    private void OnEnable()
    {
        AIs.Add(this);
        AIs.Sort(CompareActionSortingID);
        AIs.TrimExcess();
    }

    private void OnDisable()
    {
        AIs.Remove(this);
        AIs.Sort(CompareActionSortingID);
        AIs.TrimExcess();
    }

    public void PerformAction()
    {
        Debug.Log($"AI perform something");
        // Need to program an AI and strategies
        MoveForward();
    }

    public void RotateSelf(Block.Rotations rotation)
    {
        controlBlock.RotateHorizontally(rotation);
    }

    public void MoveForward()
    {
        controlBlock.MoveFoward();
    }

    public void ContinueToMoveIfAllowed(CharacterBlock compareBlock)
    {
        if (compareBlock != controlBlock) { return; }
        PerformAction();
    }

    public void CallAIIsFinished(CharacterBlock compareBlock)
    {
        if(compareBlock != controlBlock) { return; }
        IncrementCompletedAICount();
    }

    public void IncrementCompletedAICount()
    {
        completedAICount++;
        finishedAIs.Add(this);
        if (completedAICount >= AIs.Count)
        {
            if (OnAIsAreFinished != null)
                OnAIsAreFinished();
        }
    }

    public static void ResetAIs()
    {
        foreach (AIController ai in AIs)
        {
            ai.isFinished = false;
            ai.controlBlock.ResetCurrentMoves();
        }
    }

    public static int GetCompletedAICount()
    {
        return finishedAIs.Count;
    }

    private static int CompareActionSortingID(AIController A, AIController B)
    {
        if(A == null)
        {
            if(B == null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else if(B == null)
        {
            if (A == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            if(A.actionSortingID > B.actionSortingID)
            {
                return 1;
            }
            else if(A.actionSortingID < B.actionSortingID)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
