﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum SoundEffectType
{
    PlayerWalk,
    DigHole,
    BackToGround,
    PlayerDie,
    PlayerRevive,    
    LevelComplete,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private Transform m_soundEffectRoot;

    private List<AudioSource> m_allSoundEffect;

    [SerializeField]
    private AudioSource m_curBGM;

    bool m_isMute;

    private void Awake()
    {
        instance = this;
        FadeInBGM();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_allSoundEffect = new List<AudioSource>();

        for(int i=0; i<m_soundEffectRoot.childCount; i++)
        {
            m_allSoundEffect.Add(m_soundEffectRoot.GetChild(i).GetComponent<AudioSource>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            m_isMute = !m_isMute;
            m_curBGM.mute = m_isMute;

            foreach(AudioSource SFX in m_allSoundEffect)
            {
                SFX.mute = m_isMute;
            }
        }
    }

    public void PlaySoundEffect(SoundEffectType type)
    {
        if (IsPlaying(type))
            return;

        m_allSoundEffect[(int)type].Play();
    }

    public void StopSoundEffect(SoundEffectType type)
    {
        if (!IsPlaying(type))
            return;

        m_allSoundEffect[(int)type].Stop();
    }

    public bool IsPlaying(SoundEffectType type)
    {
        return m_allSoundEffect[(int)type].isPlaying;
    }

    public void FadeInBGM()
    {
        DOTween.To(() => m_curBGM.volume, x => m_curBGM.volume = x, 0.5f, 1.0f);
    }

    public void FadeOutBGM()
    {
        DOTween.To(() => m_curBGM.volume, x => m_curBGM.volume = x, 0f, 3.2f);
    }
}
