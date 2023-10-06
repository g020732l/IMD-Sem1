using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] float mf_MoveSpeed;
    [SerializeField] float mf_JumpForce;
    [SerializeField] Transform m_CastPosition;
    [SerializeField] float mf_CircleRadius;
    [SerializeField] LayerMask m_LayerMask;
    [SerializeField] float mf_axis;

    bool isGrounded;

    Rigidbody2D m_rb;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.CircleCast(m_CastPosition.position, mf_CircleRadius, Vector2.zero, 0, m_LayerMask);

        m_rb.velocity = new Vector2(mf_axis * mf_MoveSpeed, m_rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_CastPosition.position, mf_CircleRadius);
        }
        else
        {
            Gizmos.color= Color.red;
            Gizmos.DrawSphere(m_CastPosition.position, mf_CircleRadius);
        }
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            m_rb.AddForce(Vector2.up * mf_JumpForce);
        }
    }


    public void PlayerMove(InputAction.CallbackContext context)
    {
        mf_axis = context.ReadValue<float>();
    }
}
