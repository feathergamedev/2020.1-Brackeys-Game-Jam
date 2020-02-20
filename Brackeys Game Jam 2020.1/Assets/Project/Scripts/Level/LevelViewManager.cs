using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelViewManager : MonoBehaviour
{
    public static LevelViewManager instance;

    [SerializeField]
    private float m_scrollTime;

    [SerializeField]
    private Ease m_scrollEase;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [SerializeField]
    private RectTransform m_levelCanvas;

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        UnregisterEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RegisterEvent()
    {
        EventEmitter.Add(GameEvent.LevelComplete, OnLevelComplete);
        EventEmitter.Add(GameEvent.LevelFail, OnLevelFail);
    }

    void UnregisterEvent()
    {
        EventEmitter.Add(GameEvent.LevelComplete, OnLevelComplete);
        EventEmitter.Remove(GameEvent.LevelFail, OnLevelFail);
    }

    void OnLevelComplete(IEvent @event)
    {
        ResetLevelView();
    }

    void OnLevelFail(IEvent @event)
    {
        ResetLevelView();
    }

    public void ResetLevelView()
    {
        m_levelCanvas.localPosition = Vector3.zero;
    }

    public void ChangeLevelView(LevelViewSensor sensor)
    {
        var newPos = sensor.m_newBottomPos.position;
        newPos = Camera.main.ScreenToWorldPoint(newPos);
        newPos.z = 0;

        Debug.Log(newPos);

        m_levelCanvas.DOLocalMoveY(newPos.y, m_scrollTime).SetEase(m_scrollEase);
    }
}
