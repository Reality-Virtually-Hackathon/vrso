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
        Six
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

    private int m_currentFrame = 0;

    private float m_currentTime;

    private int m_numChanged;

    private void Awake()
    {
        m_instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(m_currentFrame == 4)
        {
            m_currentTime -= Time.deltaTime;

            if(m_currentTime <= 0)
            {
                AdvanceFrame();
            }
        }
    }

    public void AdvanceFrame(FrameFour a_Value = FrameFour.Up)
    {
        bool changed = false;

        if (CurrentFrame == FrameNumber.Four)
        {
            if(!m_fourCheck.Contains(a_Value))
            {
                m_fourCheck.Add(a_Value);

                if(m_fourCheck.Count == 3)
                {
                    m_currentFrame++;
                    changed = true;
                }
            }
        }
        else
        {
            m_currentFrame++;
            changed = true;
        }

        if (changed)
        {
            if (m_currentFrame == 4)
            {
                if (!SteamVR_LoadLevel.loading)
                {
                    SteamVR_LoadLevel.Begin("Area2");

                    m_currentTime = 75;
                }
            }
            else if (m_currentFrame == 5)
            {
                if (!SteamVR_LoadLevel.loading)
                {
                    SteamVR_LoadLevel.Begin("Area3");

                    m_currentTime = 75;
                }
            }
        }
    }
}