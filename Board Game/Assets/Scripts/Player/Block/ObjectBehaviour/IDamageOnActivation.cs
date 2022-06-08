using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageOnActivation
{
    public int attackedCharacterCount { get; set; }
    public int curAttackedCharacterCount {get; set;}

    IEnumerator DamageCharacterCoroutine(ObjectBlock objectBlock, CharacterBlock userBlock);
}
