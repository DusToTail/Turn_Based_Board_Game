using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivationOnCollision
{
    public void OnCollision(ObjectBlock objectBlock, CharacterBlock userBlock);
}
