using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField] Transform m_Target;
    [SerializeField] Vector3 mf_TargetOffset;
    [SerializeField] float mf_CameraSpeed;

    void Start()
    {
        if (m_Target == null)
        {
            m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void FixedUpdate()
    {
        Vector3 goalPosition = m_Target.position + mf_TargetOffset;
        Vector3 lerpPosition = Vector3.Lerp(transform.position, goalPosition, mf_CameraSpeed);
        transform.position = lerpPosition;
    }
}
