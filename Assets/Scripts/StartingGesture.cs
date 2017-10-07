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
        for(int i = 0; i < m_handPoints.Length; i++)
        {
            if(a_hitObject == m_handPoints[i])
            {
                if(i == (lastHit + 1))
                {
                    lastHit = i;
                    m_handPoints[i].SetActive(false);
                }
                else if(i != lastHit)
                {
                    lastHit = -1;
                    foreach(GameObject hp in m_handPoints)
                    {
                        hp.SetActive(true);
                    }

                    break;
                }
            }
        }

        if(lastHit == m_handPoints.Length - 1)
        {
            foreach (AudioSource i in m_instruments)
            {
                if (!i.isPlaying)
                {
                    i.Play();
                }
            }
        }

        m_other.enabled = false;
        this.enabled = false;
    }
}