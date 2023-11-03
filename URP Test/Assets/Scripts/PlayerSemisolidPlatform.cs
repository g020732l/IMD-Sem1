using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSemisolidPlatform : MonoBehaviour
{
    private GameObject currentPlatform;
    [SerializeField] private BoxCollider2D playerCollider;
    //[SerializeField] private PlayerController playerController;
    [SerializeField] private float collisionDowntime = 0.2f;
    Coroutine c_RDropThroughPlat;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("SemisolidPlatform"))
        {
            currentPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("SemisolidPlatform"))
        {
            currentPlatform = null;
        }
    }

    public void PlayerCrouchJumped()
    {
        Debug.Log("CrouchJumped called");

        if (currentPlatform != null) 
        {
            if (c_RDropThroughPlat == null)
            {
                Debug.Log("Should drop through platform here");

                c_RDropThroughPlat = StartCoroutine(DisableCollision());
            }
            else
            {
                c_RDropThroughPlat = null;

                Debug.Log("Reset coroutine; should drop through platform here");

                c_RDropThroughPlat = StartCoroutine(DisableCollision());
            }
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentPlatform.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(collisionDowntime);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
