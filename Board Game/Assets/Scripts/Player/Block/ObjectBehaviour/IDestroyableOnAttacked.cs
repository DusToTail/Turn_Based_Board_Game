using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestroyableOnAttacked
{
    public void OnAttacked(ObjectBlock self, CharacterBlock attackingBlock);

}
