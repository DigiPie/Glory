using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponWithEffect : PlayerWeapon
{
    public GameObject effect;
    public float overtimeDamage;
    public float damageInterval;
    public float duration;

    public void SpawnEffect(EnemyHealthSystem enemyHealthSystem, Transform followTarget)
    {
        GameObject newEffect = Instantiate(effect, followTarget.transform);
        newEffect.transform.parent = followTarget.transform;
        newEffect.GetComponent<Effect>().Setup(enemyHealthSystem, overtimeDamage, damageInterval, duration);
    }
}