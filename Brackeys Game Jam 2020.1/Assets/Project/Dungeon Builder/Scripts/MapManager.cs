using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Empty = -1,
    NormalFloor_0 = 0,
    NormalFloor_1 = 1,
    NormalFloor_2 = 2,
    Hole = 3,
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [SerializeField]
    private MapGenerator m_mapGenerator;



    private Dictionary<Vector2Int, Block> m_blocks;

    [SerializeField]
    private List<Actor> m_actors;

    public Transform m_blockRoot;

    public Transform m_actorRoot;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        BlockSetup();
        ActorSetup();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_blocks[new Vector2Int(2, 0)].GetExploded();
        }
    }

    public void MoveTo(Actor actor, Vector2Int target)
    {
        if (IsSteppable(target) == false)
        {
            return;
        }

        actor.transform.position = m_blocks[target].transform.position;
        actor.Coordinate = target;
    }

    public bool IsSteppable(Vector2Int coordinate)
    {
        if (m_blocks.ContainsKey(coordinate) == false)
            return false;

        var block = m_blocks[coordinate];

        if (block.CurBlockType == BlockType.Hole)
            return false;

        block.GetTrampled(1);

        return true;
    }

    public void ReplaceBlock(Vector2Int coordinate, BlockType type)
    {
        Destroy(m_blocks[coordinate]);
        m_blocks.Remove(coordinate);
        var newBlock = m_mapGenerator.CreateBlock(type, coordinate);

        m_blocks.Add(coordinate, newBlock);

        if (type == BlockType.Hole)
        {
            ExplodeAffectNeighbors(coordinate);
        }
    }

    void ExplodeAffectNeighbors(Vector2Int coordinate)
    {
        var checkList = new List<Block>();

        if (m_blocks.ContainsKey(coordinate + Direction.Left.ToVec2()))
        {
            checkList.Add(m_blocks[coordinate + Direction.Left.ToVec2()]);
        }

        if (m_blocks.ContainsKey(coordinate + Direction.Right.ToVec2()))
        {
            checkList.Add(m_blocks[coordinate + Direction.Right.ToVec2()]);
        }

        if (m_blocks.ContainsKey(coordinate + Direction.Up.ToVec2()))
        {
            checkList.Add(m_blocks[coordinate + Direction.Up.ToVec2()]);
        }

        if (m_blocks.ContainsKey(coordinate + Direction.Down.ToVec2()))
        {
            checkList.Add(m_blocks[coordinate + Direction.Down.ToVec2()]);
        }

        foreach(Block b in checkList)
        {
            b.AffectedByNeighbor();
        }
    }

    void BlockSetup()
    {
        m_blocks = new Dictionary<Vector2Int, Block>();

        foreach(Transform t in m_blockRoot)
        {
            var newBlock = t.GetComponent<Block>();
            m_blocks.Add(newBlock.Coordinate, newBlock);
        }

        m_mapGenerator.GenerateMap(m_blockRoot);
    }

    void ActorSetup()
    {
        m_actors = new List<Actor>();

        foreach(Transform t in m_actorRoot)
        {
            var newActor = t.GetComponent<Actor>();
            m_actors.Add(newActor);
        }

        foreach(Actor a in m_actors)
        {
            a.transform.localPosition = m_blocks[a.Coordinate].transform.localPosition;
        }
    }
}
