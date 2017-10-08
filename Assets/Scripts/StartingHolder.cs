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

    private int m_movingCount;

    [SerializeField]
    private Text m_debugText;

    [SerializeField]
    private AudioSource[] m_instruments;

    private void OnEnable()
    {
        StartCoroutine(MoveFakeObject());
    }

    private void Update()
    {
        if (!GlobalInfo.Instance.CheckStart)
        {
            if (!m_leftGesture.HasObject && !m_rightGesture.HasObject && m_leftGesture.IsPointing && m_rightGesture.IsPointing)
            {
                float yDiff = m_leftPointer.transform.position.y - m_rightPointer.transform.position.y;

                float sqrMag = (m_rightPointer.transform.position - m_leftPointer.transform.position).sqrMagnitude;

                m_debugText.text = "yDiff: " + yDiff + " sqrMag: " + sqrMag;

                if (Mathf.Abs(yDiff) < .02f && sqrMag < .004f)
                {
                    GlobalInfo.Instance.CheckStart = true;
                }
            }
        }

        if (!m_leftGesture.HasObject && !m_rightGesture.HasObject && m_leftGesture.isClosed && m_rightGesture.isClosed)
        {
            foreach (AudioSource i in m_instruments)
            {
                if (!i.isPlaying)
                {
                    i.Pause();
                }
            }
        }
    }

    IEnumerator MoveFakeObject()
    {
        while (true)
        {
            if (m_movingCount == m_leftCap.Tracked.Count)
            {
                m_movingCount = 0;
            }

            iTween.MoveTo(m_leftObj.gameObject, iTween.Hash("position", m_leftCap.Tracked[m_movingCount], "time", (3f / m_leftCap.Tracked.Count)));
            iTween.MoveTo(m_rightObj.gameObject, iTween.Hash("position", m_rightCap.Tracked[m_movingCount], "time", (3f / m_rightCap.Tracked.Count)));

            m_movingCount++;

            yield return new WaitForSeconds((3f / m_leftCap.Tracked.Count));
        }
    }
}