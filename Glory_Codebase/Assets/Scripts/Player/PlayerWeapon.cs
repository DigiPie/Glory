using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class PlayerWeapon : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private TextMeshProFloatingText floatingText_Script;
    private Vector2 dirV; // Direction of melee projectile
    public float damage = 10;
    private float stunDuration = 0.0f; // Stun duration on enemy
    private float blinkDuration = 1.0f; // Blink duration on enemy
    public float lifespan = 0.2f; // Lifespan of melee projectile
    public float speed = 0.05f; // Speed of melee projectile
    public Color damageCounterColour; // Damage counter colour
    public float damageCounterSize = 3;
    public Vector2 initialOffset;

    public void Setup(Vector2 dir)
    {
        Destroy(gameObject, lifespan);
        dirV = speed * dir;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (dir.x < 0)
        {
            transform.Translate(-initialOffset.x, initialOffset.y, 0);

            if (spriteRenderer != null)
                spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        else
        {
            transform.Translate(initialOffset.x, initialOffset.y, 0);
        }
    }

    public void Setup(Vector2 dir, float stunDuration)
    {
        Setup(dir);
        this.stunDuration = stunDuration;
    }

    public void Setup(Vector2 dir, float stunDuration, float blinkDuration)
    {
        Setup(dir, stunDuration);
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
        go.name = "Damage Counter";
        go.transform.position = new Vector3(
            pos.x + Random.Range(-0.2f, 0.2f), 
            pos.y + Random.Range(-0.2f, 0.2f), 
            1);
        TextMeshPro textMeshPro = go.AddComponent<TextMeshPro>();
        textMeshPro.autoSizeTextContainer = true;
        textMeshPro.rectTransform.pivot = new Vector2(0.5f, 0);

        textMeshPro.alignment = TextAlignmentOptions.Bottom;
        textMeshPro.fontSize = damageCounterSize;
        textMeshPro.enableKerning = false;

        textMeshPro.color = damageCounterColour;
        textMeshPro.text = damage.ToString();

        // Spawn Floating Text
        floatingText_Script = go.AddComponent<TextMeshProFloatingText>();
        go.AddComponent<DamageCounter>();
        floatingText_Script.SpawnType = 0;
        Destroy(floatingText_Script.m_floatingText, 2.0f);
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
