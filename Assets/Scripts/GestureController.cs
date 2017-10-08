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

    private List<GameObject> m_upFingers = new List<GameObject>();
    private List<GameObject> m_downFingers = new List<GameObject>();

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
    private AudioSource m_objectSource;
    private Vector3 m_hitStartPos;
    private float m_lastSelectTime;

    [SerializeField]
    private bool isLeft;

    public void AddFinger(Location a_location, GameObject a_finger)
    {
        switch (a_location)
        {
            case Location.Up:
                if (!m_upFingers.Contains(a_finger))
                {
                    m_upFingers.Add(a_finger);
                }
                break;
            case Location.Down:
                if (!m_downFingers.Contains(a_finger))
                {
                    m_downFingers.Add(a_finger);
                }
                break;
        }
    }

    public void RemoveFinger(Location a_location, GameObject a_finger)
    {
        switch (a_location)
        {
            case Location.Up:
                if (m_upFingers.Contains(a_finger))
                {
                    m_upFingers.Remove(a_finger);
                }
                break;
            case Location.Down:
                if (m_downFingers.Contains(a_finger))
                {
                    m_downFingers.Remove(a_finger);
                }
                break;
        }
    }

    private void Update()
    {
        float dot = Vector3.Dot(transform.up, Vector3.up);

        if (dot < -.5f)
        {
            m_rayStart.localRotation = Quaternion.Euler(0, 0, -15f);
        }
        else if (dot > .5f)
        {
            m_rayStart.localRotation = Quaternion.Euler(0, 0, 15f);

        }

        m_volumeLineRenderer.SetPosition(0, m_rayStart.position);
        m_volumeLineRenderer.SetPosition(1, m_rayStart.position + m_rayStart.right * 100 * (isLeft ? -1 : 1));

        if (!GlobalInfo.Instance.Started)
        {
            return;
        }

        /*
        m_stopLineRenderer.SetPosition(0, transform.position);
        m_stopLineRenderer.SetPosition(1, transform.position + transform.up * 100);
        */

        if (opened)
        {
            opened = false;
        }
        if (closed)
        {
            closed = false;
        }

        int openCount = m_upFingers.Count;
        int closedCount = m_downFingers.Count;

        if (openCount >= 4)
        {
            if (!isOpen)
            {
                opened = true;
                Debug.Log("We've Opened");
            }

            isOpen = true;
        }

        if (closedCount >= 4)
        {
            if (!isClosed)
            {
                closed = true;
                Debug.Log("We've Closed");
            }

            isClosed = true;
        }

        if (openCount < 3)
        {
            isOpen = false;
        }

        if (closedCount < 3)
        {
            isClosed = false;
        }

        if (Time.time - m_lastSelectTime > 1)
        {
            if (openCount == 2 || openCount == 1)
            {
                Ray selectRay = new Ray(m_rayStart.position, m_rayStart.right * (isLeft ? -1 : 1));
                RaycastHit selectHit;

                if (Physics.Raycast(selectRay, out selectHit, 1000))
                {
                    if (selectHit.collider.gameObject.tag == "Sections")
                    {
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
            if (opened || closed)
            {
                m_hitStartPos = transform.position;
            }
            else if (m_hitStartPos == Vector3.zero && isOpen)
            {
                m_hitStartPos = transform.position;
            }
            else if (isOpen)
            {
                float change = (transform.position.y - m_hitStartPos.y) / 2f;

                if (change > 0 && !m_objectSource.isPlaying)
                {
                    m_objectSource.UnPause();
                }

                float nextVolume = Mathf.Clamp01(m_objectSource.volume + change);

                m_objectSource.volume = nextVolume;
            }

            if (closed)
            {
                m_objectSource.Pause();

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
    }
}