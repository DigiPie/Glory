using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class PlayerWeapon : MonoBehaviour {
    private TextMeshProFloatingText floatingText_Script;
    private Vector2 dirV; // Direction of melee projectile
    private float damage = 10;
    private float stunDuration = 0.0f; // Stun duration on enemy
    private float blinkDuration = 1.0f; // Blink duration on enemy
    public float lifespan = 0.2f; // Lifespan of melee projectile
    public float speed = 0.05f; // Speed of melee projectile
    public Color damageCounterColour; // Damage counter colour

    public void Setup(Vector2 dir)
    {
        dirV = speed * dir;
        Destroy(gameObject, lifespan);
    }

    public void Setup(Vector2 dir, float damage)
    {
        dirV = speed * dir;
        Destroy(gameObject, lifespan);
        this.damage = damage;
    }

    public void Setup(Vector2 dir, float damage, float stunDuration)
    {
        dirV = speed * dir;
        Destroy(gameObject, lifespan);
        this.damage = damage;
        this.stunDuration = stunDuration;
    }

    public void Setup(Vector2 dir, float damage, float stunDuration, float blinkDuration)
    {
        dirV = speed * dir;
        Destroy(gameObject, lifespan);
        this.damage = damage;
        this.stunDuration = stunDuration;
        this.blinkDuration = blinkDuration;
    }

    void FixedUpdate()
    {
        transform.Translate(dirV.x, dirV.y, 0);
    }

    public void SpawnDamageCounter(Vector3 pos)
    {
        // TextMesh Pro Implementation
        GameObject go = new GameObject();
        go.transform.position = new Vector3(pos.x - 0.4f + Random.Range(0, 0.4f), 
            pos.y - 0.2f + Random.Range(0, 0.2f), 100);
        TextMeshPro textMeshPro = go.AddComponent<TextMeshPro>();

        textMeshPro.autoSizeTextContainer = true;
        textMeshPro.rectTransform.pivot = new Vector2(0.5f, 0);

        textMeshPro.alignment = TextAlignmentOptions.Bottom;
        textMeshPro.fontSize = 0.3f * damage;
        textMeshPro.enableKerning = false;

        textMeshPro.color = damageCounterColour;
        textMeshPro.text = "" + damage;

        // Spawn Floating Text
        floatingText_Script = go.AddComponent<TextMeshProFloatingText>();
        go.AddComponent<DamageCounter>();
        floatingText_Script.SpawnType = 0;
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetStunDuration()
    {
        return stunDuration;
    }

    public float GetBlinkDuration()
    {
        return blinkDuration;
    }
}
