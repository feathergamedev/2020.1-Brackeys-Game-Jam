    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanic_FinishPoint : MonoBehaviour, IMechanic
{
    public void Triggered()
    {
        EventEmitter.Emit(GameEvent.LevelComplete);
    }
}
