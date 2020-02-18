using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [SerializeField]
    private MapGenerator m_mapGenerator;

    private Dictionary<Vector2Int, Block> m_blocks;

    [SerializeField]
    private List<Actor> m_actors;

    [SerializeField]
    private Transform m_blockRoot;

    [SerializeField]
    private Transform m_actorRoot;

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
        // Temp, return true as long as block exist.
        return (m_blocks.ContainsKey(coordinate));
    }

    void BlockSetup()
    {
        m_blocks = new Dictionary<Vector2Int, Block>();

        foreach(Transform t in m_blockRoot)
        {
            var newBlock = t.GetComponent<Block>();
            m_blocks.Add(newBlock.Coordinate, newBlock);
        }

        m_mapGenerator.GenerateMap(ref m_blocks, m_blockRoot);
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
