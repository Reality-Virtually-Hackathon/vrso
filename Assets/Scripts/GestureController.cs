using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureController : MonoBehaviour
{
    public enum Location
    {
        Up,
        Down
    }

    private List<GameObject> m_upFingers = new List<GameObject>();
    private List<GameObject> m_downFingers = new List<GameObject>();

    public bool isOpen;
    public bool opened;
    private float openTime;
    public bool isClosed;
    public bool closed;
    private float closedTime;

    public void AddFinger(Location a_location, GameObject a_finger)
    {
        switch (a_location)
        {
            case Location.Up:
                if (!m_upFingers.Contains(a_finger))
                {
                    m_upFingers.Add(a_finger);
                }
                break;
            case Location.Down:
                if (!m_downFingers.Contains(a_finger))
                {
                    m_downFingers.Add(a_finger);
                }
                break;
        }
    }

    public void RemoveFinger(Location a_location, GameObject a_finger)
    {
        switch (a_location)
        {
            case Location.Up:
                if (m_upFingers.Contains(a_finger))
                {
                    m_upFingers.Remove(a_finger);
                }
                break;
            case Location.Down:
                if (m_downFingers.Contains(a_finger))
                {
                    m_downFingers.Remove(a_finger);
                }
                break;
        }
    }

    private void Update()
    {
        if(opened)
        {
            opened = false;
        }
        if(closed)
        {
            closed = false;
        }

        int openCount = m_upFingers.Count;
        int closedCount = m_downFingers.Count;

        if(openCount >= 3)
        {
            openTime = Time.time;

            isOpen = true;

            if(Time.time - closedTime <= 1f)
            {
                closedTime = 0;
                opened = true;
                Debug.Log("We've Opened");
            }
        }

        if(closedCount >= 3)
        {
            closedTime = Time.time;

            isClosed = true;

            if(Time.time - openTime <= 1f)
            {
                openTime = 0;
                closed = true;
                Debug.Log("We've Closed");
            }
        }

        if(openCount < 3)
        {
            isOpen = false;
        }

        if(closedCount < 3)
        {
            isClosed = false;
        }
    }
}