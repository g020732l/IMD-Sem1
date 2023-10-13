using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float mf_MoveSpeed;
    [SerializeField] float mf_JumpForce;
    [SerializeField] AnimationCurve m_AnimationCurve;

    [SerializeField] PlayerInput m_PlayerInput;
    Coroutine c_RMove;
    bool mb_InMoveActive;
    float mf_axis;

    [SerializeField] GameObject m_Head;
    //[SerializeField] Collider2D m_HeadCollider;
    Coroutine c_RCrouch;
    bool mb_InCrouchActive;
    bool isUnderGeometry;
    bool waitingToUncrouch;

    [Header("Collision checks")]
    [SerializeField] Transform m_GroundCastPosition;
    [SerializeField] float mf_CircleRadius;
    //[SerializeField] Transform m_HeadCastPosition;
    //[SerializeField] Vector2 mf_BoxRadius;
    [SerializeField] LayerMask m_LayerMask;
    bool isGrounded;    
    
    Rigidbody2D m_rb;
    

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_PlayerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        m_PlayerInput.actions.FindAction("Jump").performed += PlayerJump;

        m_PlayerInput.actions.FindAction("Move").performed += Handle_MovePerformed;
        m_PlayerInput.actions.FindAction("Move").canceled += Handle_MoveCancelled;

        m_PlayerInput.actions.FindAction("Crouch").performed += Handle_CrouchPerformed;
        m_PlayerInput.actions.FindAction("Crouch").canceled += Handle_CrouchCancelled;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.CircleCast(m_GroundCastPosition.position, mf_CircleRadius, Vector2.zero, 0, m_LayerMask);
        //isUnderGeometry = Physics2D.BoxCast(m_HeadCastPosition.position, mf_BoxRadius, 0, m_LayerMask);

        m_rb.velocity = new Vector2(mf_axis * mf_MoveSpeed, m_rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_GroundCastPosition.position, mf_CircleRadius);
        }
        else
        {
            Gizmos.color= Color.red;
            Gizmos.DrawSphere(m_GroundCastPosition.position, mf_CircleRadius);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump pressed");
        if (isGrounded && !mb_InCrouchActive)
        {
            Debug.Log("Jump Succeeded");
            m_rb.AddForce(Vector2.up * mf_JumpForce);
        }
    }

    void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        mf_axis = context.ReadValue<float>();
        mb_InMoveActive = true;
        if (c_RMove == null)
        {
            c_RMove = StartCoroutine(C_MoveUpdate());
        }
    }
    void Handle_MoveCancelled(InputAction.CallbackContext context)
    {
        mf_axis = context.ReadValue<float>();
        mb_InMoveActive = false;
        if (c_RMove != null)
        {
            StopCoroutine(c_RMove);
            c_RMove = null;
        }
    }

    void Handle_CrouchPerformed(InputAction.CallbackContext context)
    {
        mb_InCrouchActive = true;
        m_Head.SetActive(false);
        if (c_RCrouch == null)
        {
            c_RCrouch = StartCoroutine(C_CrouchUpdate());
        }
    }
    void Handle_CrouchCancelled(InputAction.CallbackContext context)
    {
        mb_InCrouchActive = false;
        m_Head.SetActive(true);
        if (c_RCrouch != null && !isUnderGeometry)
        {
            StopCoroutine(c_RCrouch);
            c_RCrouch = null;
        }
        else if (isUnderGeometry)
        {

        }
    }

    IEnumerator C_MoveUpdate()
    {
        while (mb_InMoveActive)
        {
            Debug.Log($"Move input = {mf_axis}");
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator C_CrouchUpdate()
    {
        while (mb_InCrouchActive)
        {
            Debug.Log("Crouching Active");
            yield return new WaitForFixedUpdate();
        }
    }

    //public void PlayerMove(InputAction.CallbackContext context)
    //{
    //    mf_axis = context.ReadValue<float>();
    //}
}
