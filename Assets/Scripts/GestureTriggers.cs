using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureTriggers : MonoBehaviour
{
    private GestureController m_gestureController;
    [SerializeField]
    private GestureController.Location m_location;

    private void Awake()
    {
        m_gestureController = GetComponentInParent<GestureController>();
    }

    private void OnTriggerEnter(Collider a_col)
    {
        m_gestureController.AddFinger(m_location, a_col.gameObject);
    }

    private void OnTriggerExit(Collider a_col)
    {
        m_gestureController.RemoveFinger(m_location, a_col.gameObject);
    }
}