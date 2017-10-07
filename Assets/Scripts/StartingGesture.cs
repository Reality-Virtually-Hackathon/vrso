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

    private int lastHit = -1;

    public void HitTrigger(GameObject a_hitObject)
    {
        for (int i = 0; i < m_handPoints.Length; i++)
        {
            if (a_hitObject == m_handPoints[i])
            {
                if (i == (lastHit + 1))
                {
                    lastHit = i;
                    m_handPoints[i].SetActive(false);

                    if (i + 1 < m_handPoints.Length)
                    {
                        m_handPoints[i + 1].SetActive(true);
                    }
                }
                else if (i != lastHit)
                {
                    lastHit = -1;

                    for (int j = 1; j < m_handPoints.Length; j++)
                    {
                        m_handPoints[j].SetActive(false);
                    }

                    m_handPoints[0].SetActive(true);

                    break;
                }
            }
        }

        if (lastHit == m_handPoints.Length - 1)
        {
            foreach (AudioSource i in m_instruments)
            {
                if (!i.isPlaying)
                {
                    i.Play();
                }
            }

            m_other.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}