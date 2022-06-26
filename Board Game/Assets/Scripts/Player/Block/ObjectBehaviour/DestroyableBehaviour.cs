using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableBehaviour : MonoBehaviour, IDestroyableOnAttacked
{
    public GameManager gameManager;
    public int maxHealth;
    private int curHealth;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        curHealth = maxHealth;
    }

    public void OnAttacked(ObjectBlock self, CharacterBlock attackingBlock)
    {
        self.isFinished = false;
        curHealth--;
        if (curHealth <= 0) 
        { 
            curHealth = 0;
            StartCoroutine(OnDestroyedCoroutine(self, attackingBlock));
        }
        else
            StartCoroutine(OnAttackedCoroutine(self, attackingBlock));
    }

    private IEnumerator OnAttackedCoroutine(ObjectBlock self, CharacterBlock attackingBlock)
    {
        // Trigger animation

        // Trigger sound effect

        yield return null;

        attackingBlock.curAttackedEntityCount++;
        gameManager.CallBlockEndedBehaviour(self);
        self.isFinished = true;
    }

    private IEnumerator OnDestroyedCoroutine(ObjectBlock self, CharacterBlock attackingBlock)
    {
        // Trigger animation

        // Trigger sound effect

        yield return null;
        attackingBlock.curAttackedEntityCount++;
        gameManager.CallBlockEndedBehaviour(self);
        self.isFinished = true;
        Destroy(self.gameObject);
    }
}
