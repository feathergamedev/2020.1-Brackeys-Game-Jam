using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField]
    private List<GameObject> m_allLevels;

    private GameObject m_curLevelObject;

    private int m_curLevelID;

    [SerializeField]
    private Transform m_initPlayerPos;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        UnRegisterEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        EnterLevel(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RegisterEvent()
    {
        EventEmitter.Add(GameEvent.LevelComplete, OnLevelComplete);
    }

    void UnRegisterEvent()
    {
        EventEmitter.Remove(GameEvent.LevelComplete, OnLevelComplete);
    }

    void OnLevelComplete(IEvent @event)
    {
        LevelViewManager.instance.ResetLevelView();

        if (m_curLevelID+1 >= m_allLevels.Count)
        {
            Debug.LogError("No more level.");
            EnterLevel(0);
        }
        else
        {
            EnterLevel(m_curLevelID + 1);
        }
    }

    public void LevelRestart()
    {
        CreateLevel(m_curLevelID);
        LevelViewManager.instance.ResetLevelView(true);
    }

    public void EnterLevel(int levelID)
    {
        CreateLevel(levelID);
        LevelViewManager.instance.ResetLevelView(false);
    }

    void CreateLevel(int levelID)
    {
        if (m_curLevelObject)
            Destroy(m_curLevelObject);

        m_curLevelObject = Instantiate(m_allLevels[levelID], Vector3.zero, Quaternion.identity, transform);
        m_curLevelObject.transform.localPosition = Vector3.zero;

        m_curLevelID = levelID;
    }

    public Vector3 GetInitPlayerPos()
    {
        return m_initPlayerPos.localPosition;
    }

}
