using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuff : MonoBehaviour {
    public float lifespan = 4f; // Lifespan of buff
    public float invulDuration = 4f;
    public float fasterSpeedDuration = 4f;
    public float speedMultiplier = 1.5f;

    public void Setup()
    {
        Destroy(gameObject, lifespan);
    }

    public void SetToPosition(Vector3 position)
    {
        this.transform.position = position;
    }
}
