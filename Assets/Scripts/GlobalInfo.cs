using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInfo : MonoBehaviour
{
    private static GlobalInfo m_instance;

    public static GlobalInfo Instance { get { return m_instance; } }

    public enum FrameNumber
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven
    }
    public FrameNumber CurrentFrame { get { return (FrameNumber)m_currentFrame; } }

    public enum FrameFour
    {
        Up,
        Down,
        Stop
    }

    private List<FrameFour> m_fourCheck = new List<FrameFour>();

    [HideInInspector]
    public bool Started;

    [HideInInspector]
    public bool CheckStart;

    private bool prevStarted;

    private int m_currentFrame = 0;

    private float m_currentTime;
    private bool m_startTimer;

    private int m_numChanged;

    private bool fading = false;

    private void Awake()
    {
        if (m_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            m_instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (m_startTimer)
        {
            m_currentTime -= Time.deltaTime;

            if (m_currentTime <= 0)
            {
                AdvanceFrame(FrameFour.Up, true);
            }
        }

        if (prevStarted != Started && Started && CurrentFrame == FrameNumber.Four && !m_startTimer)
        {
            m_currentTime = 32f;
            m_startTimer = true;
        }

        if (prevStarted != Started && Started && CurrentFrame == FrameNumber.Five && !m_startTimer)
        {
            m_currentTime = 30f;
            m_startTimer = true;
        }

        if (prevStarted != Started && Started && CurrentFrame == FrameNumber.Six && !m_startTimer)
        {
            m_currentTime = 25f;
            m_startTimer = true;
        }

        prevStarted = Started;
    }

    public void AdvanceFrame(FrameFour a_Value = FrameFour.Up, bool a_force = false)
    {
        bool changed = false;

        if (CurrentFrame == FrameNumber.Four)
        {
            if (!m_fourCheck.Contains(a_Value))
            {
                m_fourCheck.Add(a_Value);
            }

            if (a_force && m_fourCheck.Count == 3)
            {
                m_currentFrame++;
                changed = true;
            }
        }
        else
        {
            m_currentFrame++;
            changed = true;
        }

        if (changed)
        {
            if (CurrentFrame == FrameNumber.Four)
            {
                if (!SteamVR_LoadLevel.loading)
                {
                    CheckStart = false;
                    Started = false;
                    m_startTimer = false;

                    SteamVR_LoadLevel.Begin("Area2");
                }
            }
            else if (CurrentFrame == FrameNumber.Five)
            {
                if (!SteamVR_LoadLevel.loading)
                {
                    CheckStart = false;
                    Started = false;
                    m_startTimer = false;

                    SteamVR_LoadLevel.Begin("Area3");
                }
            }
            else if (CurrentFrame == FrameNumber.Six)
            {
                if (!SteamVR_LoadLevel.loading)
                {
                    CheckStart = false;
                    Started = false;
                    m_startTimer = false;

                    SteamVR_LoadLevel.Begin("Area4");
                }
            }
            else if (CurrentFrame == FrameNumber.Seven)
            {
                if (!SteamVR_LoadLevel.loading)
                {
                    CheckStart = false;
                    Started = false;
                    m_startTimer = false;

                    SteamVR_LoadLevel.Begin("Area5");
                }
            }
        }
    }
}