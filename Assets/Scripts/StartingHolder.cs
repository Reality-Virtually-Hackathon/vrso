using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingHolder : MonoBehaviour
{
    [SerializeField]
    private Capture m_leftCap;

    [SerializeField]
    private GameObject m_leftObj;

    [SerializeField]
    private Capture m_rightCap;

    [SerializeField]
    private GameObject m_rightObj;

    [SerializeField]
    private Transform m_leftPointer;

    [SerializeField]
    private Transform m_rightPointer;

    [SerializeField]
    private GestureController m_leftGesture;

    [SerializeField]
    private GestureController m_rightGesture;

    [SerializeField]
    private StartingGesture m_leftStart;

    [SerializeField]
    private StartingGesture m_rightStart;

    private int m_movingCount;

    [SerializeField]
    private Text m_debugText;

    [SerializeField]
    private Text m_leftHandText;
    [SerializeField]
    private Text m_rightHandText;
    [SerializeField]
    private Text m_frameText;

    [SerializeField]
    private AudioSource[] m_instruments;

    private Vector3 m_volumeStartPos;
    private Vector3 m_volumeLastPos;
    private bool m_started;
    private float m_startTime;

    private bool m_movingDown;
    private bool m_movingUp;

    private bool m_goneUp;
    private bool m_goneDown;

    private void Update()
    {
        string leftText = "";

        leftText += "IsOpen: " + m_leftGesture.isOpen;
        leftText += "\nIsClosed: " + m_leftGesture.isClosed;
        leftText += "\nIsPointing: " + m_leftGesture.IsPointing;
        leftText += "\nHasObject: " + m_leftGesture.HasObject;
        leftText += "\nPalmUp: " + m_leftGesture.PalmUp;
        leftText += "\nPalmDown: " + m_leftGesture.PalmDown;

        m_leftHandText.text = leftText;

        string rightText = "";

        rightText += "IsOpen: " + m_rightGesture.isOpen;
        rightText += "\nIsClosed: " + m_rightGesture.isClosed;
        rightText += "\nIsPointing: " + m_rightGesture.IsPointing;
        rightText += "\nHasObject: " + m_rightGesture.HasObject;
        rightText += "\nPalmUp: " + m_rightGesture.PalmUp;
        rightText += "\nPalmDown: " + m_rightGesture.PalmDown;

        m_rightHandText.text = rightText;

        m_frameText.text = GlobalInfo.Instance.CurrentFrame.ToString();

        if(!m_started && GlobalInfo.Instance.Started)
        {
            m_started = true;
            m_startTime = Time.time;
        }

        string dText = "CheckStart: " + GlobalInfo.Instance.CheckStart + "\nStarted: " + GlobalInfo.Instance.Started;

        if (!GlobalInfo.Instance.CheckStart)
        {
            if (!m_leftGesture.HasObject && !m_rightGesture.HasObject && m_leftGesture.IsPointing && m_rightGesture.IsPointing)
            {
                float yDiff = m_leftPointer.transform.position.y - m_rightPointer.transform.position.y;

                float sqrMag = (m_rightPointer.transform.position - m_leftPointer.transform.position).sqrMagnitude;

                dText += "\nyDiff: " + yDiff + " \nqrMag: " + sqrMag;

                if (Mathf.Abs(yDiff) < .02f && sqrMag < .004f)
                {
                    GlobalInfo.Instance.CheckStart = true;
                    GlobalInfo.Instance.Started = false;
                    m_started = false;
                    m_leftCap.gameObject.SetActive(true);
                    m_rightCap.gameObject.SetActive(true);
                    m_leftStart.gameObject.SetActive(true);
                    m_rightStart.gameObject.SetActive(true);
                }
            }
        }
        else if(m_started)
        {
            if(Time.time - m_startTime > 10)
            {
                GlobalInfo.Instance.CheckStart = false;
            }
        }

        m_debugText.text = dText;

        if (GlobalInfo.Instance.Started)
        {
            if ((int)GlobalInfo.Instance.CurrentFrame >= (int)GlobalInfo.FrameNumber.Two)
            {
                if (!m_leftGesture.HasObject && !m_rightGesture.HasObject && m_leftGesture.isClosed && m_rightGesture.isClosed)
                {
                    foreach (AudioSource i in m_instruments)
                    {
                        i.volume = 0;
                    }

                    if(GlobalInfo.Instance.CurrentFrame == GlobalInfo.FrameNumber.Two)
                    {
                        GlobalInfo.Instance.AdvanceFrame();
                    }
                }
            }

            if ((int)GlobalInfo.Instance.CurrentFrame >= (int)GlobalInfo.FrameNumber.Three)
            {
                if (!m_leftGesture.HasObject && !m_rightGesture.HasObject && m_leftGesture.isOpen && m_rightGesture.isOpen)
                {
                    if (m_volumeStartPos == Vector3.zero)
                    {
                        m_volumeStartPos = (m_leftGesture.transform.position + m_rightGesture.transform.position) / 2f;
                    }

                    Vector3 nextPos = (m_leftGesture.transform.position + m_rightGesture.transform.position) / 2f;

                    if (m_leftGesture.PalmUp && m_rightGesture.PalmUp)
                    {
                        if (nextPos.y < m_volumeStartPos.y)
                        {
                            m_volumeStartPos = nextPos;
                        }
                        else if (nextPos.y < m_volumeLastPos.y)
                        {
                            m_movingDown = true;
                        }
                        else
                        {
                            if (m_movingDown)
                            {
                                m_volumeStartPos = nextPos;
                                m_movingDown = false;
                            }

                            float change = (nextPos.y - m_volumeStartPos.y) / 50f;

                            foreach (AudioSource i in m_instruments)
                            {
                                float nextVolume = Mathf.Clamp01(i.volume + change);

                                i.volume = nextVolume;
                            }

                            if(!m_goneUp)
                            {
                                m_goneUp = true;
                            }
                        }
                    }
                    else if (m_leftGesture.PalmDown && m_rightGesture.PalmDown)
                    {
                        if (nextPos.y > m_volumeStartPos.y)
                        {
                            m_volumeStartPos = nextPos;
                        }
                        else if (nextPos.y > m_volumeLastPos.y)
                        {
                            m_movingUp = true;
                        }
                        else
                        {
                            if (m_movingUp)
                            {
                                m_volumeStartPos = nextPos;
                                m_movingUp = false;
                            }

                            float change = (nextPos.y - m_volumeStartPos.y) / 50f;

                            foreach (AudioSource i in m_instruments)
                            {
                                float nextVolume = Mathf.Clamp01(i.volume + change);

                                i.volume = nextVolume;
                            }

                            if(!m_goneDown)
                            {
                                m_goneDown = true;
                            }
                        }
                    }

                    m_volumeLastPos = nextPos;
                }
                else if (m_volumeStartPos != Vector3.zero)
                {
                    m_volumeStartPos = Vector3.zero;
                }

                if(GlobalInfo.Instance.CurrentFrame == GlobalInfo.FrameNumber.Three)
                {
                    if(m_goneUp && m_goneDown)
                    {
                        GlobalInfo.Instance.AdvanceFrame();
                    }
                }
            }
        }
    }
}