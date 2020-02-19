using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanic_FinishPoint : MonoBehaviour, IMechanic
{
    public void Triggered()
    {
        Debug.Log("Finished!");
        EventEmitter.Emit(GameEvent.LevelComplete);
    }
}
