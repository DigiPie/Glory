using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;

public class Effect : MonoBehaviour {
    private TextMeshProFloatingText floatingText_Script;
    private EnemyHealthSystem enemyHealthSystem;
    private float lifespan;
    private float damage;
    private float damageInterval;
    private float damageReadyTime;
    public Color damageCounterColour; // Damage counter colour
    public float damageCounterSize = 3;

    public void Setup(EnemyHealthSystem enemyHealthSystem, float damage, float damageInterval, float damageDuration)
    {
        this.enemyHealthSystem = enemyHealthSystem;
        this.damage = damage;
        this.damageInterval = damageInterval;

        lifespan = damageDuration;
        Destroy(gameObject, lifespan);

        damageReadyTime = Time.timeSinceLevelLoad + this.damageInterval;
    }

    private void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > damageReadyTime)
        {
            enemyHealthSystem.DeductHealth(damage);
            damageReadyTime = Time.timeSinceLevelLoad + damageInterval;
            SpawnDamageCounter();
        }
    }

    public void SpawnDamageCounter()
    {
        // TextMesh Pro Implementation
        GameObject go = new GameObject();
        go.name = "Damage Counter";
        go.transform.position = new Vector3(
            transform.position.x + Random.Range(-0.2f, 0.2f),
            transform.position.y + Random.Range(-0.2f, 0.2f),
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
}
