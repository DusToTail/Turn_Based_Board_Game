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
        // Get one forward cell
        Cell moveCell = _gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);

        // Anticipate obstacles, enemies, end goal

        // Animation and Sound effect


    }

    public void AttackForward(int damageAmount)
    {
        // Get an array of cells basing on the weapons
        Cell attackCell = _gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);
        // Use those cells from the gridController to query the characterPlane to check for those that is occupied

        // Animation and Sound effect

        // Trigger the occupants' TriggerHit method.
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

    private void MinusMoves(int movesNum)
    {
        if (movesNum < 0) { return; }
        _curMovesLeft -= movesNum;
        if (_curHealth < 0) { _curHealth = 0; }
    }

    private void PlusMoves(int movesNum)
    {
        if (movesNum < 0) { return; }
        _curMovesLeft += movesNum;
    }

    private void ResetMovesPerTurn()
    {
        _curMovesLeft = movesPerTurn;
    }

    private bool NoMoreMoves() { return _curMovesLeft == 0; }
}
