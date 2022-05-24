using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterBlock : Block
{
    [SerializeField] private int movesPerTurn;
    [SerializeField] private int maxHealth;
    [SerializeField] private int attackDamage;

    private int _curHealth;
    private int _curMovesLeft;
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }


    public virtual void TriggerMovement()
    {

    }

    public virtual void TriggerAttack()
    {

    }

    public virtual void TriggerHit()
    {

    }

    public void MoveFoward()
    {
        GetComponent<Rigidbody>().AddForce(forwardDirection);
    }

    public void AttackForward(int damageAmount)
    {
        Cell attackCell = _gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);

    }

    public void TakeDamage(int damageAmount)
    {
        MinusHealth(damageAmount);
    }

    public void HealHealth(int healAmount)
    {
        PlusHealth(healAmount);
    }

    private void MinusHealth(int amount)
    {
        if (amount < 0) { return; }
        _curHealth -= amount;
        if (_curHealth < 0) { _curHealth = 0; }
    }

    private void PlusHealth(int amount)
    {
        if(amount < 0) { return; }
        _curHealth += amount;
        if(_curHealth > maxHealth) { _curHealth = maxHealth; }
    }

    private bool HealthIsZero() { return _curHealth == 0; }

}
