using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour, IActivationOnStep, IDamageOnActivation
{
    public GameManager gameManager;
    public WeaponHandler weaponHandler;

    public int attackedCharacterCount { get; set; }
    public int curAttackedCharacterCount { get; set; }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnStepped(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        StartCoroutine(DamageCharacterCoroutine(objectBlock, userBlock));
    }

    public IEnumerator DamageCharacterCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock)
    {
        // Trigger animation

        weaponHandler.UseWeapon(objectBlock);
        // Trigger sound effect

        yield return new WaitUntil(() => curAttackedCharacterCount >= attackedCharacterCount);
        gameManager.CallBlockEndedBehaviour(objectBlock);
        objectBlock.isFinished = true;
    }

}
