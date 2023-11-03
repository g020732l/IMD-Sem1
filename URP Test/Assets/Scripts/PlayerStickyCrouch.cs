using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStickyCrouch : MonoBehaviour
{
    [SerializeField] BoxCollider2D m_Trigger0;
    [SerializeField] BoxCollider2D m_Trigger1;
    bool isCrouching;

    public void UpdateCrouching(bool crouchState)
    {
        isCrouching = crouchState;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
