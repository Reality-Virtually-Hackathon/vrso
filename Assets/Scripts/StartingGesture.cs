using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingGesture : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_handPoints;

    [SerializeField]
    private StartingGesture m_other;

    [SerializeField]
    private AudioSource[] m_instruments;

    [SerializeField]
    private Capture m_capture;
    public Capture Capture { get { return m_capture; } }

    private List<Vector3> m_tracked = new List<Vector3>();

    private int lastHit = -1;
    private bool init = false;

    private void Update()
    {
        if (GlobalInfo.Instance.CheckStart)
        {
            if(!init)
            {
                m_tracked.Clear();
                init = true;
            }

            if (m_tracked.Count == 0)
            {
                m_tracked.Add(transform.position);
            }
            else
            {
                Vector3 pos = transform.position;
                Vector3 check = m_tracked[m_tracked.Count - 1];
                pos.z = check.z = 0;
                if ((pos - check).sqrMagnitude > .001f)
                {
                    m_tracked.Add(transform.position);
                }

                if (m_tracked.Count == m_capture.Tracked.Count)
                {
                    for (int i = 0; i < m_tracked.Count; i++)
                    {
                        Vector3 val = m_tracked[i];
                        val.z = 0.45f;

                        m_tracked[i] = val;
                    }

                    float dif = 0;
                    for (int i = 0; i < m_tracked.Count; i++)
                    {
                        dif += Vector3.Distance(m_capture.Tracked[i], m_tracked[i]);
                    }

                    if (dif / 50f < 1)
                    {
                        foreach (AudioSource i in m_instruments)
                        {
                            i.volume = .4f;

                            if (!i.isPlaying)
                            {
                                i.Play();
                            }
                        }

                        m_capture.gameObject.SetActive(false);
                        m_other.Capture.gameObject.SetActive(false);
                        m_other.gameObject.SetActive(false);
                        gameObject.SetActive(false);
                        GlobalInfo.Instance.Started = true;

                        if(GlobalInfo.Instance.CurrentFrame == GlobalInfo.FrameNumber.One)
                        {
                            GlobalInfo.Instance.AdvanceFrame();
                        }
                    }
                    else
                    {
                        m_handPoints[0].SetActive(true);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        init = false;
    }
}