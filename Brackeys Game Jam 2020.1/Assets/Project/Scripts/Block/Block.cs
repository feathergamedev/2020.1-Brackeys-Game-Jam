using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public BlockType CurBlockType;

    [SerializeField]
    private Vector2Int m_coordinate;


    public Vector2Int Coordinate
    {
        get
        {
            return m_coordinate;
        }
        set
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

    public void GetTrampled(int damage)
    {
        var isNotDestructable = false;

        if (isNotDestructable)
            return;

        if ((int)CurBlockType + damage >= (int)BlockType.Hole)
            return;

        var nextType = (BlockType)((int)CurBlockType + damage);

        MapManager.instance.ReplaceBlock(m_coordinate, nextType);
    }

    public void GetExploded()
    {
        if (CurBlockType == BlockType.Hole)
            return;

        MapManager.instance.ReplaceBlock(m_coordinate, BlockType.Hole);
    }

    // Called when neighbor explode.
    public void AffectedByNeighbor()
    {
        switch (CurBlockType)
        {
            case BlockType.NormalFloor_0:
            case BlockType.NormalFloor_1:
                GetTrampled(1);
                break;
            case BlockType.NormalFloor_2:
                GetExploded();
                break;
            default:
                break;
        }
    }
}
