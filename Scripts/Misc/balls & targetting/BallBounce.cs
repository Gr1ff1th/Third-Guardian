using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBounce : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 LastVelocity;
    Vector2 direction;

    //ignore there gameobjects
    public GameObject objectToIgnore;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Physics2D.IgnoreCollision(objectToIgnore.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        LastVelocity = rb.velocity;
    }

    public void ResetIgnoreCollision()
    {
        Physics2D.IgnoreCollision(objectToIgnore.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        //use tag for this
        if (collision.gameObject == objectToIgnore)//(collision.gameObject.tag == "BallObject")
        {
            Physics2D.IgnoreCollision(objectToIgnore.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
            return;
        }
        */

        var speed = LastVelocity.magnitude;
        direction = Vector2.Reflect(LastVelocity.normalized, collision.GetContact(0).normal);//collision.contacts[0].normal);
        rb.velocity = direction * Mathf.Max(speed, 0f);

        /*lets test with this on for a while, although doesnt seem to fix the issue entirely at least
        if(collision.contactCount > 1)
        {
            speed = rb.velocity.magnitude;
            direction = Vector3.Reflect(rb.velocity.normalized, collision.GetContact(1).normal);//collision.contacts[0].normal);
            rb.velocity = direction * Mathf.Max(speed, 0f);
        }
        */
    }
}
