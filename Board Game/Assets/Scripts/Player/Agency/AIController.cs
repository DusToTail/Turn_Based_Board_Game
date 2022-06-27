using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI agent that controls a character block via a behaviour tree runner
/// </summary>
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

    public IEnumerator BehaviourTreeAction { get { return _behaviourTreeRunner.RunTree(); } }
    private GameManager _gameManager;
    private PlayerController _playerController;
    private BehaviourTreeRunner _behaviourTreeRunner;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _playerController = FindObjectOfType<PlayerController>();
        _behaviourTreeRunner = GetComponent<BehaviourTreeRunner>();
        controlBlock = GetComponent<CharacterBlock>();
    }
    private void Start()
    {
        target = _playerController.playerBlock;
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

    public void PerformAction() => StartCoroutine(_behaviourTreeRunner.RunTree());

    /// <summary>
    /// Perform action via behaviour tree runner when GameManager.OnNextMoveRequired event is sent 
    /// ONLY IF the block that initiated the event is the same is this block
    /// </summary>
    /// <param name="compareBlock"></param>
    public void ContinueToMoveIfAllowed(CharacterBlock compareBlock)
    {
        if (compareBlock != controlBlock) { return; }
        PerformAction();
    }

    /// <summary>
    /// Announce that this AI agent is finished with its turn when GameManager.OnCharacterRanOutOfMoves event is sent 
    /// ONLY IF the block that initiated the event is the same is this block
    /// </summary>
    /// <param name="compareBlock"></param>
    public void CallAIIsFinished(CharacterBlock compareBlock)
    {
        if(compareBlock != controlBlock) { return; }
        Debug.Log($"AI of {controlBlock.name} is finished with his/her moves");
        if (OnAIIsFinished != null)
            OnAIIsFinished(this);
    }

    /// <summary>
    /// Increase the completed during the turn AI count by one when this.OnAIIsFinished event is sent 
    /// ONLY IF the AI agent that initiated the event is the same is this agent to ensure incrementing only ONCE
    /// If all AI are finished, inform the game manager to switch turn back to player
    /// else, start the next AI in the list to perform action
    /// </summary>
    /// <param name="ai"></param>
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
            StartNextAIPerform();
    }

    /// <summary>
    /// Reset all AI's stat and the completedAICount to 0 when GameManager.OnLevelStarted is sent
    /// </summary>
    public static void ResetAIsStats()
    {
        foreach (AIController ai in AIs)
        {
            ai.controlBlock.ResetCurrentMoves();
            ai.controlBlock.ResetHealth();
        }
        completedAICount = 0;
    }

    /// <summary>
    /// Reset all AI's available move points when GameManager.OnAITurnStarted is sent and start the first AI action
    /// </summary>
    public static void ResetAIsMoves()
    {
        foreach (AIController ai in AIs)
            ai.controlBlock.ResetCurrentMoves();

        completedAICount = 0;
        StartNextAIPerform();
    }

    public static void StartNextAIPerform()
    {
        if(AIs.Count == 0) 
        {
            if (OnAIsAreFinished != null)
                OnAIsAreFinished();
            return; 
        }
        AIs[completedAICount].PerformAction();
    }

    /// <summary>
    /// Predicate for sorting the AI performing list by its action sorting ID (which AI performs before) when enabled and disabled
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
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
