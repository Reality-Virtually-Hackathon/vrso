using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingHolder : MonoBehaviour
{
    [SerializeField]
    private Capture m_LeftCap;

    [SerializeField]
    private GameObject m_LeftObj;

    [SerializeField]
    private Capture m_RightCap;

    [SerializeField]
    private GameObject m_RightObj;

    private int m_movingCount;

    private void OnEnable()
    {
        StartCoroutine(MoveFakeObject());
    }

    IEnumerator MoveFakeObject()
    {
        while (true)
        {
            if (m_movingCount == m_LeftCap.Tracked.Count)
            {
                m_movingCount = 0;
            }

            iTween.MoveTo(m_LeftObj.gameObject, iTween.Hash("position", m_LeftCap.Tracked[m_movingCount], "time", (3f / m_LeftCap.Tracked.Count)));
            iTween.MoveTo(m_RightObj.gameObject, iTween.Hash("position", m_RightCap.Tracked[m_movingCount], "time", (3f / m_RightCap.Tracked.Count)));

            m_movingCount++;

            yield return new WaitForSeconds((3f / m_LeftCap.Tracked.Count));
        }
    }
}