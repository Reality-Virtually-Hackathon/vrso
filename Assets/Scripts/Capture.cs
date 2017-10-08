using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> m_tracked = new List<Vector3>();
    public List<Vector3> Tracked { get { return m_tracked; } }

    private bool m_started;

    [SerializeField]
    private LineRenderer m_lineRenderer;

    [SerializeField]
    private bool m_isLeft;

    [SerializeField]
    private GameObject m_moveObject;

    private int m_movingCount;

    private void OnEnable()
    {
        m_movingCount = 0;
        m_moveObject.SetActive(true);
        StartCoroutine(MoveFakeObject());
    }

    private void OnDisable()
    {
        m_moveObject.SetActive(false);
    }

    private void Awake()
    {
        for (int i = 0; i < m_tracked.Count; i++)
        {
            Vector3 val = m_tracked[i];

            if (m_isLeft)
            {
                val.x += 0.1f;
            }
            else
            {
                val.x -= 0.1f;
            }

            m_tracked[i] = val;
        }

        m_lineRenderer.positionCount = m_tracked.Count;

        for (int i = 0; i < m_tracked.Count; i++)
        {
            m_lineRenderer.SetPosition(i, m_tracked[i]);
        }
    }

    IEnumerator MoveFakeObject()
    {
        while (true)
        {
            if (m_movingCount == m_tracked.Count)
            {
                m_movingCount = 0;
            }

            iTween.MoveTo(m_moveObject.gameObject, iTween.Hash("position", m_tracked[m_movingCount], "time", (3f / m_tracked.Count)));

            m_movingCount++;

            yield return new WaitForSeconds((3f / m_tracked.Count));
        }
    }

    /*
    private void Awake()
    {
        for(int i =0; i < m_tracked.Count; i++)
        {
            Vector3 val = m_tracked[i];
            val.z = 0.45f;

            m_tracked[i] = val;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_started = !m_started;
        }

        if (m_started)
        {
            if (m_tracked.Count == 0)
            {
                m_tracked.Add(transform.position);
            }
            else
            {
                if ((transform.position - m_tracked[m_tracked.Count - 1]).sqrMagnitude > .001f)
                {
                    m_tracked.Add(transform.position);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 1; i < m_tracked.Count; i++)
        {
            Gizmos.DrawLine(m_tracked[i - 1], m_tracked[i]);
        }
    }
    */
}
