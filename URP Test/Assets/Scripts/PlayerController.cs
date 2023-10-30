using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float mf_BaseMoveSpeed;
    [SerializeField] float mf_CrouchSpeed;
    float mf_MoveSpeed;
    [SerializeField] float mf_JumpForce;
    [SerializeField] AnimationCurve m_AnimationCurve;
    [SerializeField] PlayerInput m_PlayerInput;
    Coroutine c_RMove;
    Coroutine c_RJumpBuffer;
    bool mb_InMoveActive;
    bool jumpBuffered;
    float mf_axis;


    [Header("Collision checks")]
    [SerializeField] Transform m_GroundCastPosition;
    [SerializeField] float mf_CircleRadius;
    bool isGrounded;

    [SerializeField] GameObject m_Head;
    [SerializeField] Collider2D m_HeadTrigger;
    Coroutine c_RCrouch;
    bool mb_InCrouchActive;
    bool isUnderGeometry;
    bool waitingToUncrouch;

    [SerializeField] LayerMask m_LayerMask;       


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

        mf_MoveSpeed = mf_BaseMoveSpeed;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.CircleCast(m_GroundCastPosition.position, mf_CircleRadius, Vector2.zero, 0, m_LayerMask);

        m_rb.velocity = new Vector2(mf_axis * mf_MoveSpeed, m_rb.velocity.y);

        //Physics.IgnoreLayerCollision(5, 7, (m_rb.velocity.y > 0.0f));

        if (jumpBuffered && isGrounded && !mb_InCrouchActive)
        {
            m_rb.AddForce(Vector2.up * mf_JumpForce);
            StopCoroutine(C_JumpBuffer());
            c_RJumpBuffer = null;
        }
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
        isUnderGeometry = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("Trigger has been exited!");
        isUnderGeometry = false;

        if (waitingToUncrouch && !m_Head.activeSelf)
        {
            //Debug.Log("Trigger exit has caused uncrouch!");
            waitingToUncrouch = false;
            m_Head.SetActive(true);
            mf_MoveSpeed = mf_BaseMoveSpeed;
            if (c_RCrouch !=  null)
            {
                StopCoroutine(c_RCrouch);
                c_RCrouch = null;
            }
        }
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        //Debug.Log("Jump pressed");
        if (isGrounded && !mb_InCrouchActive)
        {
            //Debug.Log("Jump Succeeded");
            m_rb.AddForce(Vector2.up * mf_JumpForce);
        }
        else if (!isGrounded || mb_InCrouchActive)
        {
            if (c_RJumpBuffer != null)
            {
                c_RJumpBuffer = StartCoroutine(C_JumpBuffer());
            }
            else
            {
                StopCoroutine(C_JumpBuffer());
                c_RJumpBuffer = null;
                c_RJumpBuffer = StartCoroutine(C_JumpBuffer());
            }
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
        if (c_RCrouch == null)
        {
            mf_MoveSpeed = mf_CrouchSpeed;
            m_Head.SetActive(false);
            c_RCrouch = StartCoroutine(C_CrouchUpdate());
        }
    }
    void Handle_CrouchCancelled(InputAction.CallbackContext context)
    {
        mb_InCrouchActive = false;        
        if (c_RCrouch != null && !isUnderGeometry)
        {
            mf_MoveSpeed = mf_BaseMoveSpeed;
            StopCoroutine(c_RCrouch);
            c_RCrouch = null;
            m_Head.SetActive(true);
        }
        else if (isUnderGeometry)
        {
            //Debug.Log("Uncrouched whilst under geometry!");
            waitingToUncrouch = true;
        }
    }

    IEnumerator C_MoveUpdate()
    {
        while (mb_InMoveActive)
        {
            //Debug.Log($"Move input = {mf_axis}");
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator C_CrouchUpdate()
    {
        while (mb_InCrouchActive)
        {
            //Debug.Log("Crouching Active");
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator C_JumpBuffer()
    {
        Debug.Log("Jump is buffered");
        jumpBuffered = true;
        yield return new WaitForSeconds(0.25f);
        jumpBuffered = false;
        yield break;
    }

    //public void PlayerMove(InputAction.CallbackContext context)
    //{
    //    mf_axis = context.ReadValue<float>();
    //}
}
