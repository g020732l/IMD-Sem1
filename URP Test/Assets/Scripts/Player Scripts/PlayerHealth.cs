using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Health Vars")]
    [SerializeField] int maxHealth;
    int currentHealth;

    [Header("Invuln Vars")]
    [SerializeField] bool invulnAble;
    [SerializeField] float invulnDuration;
    bool invulnActive = false;

    [Header("Point Vars")]
    [SerializeField] bool dropsPoints = true;
    [SerializeField] int minPoints;
    [SerializeField] int maxPoints;
    [SerializeField] GameObject pointDrop;


    [Header("HUD Elements")]
    [SerializeField] bool hudAble;
    [SerializeField] TextMeshProUGUI healthText;

    [Header("Other")]
    [SerializeField] bool player;
    Coroutine c_RInvuln;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Damaged(int incomingDamage)
    {
        if (!invulnActive)
        {
            this.currentHealth = Mathf.Clamp(currentHealth - incomingDamage, 0, maxHealth);
        }

        if (hudAble)
        {
            healthText.text = $"HP {currentHealth}/{maxHealth}";
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        if (invulnAble)
        {

            StartCoroutine(Invuln());
        }
    }

    private void Die()
    {
        if (player)
        {

        }
        else
        {
            if (dropsPoints)
            {
                for (int i = 0; i <= Random.Range(minPoints, maxPoints); i++)
                {
                    var launchDirection = new Vector3(0, 0, Random.Range(-135.0f, 135.0f));
                    float launchForce = Random.Range(200.0f, 400.0f);
                    GameObject point = Instantiate(pointDrop, transform.position, Quaternion.identity);
                    Points points = point.GetComponent<Points>();
                    //points.direction = 
                    points.launchForce = launchForce;
                }
            }
            Destroy(gameObject);
        }
    }

    IEnumerator Invuln()
    {
        invulnActive = true;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        yield return new WaitForSeconds(invulnDuration);
        invulnActive = false;
        GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        InvulnOver();
    }

    private void InvulnOver()
    {
        if(c_RInvuln != null)
        {
            StopCoroutine(Invuln());
            c_RInvuln = null;
        }        
    }
}
