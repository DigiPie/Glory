using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponWithEffect : PlayerWeapon
{
    public GameObject effect;
    public float overtimeDamage;
    public float damageInterval;
    public float duration;
    public float slowMultiplier = 0;
    public bool isAboveHead;
}