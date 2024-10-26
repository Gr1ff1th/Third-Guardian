using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;

    public Vector2 startingVector;

    public Sprite attackPhaseIcon;
    public Sprite defensePhaseIcon;

    //transferred from ballbounce
    public Vector2 LastVelocity;
    Vector2 direction;

    //ignore there gameobjects
    public GameObject objectToIgnore;
    public GameObject objectToIgnore2;
    public GameObject objectToIgnore3;

    public Vector3 startPosition;

    //public int minForce;
    //public int maxForce;

    public bool isTargetBall;
    public bool isCrosshairBall;
    public bool isTarget2Ball;
    public bool isCrosshair2Ball;

    public GameObject heroImage;

    // Start is called before the first frame update
    void Start()
    {
        //int randomForce1 = Random.Range(-20, 20);
        //int randomForce2 = Random.Range(-20, 20);

        //this doesnt seem to work properly for some reason
        //startPosition = gameObject.transform.position;

        rb = GetComponent<Rigidbody2D>();

        Physics2D.IgnoreCollision(objectToIgnore.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(objectToIgnore2.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(objectToIgnore3.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());

        startingVector = RandomUnitVector();

        rb.AddForce(startingVector);//new Vector2(randomForce1 * Time.deltaTime * speed, randomForce2 * Time.deltaTime * speed));//(new Vector2(-20 * Time.deltaTime * speed, -20 * Time.deltaTime * speed));
    }

    // Update is called once per frame
    void Update()
    {
        LastVelocity = rb.velocity;
        /*
        if(LastVelocity.normalized.x < 0.01f && LastVelocity.normalized.x > -0.01f)
        {
            Halt();
            gameObject.transform.position = startPosition;
            Restart();
        }

        else if (LastVelocity.normalized.y < 0.01f && LastVelocity.normalized.y > -0.01f)
        {
            Halt();
            gameObject.transform.position = startPosition;
            Restart();
        }
                
        Debug.Log("lastvelocity x is: " + LastVelocity.x);
        Debug.Log("lastvelocity y is: " + LastVelocity.y);
        */
    }

    public void ResetIgnoreCollision()
    {
        Physics2D.IgnoreCollision(objectToIgnore.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(objectToIgnore2.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(objectToIgnore3.gameObject.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
    }

    public Vector3 RandomUnitVector()
    {
        //why use deltatime for this purpose, since the speed should be fixed
        //Debug.Log("time.deltatime is: " + Time.deltaTime);

        //float random = Random.Range(0f, 360f);
        float random = Random.Range(0f, 2 * Mathf.PI);

        /* actually lets not do this right now
        if(isTargetBall == true)
        {
            random = 0f;
        }
        */
        return new Vector3(Mathf.Cos(random) * 0.02f * speed, Mathf.Sin(random) * 0.02f * speed);//(Mathf.Cos(random) * Time.deltaTime * speed, Mathf.Sin(random) * Time.deltaTime * speed);


    }

    public void Halt()
    {
        rb = GetComponent<Rigidbody2D>();
        //gameObject.GetComponent<BallBounce>().LastVelocity = new Vector2(0, 0);
        rb.velocity = new Vector2(0, 0);
    }

    public void Restart()
    {
        Vector3 restartVector = RandomUnitVector();

        //test combatpause check here also?
        if (rb.velocity == Vector2.zero && GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().combatPaused == false)
        {
            if (isTarget2Ball)
            {
                gameObject.transform.position = GameManager.ins.references.targettingHandler.targetStartPosition;
            }
            else if (isCrosshair2Ball)
            {
                gameObject.transform.position = GameManager.ins.references.targettingHandler.crosshairStartPosition;
            }
            else if (isTargetBall)
            {
                gameObject.transform.position = GameManager.ins.references.targettingHandler.targetStartPosition;
            }
            else if (isCrosshairBall)
            {
                gameObject.transform.position = GameManager.ins.references.targettingHandler.crosshairStartPosition;
            }
            rb.AddForce(restartVector);
        }
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

        TestIfBallStuckOnAnyAxis();

        //Invoke("TestIfBallStuckOnAnyAxis", 1.0f);

        /*lets test with this on for a while, although doesnt seem to fix the issue entirely at least
        if(collision.contactCount > 1)
        {
            speed = rb.velocity.magnitude;
            direction = Vector3.Reflect(rb.velocity.normalized, collision.GetContact(1).normal);//collision.contacts[0].normal);
            rb.velocity = direction * Mathf.Max(speed, 0f);
        }
        */
    }

    void TestIfBallStuckOnAnyAxis()
    {
        if (LastVelocity.normalized.x < 0.05f && LastVelocity.normalized.x > -0.05f)
        {
            Halt();
            Invoke("Restart", 0.5f);
        }
        else if (LastVelocity.normalized.y < 0.05f && LastVelocity.normalized.y > -0.05f)
        {
            Halt();
            Invoke("Restart", 0.5f);
        }

    }

}
