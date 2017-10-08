using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureController : MonoBehaviour
{
    public enum Location
    {
        Up,
        Down
    }

    private Dictionary<Finger.Digit, bool> m_upFingers = new Dictionary<Finger.Digit, bool>();
    private Dictionary<Finger.Digit, bool> m_downFingers = new Dictionary<Finger.Digit, bool>();

    public bool isOpen;
    public bool opened;
    private float openTime;
    public bool isClosed;
    public bool closed;
    private float closedTime;

    [SerializeField]
    private LineRenderer m_volumeLineRenderer;
    [SerializeField]
    private LineRenderer m_stopLineRenderer;

    [SerializeField]
    private Transform m_rayStart;

    private GameObject m_objectHit;
    public bool HasObject { get { return m_objectHit != null; } }
    public bool IsPointing
    {
        get
        {
            if (m_upFingers.ContainsKey(Finger.Digit.Pointer))
            {
                bool pointing = false;

                if(m_upFingers[Finger.Digit.Pointer])
                {
                    pointing = true;
                }

                if(pointing)
                {
                    if(m_upFingers[Finger.Digit.Middle] || m_upFingers[Finger.Digit.Ring] || m_upFingers[Finger.Digit.Pinky])
                    {
                        pointing = false;
                    }
                }

                return pointing;
            }
            else
            {
                return false;
            }
        }
    }

    private AudioSource m_objectSource;
    private Vector3 m_hitStartPos;
    private float m_lastSelectTime;

    [SerializeField]
    private bool isLeft;

    private bool m_palmUp;
    public bool PalmUp { get { return m_palmUp; } }
    private bool m_palmDown;
    public bool PalmDown { get { return m_palmDown; } }

    private int m_numChanged = 0;

    private void Awake()
    {
        for (int i = 0; i < 4; i++)
        {
            m_upFingers.Add((Finger.Digit)i, false);
            m_downFingers.Add((Finger.Digit)i, false);
        }
    }

    private void OnDisable()
    {
        if(m_objectHit != null)
        {
            iTween objTween = m_objectHit.GetComponent<iTween>();
            if (objTween != null)
            {
                Destroy(objTween);
            }
            iTween.ScaleTo(m_objectHit, iTween.Hash("scale", Vector3.one,
                "time", (1 - m_objectHit.transform.localScale.x) / .4f, "easetype", iTween.EaseType.easeOutSine));

            m_objectHit = null;
        }
    }

    public void AddFinger(Location a_location, GameObject a_finger)
    {
        switch (a_location)
        {
            case Location.Up:
                m_upFingers[a_finger.GetComponent<Finger>().MyDigit] = true;
                break;
            case Location.Down:
                m_downFingers[a_finger.GetComponent<Finger>().MyDigit] = true;
                break;
        }
    }

    public void RemoveFinger(Location a_location, GameObject a_finger)
    {
        switch (a_location)
        {
            case Location.Up:
                m_upFingers[a_finger.GetComponent<Finger>().MyDigit] = false;
                break;
            case Location.Down:
                m_downFingers[a_finger.GetComponent<Finger>().MyDigit] = false;
                break;
        }
    }

    private void Update()
    {
        if (!GlobalInfo.Instance.Started)
        {
            if (m_volumeLineRenderer.enabled)
            {
                m_volumeLineRenderer.enabled = false;
            }

            return;
        }

        float dot = Vector3.Dot(transform.up, Vector3.up);

        if (dot < -.7f)
        {
            m_palmDown = true;
        }
        else
        {
            m_palmDown = false;
        }

        if (dot > .7f)
        {
            m_palmUp = true;
        }
        else
        {
            m_palmUp = false;
        }

        if (opened)
        {
            opened = false;
        }
        if (closed)
        {
            closed = false;
        }

        int openCount = 0;

        foreach (KeyValuePair<Finger.Digit, bool> kvp in m_upFingers)
        {
            if (kvp.Value)
            {
                openCount++;
            }
        }

        int closedCount = 0;

        foreach (KeyValuePair<Finger.Digit, bool> kvp in m_downFingers)
        {
            if (kvp.Value)
            {
                closedCount++;
            }
        }

        if (openCount >= 3)
        {
            if (!isOpen)
            {
                opened = true;
            }

            isOpen = true;
        }

        if (closedCount >= 4)
        {
            if (!isClosed)
            {
                closed = true;
            }

            isClosed = true;
        }

        if (openCount < 3)
        {
            isOpen = false;
        }

        if (closedCount < 4)
        {
            isClosed = false;
        }

        if ((int)GlobalInfo.Instance.CurrentFrame >= 3)
        {
            if (Time.time - m_lastSelectTime > 1)
            {
                if (IsPointing)
                {
                    if (!m_volumeLineRenderer.enabled)
                    {
                        m_volumeLineRenderer.enabled = true;
                    }

                    m_volumeLineRenderer.SetPosition(0, m_rayStart.position);
                    m_volumeLineRenderer.SetPosition(1, m_rayStart.position + m_rayStart.right * 100 * (isLeft ? 1 : 1));

                    Ray selectRay = new Ray(m_rayStart.position, m_rayStart.right * (isLeft ? 1 : 1));
                    RaycastHit selectHit;

                    if (Physics.Raycast(selectRay, out selectHit, 1000))
                    {
                        if (selectHit.collider.gameObject.tag == "Sections" && m_objectHit != selectHit.collider.gameObject)
                        {
                            if (m_volumeLineRenderer.enabled)
                            {
                                m_volumeLineRenderer.enabled = false;
                            }

                            if (m_objectHit != null)
                            {
                                iTween objTween = m_objectHit.GetComponent<iTween>();
                                if (objTween != null)
                                {
                                    Destroy(objTween);
                                }
                                iTween.ScaleTo(m_objectHit, iTween.Hash("scale", Vector3.one,
                                    "time", (1 - m_objectHit.transform.localScale.x) / .4f, "easetype", iTween.EaseType.easeOutSine));
                            }

                            m_lastSelectTime = Time.time;
                            m_objectHit = selectHit.collider.gameObject;
                            m_objectSource = m_objectHit.GetComponent<AudioSource>();
                            m_hitStartPos = Vector3.zero;

                            iTween.ScaleTo(m_objectHit, iTween.Hash("scale", Vector3.one * .8f,
                                "time", .5f, "looptype", iTween.LoopType.pingPong, "easeType", iTween.EaseType.easeInOutSine));
                        }
                    }
                }
                else
                {
                    if (m_volumeLineRenderer.enabled)
                    {
                        m_volumeLineRenderer.enabled = false;
                    }
                }
            }

            if (Time.time - m_lastSelectTime >= 4 && m_objectHit != null)
            {
                iTween objTween = m_objectHit.GetComponent<iTween>();
                if (objTween != null)
                {
                    Destroy(objTween);
                }
                iTween.ScaleTo(m_objectHit, iTween.Hash("scale", Vector3.one,
                    "time", (1 - m_objectHit.transform.localScale.x) / .4f, "easetype", iTween.EaseType.easeOutSine));

                m_objectHit = null;
            }

            if (m_objectHit != null)
            {
                if (m_hitStartPos == Vector3.zero && isOpen)
                {
                    m_hitStartPos = transform.position;
                }
                else if (m_palmUp && isOpen)
                {
                    if (transform.position.y < m_hitStartPos.y)
                    {
                        m_hitStartPos = transform.position;
                    }
                    else
                    {
                        float change = (transform.position.y - m_hitStartPos.y) / 10f;

                        if (change > 0 && !m_objectSource.isPlaying)
                        {
                            m_objectSource.UnPause();
                        }

                        float nextVolume = Mathf.Clamp01(m_objectSource.volume + change);

                        m_objectSource.volume = nextVolume;

                        if(GlobalInfo.Instance.CurrentFrame == GlobalInfo.FrameNumber.Four)
                        {
                            GlobalInfo.Instance.AdvanceFrame(GlobalInfo.FrameFour.Up);
                        }
                    }
                }
                else if (m_palmDown && isOpen)
                {
                    if (transform.position.y > m_hitStartPos.y)
                    {
                        m_hitStartPos = transform.position;
                    }
                    else
                    {
                        float change = (transform.position.y - m_hitStartPos.y) / 10f;

                        if (change > 0 && !m_objectSource.isPlaying)
                        {
                            m_objectSource.UnPause();
                        }

                        float nextVolume = Mathf.Clamp01(m_objectSource.volume + change);

                        m_objectSource.volume = nextVolume;

                        if (GlobalInfo.Instance.CurrentFrame == GlobalInfo.FrameNumber.Four)
                        {
                            GlobalInfo.Instance.AdvanceFrame(GlobalInfo.FrameFour.Down);
                        }
                    }
                }

                if (closed)
                {
                    m_objectSource.volume = 0;

                    iTween objTween = m_objectHit.GetComponent<iTween>();
                    if (objTween != null)
                    {
                        Destroy(objTween);
                    }
                    iTween.ScaleTo(m_objectHit, iTween.Hash("scale", Vector3.one,
                        "time", (1 - m_objectHit.transform.localScale.x) / .4f, "easetype", iTween.EaseType.easeOutSine));

                    m_objectHit = null;

                    if (GlobalInfo.Instance.CurrentFrame == GlobalInfo.FrameNumber.Four)
                    {
                        GlobalInfo.Instance.AdvanceFrame(GlobalInfo.FrameFour.Stop);
                    }
                }
            }
        }
    }
}