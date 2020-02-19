using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanic_Spike : MonoBehaviour, IMechanic
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Triggered()
    {
        EventEmitter.Emit(GameEvent.LevelFail);
    }
}
