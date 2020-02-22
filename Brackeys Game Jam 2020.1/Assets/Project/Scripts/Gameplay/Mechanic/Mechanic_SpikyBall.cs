using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanic_SpikyBall : MonoBehaviour
{

    [SerializeField]
    private float m_moveSpeed;

    [SerializeField]
    private Vector3 m_curDirection;

    [SerializeField]
    private Transform m_boundaryMin, m_boundaryMax;

    [SerializeField]
    private Collider2D m_ballCollider;

    private float m_turnAroundTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_curDirection = (m_boundaryMax.position - m_boundaryMin.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        m_ballCollider.transform.position += m_curDirection * m_moveSpeed * Time.deltaTime;

        if (m_ballCollider.OverlapPoint(m_boundaryMin.position) || m_ballCollider.OverlapPoint(m_boundaryMax.position))
        {
            if (m_turnAroundTimer <= 0f)
            {
                m_curDirection *= -1;
                m_turnAroundTimer = 0.3f;
            }
        }

        if (m_turnAroundTimer > 0f)
            m_turnAroundTimer -= Time.deltaTime;
        else
            m_turnAroundTimer = 0f;

    }
}
