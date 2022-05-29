using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivationOnStep
{
    public void OnStepped(ObjectBlock objectBlock, CharacterBlock userBlock);
}
