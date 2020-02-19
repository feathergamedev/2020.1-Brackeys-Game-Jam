using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [System.Serializable]
    struct BlockInfo
    {
        public BlockType Type;
        public GameObject Prefab;
    }

    [SerializeField]
    private Vector2 m_blockOriginPos;

    [SerializeField]
    private float m_blockDistance;

    [SerializeField]
    private List<BlockInfo> m_allBlocks;

    [SerializeField]
    private GameObject m_blockPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GenerateMap(Transform blockRoot)
    {
        for(int i=0; i<blockRoot.childCount; i++)
        {
            var block = blockRoot.GetChild(i).GetComponent<Block>();
            var coordinate = block.Coordinate;
            var newPos = m_blockOriginPos + new Vector2(coordinate.x * m_blockDistance, coordinate.y * m_blockDistance);
            block.transform.localPosition = newPos;
        }
    }

    public Block CreateBlock(BlockType blockType, Vector2Int coordinate)
    {
        for (int i=0; i<m_allBlocks.Count; i++)
        {
            if (m_allBlocks[i].Type == blockType)
            {
                var blockObject = Instantiate(m_allBlocks[i].Prefab);
                var newBlock = blockObject.GetComponent<Block>();
                newBlock.Coordinate = coordinate;
                newBlock.CurBlockType = blockType;

                SetBlock(newBlock);
                return newBlock;
            }
        }

        Debug.LogError("Block not found.");
        return null;
    }

    public void SetBlock(Block block)
    {
        block.transform.SetParent(MapManager.instance.m_blockRoot);
        block.transform.localScale = Vector3.one;

        var coordinate = block.Coordinate;
        var newPos = m_blockOriginPos + new Vector2(coordinate.x * m_blockDistance, coordinate.y * m_blockDistance);
        block.transform.localPosition = newPos;
    }
}
