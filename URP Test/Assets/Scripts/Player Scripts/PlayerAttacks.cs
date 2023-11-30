using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    [Header("Light Attack 1 Vars")]       
    [SerializeField] int light1Damage;
    [SerializeField] float light1Duration;
    [SerializeField] GameObject Light1;

    [Header("Light Attack 2 Vars")]
    [SerializeField] int light2Damage;
    [SerializeField] float light2Duration;
    [SerializeField] GameObject Light2;

    [Header("Heavy Attack 1 Vars")]
    [SerializeField] int heavy1Damage;
    [SerializeField] float Heavy1Duration;
    [SerializeField] GameObject Heavy1;

    [Header("Heavy Attack 2 Vars")]
    [SerializeField] int heavy2Damage;
    [SerializeField] float Heavy2Duration;
    [SerializeField] GameObject Heavy2;

    [Header("Player Refs")]
    [SerializeField] PlayerController PlayerController;
    [SerializeField] Transform AttackPosition;

    public void PlayerLightAttack1()
    {
        GameObject ActiveLight1 = Instantiate(Light1, AttackPosition.position, Quaternion.identity);
        LightAttack1 lightAttack1 = ActiveLight1.GetComponent<LightAttack1>();
        
    }

    public void PlayerLightAttack2()
    {
        GameObject ActiveLight2 = Instantiate(Light2, AttackPosition.position, Quaternion.identity);
    }

    public void PlayerHeavyAttack1()
    {
        GameObject ActiveHeavy1 = Instantiate(Heavy1, AttackPosition.position, Quaternion.identity);
    }

    public void PlayerHeavyAttack2()
    {
        GameObject ActiveHeavy2 = Instantiate(Heavy2, AttackPosition.position, Quaternion.identity);
    }
}
