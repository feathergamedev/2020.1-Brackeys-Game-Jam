using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField]
    private Vector2 m_blockOriginPos;

    [SerializeField]
    private float m_blockDistance;

    [SerializeField]
    private GameObject m_blockPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GenerateMap(ref Dictionary<Vector2Int, Block> map, Transform blockRoot)
    {
        for(int i=0; i<blockRoot.childCount; i++)
        {
            var block = blockRoot.GetChild(i).GetComponent<Block>();
            var coordinate = block.Coordinate;
            var newPos = m_blockOriginPos + new Vector2(coordinate.x * m_blockDistance, coordinate.y * m_blockDistance);
            block.transform.localPosition = newPos;
        }
    }
}
