using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    [SerializeField]
    private BlockType m_blockType;

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
        if (Input.GetKeyDown(KeyCode.D))
            TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        var isNotDestructable = false;

        if (isNotDestructable)
            return;

        if (m_blockType == BlockType.Hole)
            return;

        MapManager.instance.ReplaceBlock(m_coordinate, (BlockType)((int)m_blockType + 1));
    }
}
