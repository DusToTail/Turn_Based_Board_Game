using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public int attackDamage;
    public int attackRange;
    public int attackCooldown;

    protected int _curAttackCooldown;
    protected bool _canAttack;


    public abstract void Attack(CharacterBlock[] blocks);
    public void ReduceAttackCooldown(int amount) 
    { 
        if(amount < 0) { return; }
        _curAttackCooldown -= amount;
        if(_curAttackCooldown < 0) { _curAttackCooldown = 0; }
    }
    public void ResetAttackCooldown() { _curAttackCooldown = attackCooldown; }
    public void SetCanAttack(bool canAttack) { _canAttack = canAttack; }
}
