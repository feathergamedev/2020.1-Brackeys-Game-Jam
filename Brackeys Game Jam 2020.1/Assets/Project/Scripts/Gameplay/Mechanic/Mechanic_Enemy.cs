using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mechanic_Enemy : MonoBehaviour, IMechanic
{

    [SerializeField]
    private Collider2D m_detectArea;

    [SerializeField]
    private float m_rotateTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PatrolAI());
    }

    // Update is called once per frame
    void Update()
    {
        InvaderCheck();
    }

    public void Triggered()
    {
        EventEmitter.Emit(GameEvent.LevelFail);
    }

    void InvaderCheck()
    {
        var player = GameObject.FindWithTag("Player");
        var playerController = player.GetComponent<PlayerController>();

        if (playerController.CurPlayerState == PlayerState.Dig)
            return;

        if (m_detectArea.OverlapPoint(player.transform.position))
        {
            EventEmitter.Emit(GameEvent.LevelFail);
        }
    }

    IEnumerator PatrolAI()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_rotateTime * 2);

            var curRotateZ = m_detectArea.gameObject.transform.rotation.eulerAngles.z;
            var nextRotation = Quaternion.Euler(0, 0, curRotateZ - 90f);
            m_detectArea.gameObject.transform.DORotateQuaternion(nextRotation, m_rotateTime);
        }
    }
}
