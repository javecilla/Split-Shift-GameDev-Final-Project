// enemy melee attack logic
using UnityEngine;

public class DogBehavior : EnemyBase
{
    [Header("Charged Attack")]
    public float chargedAttackDamage = 25f;
    public float chargedAttackCooldown = 4f;
    float chargedAttackTimer = 0f;

    bool isChargingAttack = false;

    protected override void Update()
    {
        base.Update();
        chargedAttackTimer += Time.deltaTime;
    }

    protected override void TriggerAttack()
    {
        if (chargedAttackTimer >= chargedAttackCooldown)
        {
            chargedAttackTimer = 0f;
            isChargingAttack = true;
            animator.SetTrigger("ChargedAttack");
            // ChargedAttackDamage() called via animation event frame
        }
        else
        {
            isChargingAttack = false;
            animator.SetTrigger("NormalAttack");
            // NormalAttackDamage() called via animation event frame
        }
    }

    // Called via animation event frame
    public void ChargedAttackDamage()
    {
        if (PlayerManager.Instance == null || Player == null) return;
        Debug.Log("Charged Attack! Damage: " + chargedAttackDamage);
        Player.GetComponent<PlayerBehavior>().TakeDamage(chargedAttackDamage);
    }
}