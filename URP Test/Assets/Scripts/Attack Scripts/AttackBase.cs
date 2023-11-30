using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class AttackBase : MonoBehaviour
{
    public float mf_Startup;
    public float mf_Duration;
    public float mf_Damage;
    [SerializeField] Vector2 m_LaunchDirection;
    [SerializeField] Vector2 m_LaunchForce;
}
