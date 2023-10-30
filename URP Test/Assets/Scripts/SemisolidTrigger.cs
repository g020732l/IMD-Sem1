using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SemisolidTrigger : MonoBehaviour
{
    GameObject platform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        platform = transform.parent.gameObject;
        //Physics2D.IgnoreCollision(collision.GetComponent<BoxCollider2D>(), platform.GetComponent<BoxCollider2D>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
