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
        LevelViewManager.instance.ChangeLevelView(this);
        Destroy(this.gameObject);
    }
}
