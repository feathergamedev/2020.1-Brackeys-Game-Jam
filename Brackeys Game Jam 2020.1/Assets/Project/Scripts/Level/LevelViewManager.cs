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

    private Tween m_changeViewTween;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [SerializeField]
    private RectTransform m_levelCanvas;

    [SerializeField]
    private float m_smoothScrollTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetLevelView(bool isSmooth = false)
    {
        StartCoroutine(ResetLevelViewSequence(isSmooth));
    }

    IEnumerator ResetLevelViewSequence(bool isSmooth)
    {
        if (isSmooth)
        {
            var scrollTime = m_smoothScrollTime;

            if (m_levelCanvas.transform.position == Vector3.zero)
            {
                scrollTime = 0f;
            }

            m_levelCanvas.transform.DOMove(Vector3.zero, scrollTime);

            if (scrollTime != 0f)
                yield return new WaitForSeconds(scrollTime + 0.2f);
        }
        else
        {
            m_levelCanvas.localPosition = Vector3.zero;
        }

        EventEmitter.Emit(GameEvent.LevelStart);

        yield return null;
    }

    public void ChangeLevelView(LevelViewSensor sensor)
    {
        if (m_changeViewTween != null)
            m_changeViewTween.Kill();

        var newPos = sensor.m_newBottomPos.position;
//        newPos = Camera.main.ScreenToWorldPoint(newPos);
        newPos.z = 0;
        newPos *= 10;

        Debug.Log(newPos);

        m_changeViewTween = m_levelCanvas.DOLocalMoveY(-newPos.y, m_scrollTime).SetEase(m_scrollEase);
    }

    public void ChangeLevelView(float nextPosY)
    {
        var targetPos = m_levelCanvas.transform.localPosition.y + nextPosY;
        m_changeViewTween = m_levelCanvas.DOLocalMoveY(targetPos, m_scrollTime).SetEase(m_scrollEase);
    }

}
