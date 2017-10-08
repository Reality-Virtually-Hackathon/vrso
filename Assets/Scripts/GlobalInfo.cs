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

    private int currentFrame = 0;

    private void Awake()
    {
        m_instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadNextLevel()
    {
        if(!SteamVR_LoadLevel.loading)
        {
            currentFrame++;
            SteamVR_LoadLevel.Begin("Frame" + currentFrame);
        }
    }
}