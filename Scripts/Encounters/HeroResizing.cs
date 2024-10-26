using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroResizing : MonoBehaviour
{
    public GameObject heroImageObject;
    public int bumpCounter;
    public Vector3 temp;
    public Vector3 original;
    public float changeSpeed;
    public bool movingDown;

    public Vector3 originalPosition;
    public bool movingForward;
    public int chargeCounter;
    public float heroWidth;
    public Vector3 temp2;

    // Start is called before the first frame update
    void Start()
    {
        heroImageObject = this.gameObject;
        original = heroImageObject.transform.localScale;
        movingDown = true;

        heroWidth = heroImageObject.GetComponent<RectTransform>().rect.width *
                heroImageObject.GetComponent<RectTransform>().lossyScale.x;
        originalPosition = heroImageObject.transform.position;
        movingForward = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (bumpCounter > 0)
        {
            //Debug.Log("foeimage localscale.y is:" + foeImageObject.transform.localScale.y);

            if (movingDown == false)
            {
                temp = heroImageObject.transform.localScale;
                temp.y += changeSpeed * Time.deltaTime;
                heroImageObject.transform.localScale = temp;
            }

            if (movingDown == true)
            {
                temp = heroImageObject.transform.localScale;
                temp.y -= changeSpeed * Time.deltaTime;
                heroImageObject.transform.localScale = temp;
            }

            if (heroImageObject.transform.localScale.y <= 0.9)
            {
                movingDown = false;
            }
            if (heroImageObject.transform.localScale.y >= 1)
            {
                movingDown = true;
                bumpCounter -= 1;
            }
        }
        /*
        if (bumpCounter <= 0)
        {
            heroImageObject.transform.localScale = original;
            movingDown = true;
        }
        */
        if (chargeCounter > 0)
        {
            //Debug.Log("foeimage localscale.y is:" + foeImageObject.transform.localScale.y);

            if (movingForward == false)
            {
                temp2 = heroImageObject.transform.position;
                temp2.x -= 10f * Time.deltaTime;
                heroImageObject.transform.position = temp2;
            }

            if (movingForward == true)
            {
                temp2 = heroImageObject.transform.position;
                temp2.x += 10f * Time.deltaTime;
                heroImageObject.transform.position = temp2;
            }

            if (heroImageObject.transform.position.x >= originalPosition.x + heroWidth/2)
            {
                movingForward = false;
            }
            if (heroImageObject.transform.position.x <= originalPosition.x)
            {
                movingForward = true;
                chargeCounter -= 1;
            }
        }
        /*
        if (chargeCounter <= 0)
        {
            heroImageObject.transform.position = originalPosition;
            movingForward = true;
        }
        */
    }

    public void ActivateHeroBump(int numberOfBumps)
    {
        bumpCounter = numberOfBumps;
    }

    public void ActivateHeroAttack(int numberOfCharges)
    {
        chargeCounter = numberOfCharges;
    }
}
