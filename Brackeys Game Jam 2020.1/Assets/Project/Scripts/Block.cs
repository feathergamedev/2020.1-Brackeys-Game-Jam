using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Hole,
    NormalFloor_0,
    NormalFloor_1,
    NormalFloor_2,
}

public class Block : MonoBehaviour
{

    [SerializeField]
    private Vector2Int m_coordinate;


    public Vector2Int Coordinate
    {
        get
        {
            return m_coordinate;
        }
        private set
        {
            m_coordinate = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
