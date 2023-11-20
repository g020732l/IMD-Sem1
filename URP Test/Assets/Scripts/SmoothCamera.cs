using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField] Transform m_Target;
    [SerializeField] float mf_TargetOffset;

    void Start()
    {
        if (m_Target == null)
        {
            m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void FixedUpdate()
    {
        
    }
}
