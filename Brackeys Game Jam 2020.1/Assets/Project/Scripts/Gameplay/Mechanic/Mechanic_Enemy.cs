﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mechanic_Enemy : MonoBehaviour, IMechanic
{

    [SerializeField]
    private Collider2D m_detectArea;

    [SerializeField]
    private float m_rotateTime;

    private void OnEnable()
    {
        RegisterEvent();
    }

    private void OnDisable()
    {
        UnregisterEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        InvaderCheck();
    }

    void RegisterEvent()
    {
        EventEmitter.Add(GameEvent.LevelStart, OnLevelStart);
    }

    void UnregisterEvent()
    {
        EventEmitter.Remove(GameEvent.LevelStart, OnLevelStart);
    }

    public void Triggered()
    {
        EventEmitter.Emit(GameEvent.LevelFail);
    }

    void InvaderCheck()
    {
        var player = GameObject.FindWithTag("Player");
        var playerController = player.GetComponent<PlayerController>();

        if (playerController.CurPlayerState != PlayerState.Walk)
            return;

        if (m_detectArea.OverlapPoint(player.transform.position))
        {
            EventEmitter.Emit(GameEvent.LevelFail);
        }
    }

    void OnLevelStart(IEvent @event)
    {
        StartCoroutine(PatrolAI());
    }

    IEnumerator PatrolAI()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_rotateTime * 1.5f);

            var curRotateZ = transform.rotation.eulerAngles.z;
            var nextRotation = Quaternion.Euler(0, 0, curRotateZ - 90f);

            transform.DORotateQuaternion(nextRotation, m_rotateTime);

        }
    }
}
