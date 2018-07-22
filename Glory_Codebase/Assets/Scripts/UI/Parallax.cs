// Original Author: Rakesh Malik https://rakeshmalikblog.wordpress.com/2016/12/06/unity-scrolling-background/
// Modified by: Evan Tay

using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour
{
    public Rigidbody2D target;
    public float speed;

    private float startPos;
    private GameObject copy;
    private float width;
    private float targetVelocity;

    void Start()
    {
        startPos = transform.localPosition.x;
        copy = Instantiate(this.gameObject);
        Destroy(copy.GetComponent<Parallax>());
        copy.transform.SetParent(this.transform);
        copy.transform.localPosition = new Vector3(getWidth(), 0, 0);
    }

    void FixedUpdate()
    {
        targetVelocity = target.velocity.x;
        this.transform.Translate(new Vector3(-speed * targetVelocity, 0, 0) * Time.deltaTime);

        width = getWidth();
        if (targetVelocity > 0)
        {
            // Shift right if player moving right
            if (startPos - this.transform.localPosition.x > width)
            {
                this.transform.Translate(new Vector3(width, 0, 0));
            }
        }
        else
        {
            // Shift left if player moving left
            if (startPos - this.transform.localPosition.x < 0)
            {
                this.transform.Translate(new Vector3(-width, 0, 0));
            }
        }
    }

    float getWidth()
    {
        return GetComponent<SpriteRenderer>().bounds.size.x;
    }
}