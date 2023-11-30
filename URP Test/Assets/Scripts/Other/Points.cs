using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Points : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float speed;
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public float launchForce;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO: collision is disabled, GoToPlayer() is invoked
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: on overlap with player, add points to manager and destroy self
    }

    IEnumerator GoToPlayer()
    {
        yield return null;
    }
}
