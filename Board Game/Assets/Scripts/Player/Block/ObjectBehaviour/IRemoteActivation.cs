using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRemoteActivation
{
    public void OnTriggered(ObjectBlock self, ObjectBlock trigger, CharacterBlock userBlock);

}
