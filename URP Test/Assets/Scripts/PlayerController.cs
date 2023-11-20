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
    //[SerializeField] AnimationCurve m_AnimationCurve;
    float mf_MoveSpeed;
    bool leftFrozen;
    bool rightFrozen;
    
    [SerializeField] float mf_JumpForce;
    [SerializeField] float mf_JumpBufferTime;
    [SerializeField] float mf_JumpMaxTime;
    [SerializeField] float mf_ApexMaxTime;
    [SerializeField] float mf_ApexDelay;
    [SerializeField] float mf_ApexGravMod;
    float mf_BaseGravScale;
    [SerializeField] float mf_CoyoteTime;    
    bool mb_JumpHeld;
    bool mb_JumpRising;
    bool mb_JumpApex;
    
    
    [SerializeField] PlayerInput m_PlayerInput;
    Coroutine c_RMove;
    Coroutine c_RJump;
    Coroutine c_RJumpBuffer;
    bool mb_InMoveActive;
    bool jumpBuffered;
    float mf_Axis;

    [SerializeField] PlayerSemisolidPlatform m_PlatformController;
    Coroutine c_RUncrouchDowntime;
    float mf_UncrouchDowntime;

    [SerializeField] PlayerStickyCrouch m_StickyCrouch;

    [Header("Collision checks")]
    [SerializeField] Transform m_GroundCastPosition;
    //[SerializeField] float mf_CircleRadius;
    [SerializeField] float mf_BoxWidth;
    [SerializeField] float mf_BoxHeight;
    bool isGrounded;

    [SerializeField] GameObject m_Head;
    [SerializeField] Collider2D m_HeadTrigger;

    Coroutine c_RCrouch;
    bool isCrouching;
    bool forcedCrouch;
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
        //m_PlayerInput.actions.FindAction("Jump").performed += PlayerJump;
        m_PlayerInput.actions.FindAction("Jump").performed += Handle_JumpPerformed;
        m_PlayerInput.actions.FindAction("Jump").canceled += Handle_JumpCancelled;

        m_PlayerInput.actions.FindAction("Move").performed += Handle_MovePerformed;
        m_PlayerInput.actions.FindAction("Move").canceled += Handle_MoveCancelled;

        m_PlayerInput.actions.FindAction("Crouch").performed += Handle_CrouchPerformed;
        m_PlayerInput.actions.FindAction("Crouch").canceled += Handle_CrouchCancelled;

        mf_MoveSpeed = mf_BaseMoveSpeed;
        mf_BaseGravScale = m_rb.gravityScale;
    }

    void FixedUpdate()
    {
        //isGrounded = Physics2D.CircleCast(m_GroundCastPosition.position, mf_CircleRadius, Vector2.zero, 0, m_LayerMask);
        isGrounded = Physics2D.BoxCast(m_GroundCastPosition.position, new Vector2(mf_BoxWidth, mf_BoxHeight), 0.0f, Vector2.zero, 0.0f, m_LayerMask);
        m_StickyCrouch.UpdateGrounded(isGrounded);
        
        if (leftFrozen)
        {
            m_rb.velocity = new Vector2(Mathf.Clamp(mf_Axis, 0, 1) * mf_MoveSpeed, m_rb.velocity.y);
        }
        else if (rightFrozen)
        {
            m_rb.velocity = new Vector2(Mathf.Clamp(mf_Axis, -1, 0) * mf_MoveSpeed, m_rb.velocity.y);
        }
        else
        {
            {
                m_rb.velocity = new Vector2(mf_Axis * mf_MoveSpeed, m_rb.velocity.y);
            }
        }

        //Physics.IgnoreLayerCollision(5, 7, (m_rb.velocity.y > 0.0f));
        if (mb_JumpHeld)
        {
            if (mb_JumpRising)
            {
                m_rb.velocity = new Vector2(m_rb.velocity.x, 0.0f);
                m_rb.AddForce(Vector2.up * mf_JumpForce);
            }
            else if (mb_JumpApex)
            {
                m_rb.gravityScale *= mf_ApexGravMod;
            }
        }

        if (jumpBuffered && isGrounded && !isCrouching)
        {
            /*m_rb.velocity = Vector2.zero;
            m_rb.AddForce(Vector2.up * mf_JumpForce);*/
            StartCoroutine(C_Jumping());
            StopCoroutine(C_JumpBuffer());
            c_RJumpBuffer = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.yellow;
            //Gizmos.DrawSphere(m_GroundCastPosition.position, mf_CircleRadius);
            Gizmos.DrawCube(m_GroundCastPosition.position, new Vector3(mf_BoxWidth, mf_BoxHeight, mf_BoxWidth));
        }
        else
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawSphere(m_GroundCastPosition.position, mf_CircleRadius);
            Gizmos.DrawCube(m_GroundCastPosition.position, new Vector3(mf_BoxWidth, mf_BoxHeight, mf_BoxWidth));
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

        if (waitingToUncrouch && !forcedCrouch && !m_Head.activeSelf)
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

    public void ToggleHead()
    {
        if (m_Head.activeSelf) 
        {
            m_Head.SetActive(false);
        }
        else
        {
            m_Head.SetActive(true);
        }
    }

    public void UpdateLeftFrozen(bool leftFreezeState)
    {
        leftFrozen = leftFreezeState;        
    }

    public void UpdateRightFrozen(bool rightFreezeState)
    {
        rightFrozen = rightFreezeState;
    }

    void Handle_JumpPerformed(InputAction.CallbackContext context)
    {
        mb_JumpHeld = true;

        if (isGrounded && !isCrouching)
        {
            if (c_RJump == null)
            {
                c_RJump = StartCoroutine(C_Jumping());
            }
        }
        else if (isGrounded && isCrouching)
        {
            Debug.Log("Crouch jumped");
            m_PlatformController.PlayerCrouchJumped();
        }
        else if (!isGrounded || isCrouching)
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

    void Handle_JumpCancelled(InputAction.CallbackContext context) 
    {
        mb_JumpHeld = false;
        mb_JumpRising = false;
        mb_JumpApex = false;
        m_rb.gravityScale = mf_BaseGravScale;

        if (c_RJump != null)
        {
            StopCoroutine(c_RJump);
            c_RJump = null;
        }
    }

    void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        mf_Axis = context.ReadValue<float>();
        mb_InMoveActive = true;
        if (c_RMove == null)
        {
            c_RMove = StartCoroutine(C_MoveUpdate());
        }
    }

    void Handle_MoveCancelled(InputAction.CallbackContext context)
    {
        mf_Axis = context.ReadValue<float>();
        mb_InMoveActive = false;
        if (c_RMove != null)
        {
            StopCoroutine(c_RMove);
            c_RMove = null;
        }
    }

    void Handle_CrouchPerformed(InputAction.CallbackContext context)
    {
        isCrouching = true;
        m_StickyCrouch.UpdateCrouching(isCrouching);

        if (c_RCrouch == null)
        {
            mf_MoveSpeed = mf_CrouchSpeed;
            m_Head.SetActive(false);
            c_RCrouch = StartCoroutine(C_CrouchUpdate());
        }
    }

    //TODO: Remove all this forced crouch nonsense, add a script to playerhead that disables collision whilst falling.
    //NOTE: fixed player head issue, keeping the forcedCrouch code for now in case it's needed later. If not, then remove.
    void Handle_CrouchCancelled(InputAction.CallbackContext context)
    {
        if (!forcedCrouch)
        {
            isCrouching = false;
            m_StickyCrouch.UpdateCrouching(isCrouching);

            Debug.Log("uncrouching");

            if (c_RCrouch != null && !isUnderGeometry && !forcedCrouch)
            {
                Debug.Log("succesfully uncrouched");
                mf_MoveSpeed = mf_BaseMoveSpeed;
                StopCoroutine(c_RCrouch);
                c_RCrouch = null;
                m_Head.SetActive(true);
            }
            else if (isUnderGeometry)
            {
                Debug.Log("Uncrouched whilst under geometry!");
                waitingToUncrouch = true;
            }
        }
        else
        {
            if (c_RCrouch != null && !isUnderGeometry)
            {
                Debug.Log("succesfully uncrouched");
                mf_MoveSpeed = mf_BaseMoveSpeed;
                StopCoroutine(c_RCrouch);
                c_RCrouch = null;
                m_Head.SetActive(true);
            }
            else if (isUnderGeometry)
            {
                Debug.Log("Uncrouched whilst under geometry!");
                waitingToUncrouch = true;
            }
        }
    }

    IEnumerator C_Jumping()
    {
        while (mb_JumpHeld)
        {
            mb_JumpRising = true;
            yield return new WaitForSeconds(mf_JumpMaxTime);
            mb_JumpRising = false;

            yield return new WaitForSeconds(mf_ApexDelay);

            mb_JumpApex = true;
            yield return new WaitForSeconds(mf_ApexMaxTime); 
            mb_JumpApex = false;
            m_rb.gravityScale = mf_BaseGravScale;
            mb_JumpHeld = false;
        }
        yield break;
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
        while (isCrouching)
        {
            //Debug.Log("Crouching Active");
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator C_JumpBuffer()
    {
        Debug.Log("Jump is buffered");
        jumpBuffered = true;
        yield return new WaitForSeconds(mf_JumpBufferTime);
        jumpBuffered = false;
        yield break;
    }

    //IEnumerator C_UncrouchDowntime()
    //{
    //    forcedCrouch = true;
    //    Debug.Log("UncrouchDowntime active");
    //    yield return new WaitForSeconds(mf_UncrouchDowntime);
    //    forcedCrouch = false;
    //    if (!forcedCrouch && !waitingToUncrouch && !m_Head.activeSelf)
    //    {            
    //        m_Head.SetActive(true);
    //        mf_MoveSpeed = mf_BaseMoveSpeed;

    //        if (c_RCrouch != null)
    //        {
    //            StopCoroutine(c_RCrouch);
    //            c_RCrouch = null;
    //        }
    //    }
    //}
}
