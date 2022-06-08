using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRemoteActivationOnTrigger : IActivationOnTrigger
{
    public IRemoteActivation toBeTriggered { get; set; }
}
