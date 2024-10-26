using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyResizing : MonoBehaviour
{
    //public bool resizeFoeActive;
    public GameObject foeImageObject;
    public int foeBumpCounter;
    public Vector3 temp;
    public Vector3 original;
    public float changeSpeed;
    public bool movingDown;
    public Sprite foeGravestone;

    public Vector3 originalPosition;
    public bool movingForward;
    public int chargeCounter;
    public float foeWidth;
    public Vector3 temp2;


    // Start is called before the first frame update
    void Start()
    {
        //resizeFoeActive = false;
        original = foeImageObject.transform.localScale;
        movingDown = true;

        foeWidth = foeImageObject.GetComponent<RectTransform>().rect.width *
                foeImageObject.GetComponent<RectTransform>().localScale.x;
        //originalPosition = foeImageObject.transform.position;
        movingForward = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (foeBumpCounter > 0)
        {
            //Debug.Log("foeimage localscale.y is:" + foeImageObject.transform.localScale.y);

            if (movingDown == false)
            {
                temp = foeImageObject.transform.localScale;
                temp.y += changeSpeed * Time.deltaTime;
                foeImageObject.transform.localScale = temp;
            }

            if (movingDown == true)
            {
                temp = foeImageObject.transform.localScale;
                temp.y -= changeSpeed * Time.deltaTime;
                foeImageObject.transform.localScale = temp;
            }

            if (foeImageObject.transform.localScale.y <= 0.9)
            {
                movingDown = false;
            }
            if (foeImageObject.transform.localScale.y >= 1)
            {
                movingDown = true;
                foeBumpCounter -= 1;
            }
        }
        /*
        if(foeBumpCounter <= 0)
        {
            foeImageObject.transform.localScale = original;
            movingDown = true;
        }
        */
        if (chargeCounter > 0)
        {
            //Debug.Log("foeimage localscale.y is:" + foeImageObject.transform.localScale.y);

            if (movingForward == false)
            {
                temp2 = foeImageObject.transform.localPosition;
                temp2.x += 300f * Time.deltaTime;
                foeImageObject.transform.localPosition = temp2;
            }

            if (movingForward == true)
            {
                temp2 = foeImageObject.transform.localPosition;
                temp2.x -= 300f * Time.deltaTime;
                foeImageObject.transform.localPosition = temp2;
            }

            if (foeImageObject.transform.localPosition.x <= originalPosition.x - foeWidth / 2)
            {
                movingForward = false;
            }
            if (foeImageObject.transform.localPosition.x >= originalPosition.x)
            {
                movingForward = true;
                chargeCounter -= 1;
            }
        }
    }
    
    public void ActivateFoeBump(int numberOfBumps)
    {
        foeBumpCounter = numberOfBumps;
    }

    public void ActivateFoeAttack(int numberOfCharges)
    {
        chargeCounter = numberOfCharges;
    }

    //for v0.5.7.
    //used by battlefield foes
    //could reset position variable here too (position might change)
    public void SetSize()
    {
        original = foeImageObject.transform.localScale;
        originalPosition = foeImageObject.transform.localPosition;
        movingDown = true;
    }
}
