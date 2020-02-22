using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelViewSensor : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_nextScreen;
    public Transform m_newBottomPos;

    private void Awake()
    {
        m_nextScreen.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        m_newBottomPos.GetComponent<SpriteRenderer>().enabled = false;
    }


    public void TriggerSensor()
    {
        Debug.Log("Triggered.");
        //        LevelViewManager.instance.ChangeLevelView(this);
        LevelViewManager.instance.ChangeLevelView(-GetNextPosY());
        Destroy(this.gameObject);
    }

    float GetNextPosY()
    {
        return m_nextScreen.transform.position.y;
    }
}
