using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingTrigger : MonoBehaviour
{
    [SerializeField]
    private StartingGesture m_startingGesture;

    private void OnTriggerEnter(Collider a_col)
    {
        m_startingGesture.HitTrigger(gameObject);
    }
}