using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStickyCrouch : MonoBehaviour
{
    [SerializeField] PlayerController m_PlayerController;
    //TODO: When drawing gizmos, Unity insists this variable hasn't been assigned in the inspector and I don't know why.
    [SerializeField] private Transform m_CastPosLeft;
    [SerializeField] private Transform m_CastPosRight;
    [SerializeField] private float mf_CastLength;
    [SerializeField] bool leftGrounded;
    [SerializeField] bool rightGrounded;
    [SerializeField] bool isCrouching;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask m_LayerMask;

    private void FixedUpdate()
    {        
        leftGrounded = Physics2D.Linecast(m_CastPosLeft.position, new Vector2(m_CastPosLeft.position.x, (m_CastPosLeft.position.y - mf_CastLength)), m_LayerMask);
        rightGrounded = Physics2D.Linecast(m_CastPosRight.position, new Vector2(m_CastPosRight.position.x, (m_CastPosRight.position.y - mf_CastLength)), m_LayerMask);

        if (isCrouching && isGrounded)
        {
            if (!leftGrounded)
            {
                m_PlayerController.UpdateLeftFrozen(true);
            }
            else
            {
                m_PlayerController.UpdateLeftFrozen(false);
            }

            if (!rightGrounded)
            {
               m_PlayerController.UpdateRightFrozen(true);
            }  
            else
            {
                m_PlayerController.UpdateRightFrozen(false);
            }
        }
        else if (!isCrouching || !isGrounded)
        {
            m_PlayerController.UpdateLeftFrozen(false);
            m_PlayerController.UpdateRightFrozen(false);
        }
    }

    //TODO: Reference these in the playercontroller 
    public void UpdateCrouching(bool crouchState)
    {
        isCrouching = crouchState;
    }

    public void UpdateGrounded(bool groundedState)
    {
        isGrounded = groundedState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(m_CastPosLeft.position, new Vector2(m_CastPosLeft.position.x, (m_CastPosLeft.position.y - mf_CastLength)));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(m_CastPosRight.position, new Vector2(m_CastPosRight.position.x, (m_CastPosRight.position.y - mf_CastLength)));
    }
}
