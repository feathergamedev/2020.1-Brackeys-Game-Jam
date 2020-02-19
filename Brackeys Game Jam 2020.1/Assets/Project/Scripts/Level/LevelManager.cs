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
        LoadLevel(0);
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

    void UnRegisterEvent()
    {
        EventEmitter.Remove(GameEvent.LevelComplete, OnLevelComplete);
        EventEmitter.Remove(GameEvent.LevelFail, OnLevelFail);
    }

    void OnLevelComplete(IEvent @event)
    {
        if (m_curLevelID+1 >= m_allLevels.Count)
        {
            Debug.LogError("No more level.");
            LoadLevel(0);
        }
        else
        {
            LoadLevel(m_curLevelID + 1);
        }
    }

    void OnLevelFail(IEvent @event)
    {
        LoadLevel(m_curLevelID);
    }

    void LoadLevel(int levelID)
    {
        if (m_curLevelObject)
            Destroy(m_curLevelObject);

        m_curLevelObject = Instantiate(m_allLevels[levelID], Vector3.zero, Quaternion.identity, transform);
        m_curLevelID = levelID;

        EventEmitter.Emit(GameEvent.LevelStart);
    }

    public Vector3 GetInitPlayerPos()
    {
        return m_initPlayerPos.position;
    }

}
