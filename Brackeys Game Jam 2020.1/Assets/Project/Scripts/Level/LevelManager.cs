using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField]
    private List<GameObject> m_allLevels;

    private GameObject m_curLevelObject;

    private void Awake()
    {
        if (instance == null)
            instance = this;
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

    void LoadLevel(int levelID)
    {
        if (m_curLevelObject)
            Destroy(m_curLevelObject);

        m_curLevelObject = Instantiate(m_allLevels[levelID], Vector3.zero, Quaternion.identity, transform);
    }
}
