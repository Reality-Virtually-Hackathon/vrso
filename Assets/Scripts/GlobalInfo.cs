using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInfo : MonoBehaviour
{
    private static GlobalInfo m_instance;

    public static GlobalInfo Instance { get { return m_instance; } }

    [HideInInspector]
    public bool Started;

    [HideInInspector]
    public bool CheckStart;

    private void Awake()
    {
        m_instance = this;
    }
}
