using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public static List<AIController> AIs = new List<AIController>();
    public static int completedAICount = 0;

    public delegate void AIsAreFinished();
    public static event AIsAreFinished OnAIsAreFinished;

    public delegate void AIIsFinished(AIController finishedAI);
    public static event AIIsFinished OnAIIsFinished;


    public int actionSortingID;
    public CharacterBlock controlBlock;

    public CharacterBlock target;

    private GameManager _gameManager;
    private PlayerController _playerController;
    private BehaviourTreeRunner _behaviourTreeRunner;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _playerController = FindObjectOfType<PlayerController>();
        _behaviourTreeRunner = GetComponent<BehaviourTreeRunner>();

        target = _playerController.playerBlock;
        controlBlock = GetComponent<CharacterBlock>();

        
    }

    private void OnEnable()
    {
        GameManager.OnCharacterRanOutOfMoves += CallAIIsFinished;
        GameManager.OnNextMoveRequired += ContinueToMoveIfAllowed;

        AIs.Add(this);
        AIs.Sort(CompareActionSortingID);
        AIs.TrimExcess();

        OnAIIsFinished += IncrementCompletedAICount;
    }

    private void OnDisable()
    {
        GameManager.OnCharacterRanOutOfMoves -= CallAIIsFinished;
        GameManager.OnNextMoveRequired -= ContinueToMoveIfAllowed;

        AIs.Remove(this);
        AIs.Sort(CompareActionSortingID);
        AIs.TrimExcess();

        OnAIIsFinished -= IncrementCompletedAICount;
    }

    public void PerformAction()
    {
        Debug.Log($"AI {name} performs something");
        _behaviourTreeRunner.RunTree();
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
        Debug.Log($"AI of {controlBlock.name} is finished with his/her moves");
        if (OnAIIsFinished != null)
            OnAIIsFinished(this);
    }

    public void IncrementCompletedAICount(AIController ai)
    {
        if(ai != this) { return; }
        completedAICount++;
        if (completedAICount >= AIs.Count)
        {
            if (OnAIsAreFinished != null)
                OnAIsAreFinished();
        }
        else
        {
            StartNextAIPerform();
        }
    }

    public static void ResetAIsStats()
    {
        foreach (AIController ai in AIs)
        {
            ai.controlBlock.ResetCurrentMoves();
            ai.controlBlock.ResetHealth();
        }
        completedAICount = 0;
    }

    public static void ResetAIsMoves()
    {
        foreach (AIController ai in AIs)
        {
            ai.controlBlock.ResetCurrentMoves();
        }

        completedAICount = 0;

        StartNextAIPerform();
    }


    public static void StartNextAIPerform()
    {
        AIs[completedAICount].PerformAction();
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
