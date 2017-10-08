using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finger : MonoBehaviour
{
    public enum Digit
    {
        Pointer,
        Middle,
        Ring,
        Pinky,
        Thumb
    }

    [SerializeField]
    private Digit m_digit;

    public Digit MyDigit
    {
        get
        {
            return m_digit;
        }
    }

}