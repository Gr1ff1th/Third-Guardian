using CharacterCreator2D.UI;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationaryParallax : MonoBehaviour
{
    public float length;
    public float height;
    public float depth;
    public float depth2;

    public float startpos;
    public float startposX;
    public float startposY;
    //public GameObject cam;
    public float parallaxEffect;

    //public float temp;
    //public float distance;
    public float movementEffect;

    public float positionZ;

    //1=up, 2=right, 3=down, 4=left
    public int parallaxDirection;

    public bool isScreenMode;
    public bool isCharacterMenu;

    // Start is called before the first frame update
    void Start()
    {
        //these arent very good names for these
        depth = transform.position.y;
        depth2 = transform.position.z;

        //note that y and z axis are inverted in the game view unfortunately
        if (parallaxDirection == 1)
        {
            
            if (isScreenMode == false)
            {
                //gameObject.GetComponent<RectTransform>().position.x;
                startpos = transform.position.z;
                height = gameObject.GetComponent<RectTransform>().rect.height;
            }
            if (isScreenMode == true)
            {
                startposX = transform.position.x;
                startposY = transform.position.y;
                //length = GetComponent<RectTransform>().lossyScale.x;
                //length = Screen.width;
                length = gameObject.GetComponent<RectTransform>().rect.width * gameObject.GetComponent<RectTransform>().lossyScale.x;
                height = gameObject.GetComponent<RectTransform>().rect.height * gameObject.GetComponent<RectTransform>().lossyScale.y;

                float randomLength = Random.Range(0, length);
                transform.position = new Vector3(startposX + randomLength, startposY, transform.position.z);
            }
        }
        if (parallaxDirection == 2)
        {
            startpos = transform.position.x;
            if (isScreenMode == false)
            {
                length = gameObject.GetComponent<RectTransform>().rect.width;
            }
            if (isScreenMode == true && isCharacterMenu == false)
            {
                //length = GetComponent<RectTransform>().lossyScale.x;
                //length = Screen.width;
                length = gameObject.GetComponent<RectTransform>().rect.width * gameObject.GetComponent<RectTransform>().lossyScale.x;
                height = Screen.height;

                float randomHeight = Random.Range(0, height);
                transform.position = new Vector3(startpos, depth - randomHeight, transform.position.z);
            }
            if (isScreenMode == true && isCharacterMenu == true)
            {
                //length = GetComponent<RectTransform>().lossyScale.x;
                //length = Screen.width;
                length = gameObject.GetComponent<RectTransform>().rect.width * gameObject.GetComponent<RectTransform>().lossyScale.x;
                //height = Screen.height;

                //float randomHeight = Random.Range(0, height);
                transform.position = new Vector3(startpos, transform.position.y, transform.position.z);
            }
        }
        if (parallaxDirection == 3)
        {
            startpos = transform.position.z;
            height = gameObject.GetComponent<RectTransform>().rect.height;
        }
        if (parallaxDirection == 4)
        {
            startpos = transform.position.x;
            length = gameObject.GetComponent<RectTransform>().rect.width;
        }
        
        //length = GetComponent<SpriteRenderer>().bounds.size.x;
        
        //Debug.Log("startpos + length + height is: " + startpos + " + " + length + " + " + height);

        movementEffect = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //temp = (gameObject.transform.position.x * (1 - parallaxEffect));
        //distance = (transform.position.x + parallaxEffect);

        //up movementBonus (on current game)
        if (parallaxDirection == 1 && isScreenMode == false)
        {
            movementEffect += parallaxEffect;

            transform.position = new Vector3(transform.position.x, transform.position.y, startpos - movementEffect);

            positionZ = transform.position.z;

            if (transform.position.z < startpos - height)
            {
                transform.position = new Vector3(transform.position.x, depth, startpos);
                movementEffect = 0;
            }
        }

        //up movementBonus (on screen mode)
        if (parallaxDirection == 1 && isScreenMode == true)
        {
            movementEffect += parallaxEffect;

            transform.position = new Vector3(transform.position.x, startposY + movementEffect, transform.position.z);

            //positionZ = transform.position.z;

            if (transform.position.y > startposY + height)
            {
                float randomLength = Random.Range(0, length);
                transform.position = new Vector3(startposX + randomLength, startposY, transform.position.z);
                movementEffect = 0;
            }
        }

        //right movementBonus 
        if (parallaxDirection == 2 && isScreenMode == false)
        {
            movementEffect += parallaxEffect;

            transform.position = new Vector3(startpos - movementEffect, transform.position.y, transform.position.z);

            if (transform.position.x < startpos - (length * 2))
            {
                transform.position = new Vector3(startpos, depth, transform.position.z);
                movementEffect = 0;
            }
        }

        //right movementBonus (or actually left :-)) 
        if (parallaxDirection == 2 && isScreenMode == true && isCharacterMenu == false)
        {
            movementEffect += parallaxEffect;

            transform.position = new Vector3(startpos - movementEffect, transform.position.y, transform.position.z);

            if (transform.position.x < startpos - (length * 2))
            {
                float randomHeight = Random.Range(0, height);
                transform.position = new Vector3(startpos, depth - randomHeight, transform.position.z);
                movementEffect = 0;
            }
        }

        //right movementBonus (or actually left :-)) 
        if (parallaxDirection == 2 && isScreenMode == true && isCharacterMenu == true)
        {
            movementEffect += parallaxEffect;

            transform.position = new Vector3(startpos - movementEffect, transform.position.y, transform.position.z);

            if (transform.position.x < startpos - (length * 2))
            {
                //float randomHeight = Random.Range(0, height);
                transform.position = new Vector3(startpos, transform.position.y, transform.position.z);
                movementEffect = 0;
            }
        }

        //down movementBonus (on current game)
        if (parallaxDirection == 3)
        {
            movementEffect += parallaxEffect;

            transform.position = new Vector3(transform.position.x, transform.position.y, startpos + movementEffect);

            if (transform.position.z < startpos - height)
            {
                //transform.position = new Vector3(transform.position.x, depth, startpos);
                movementEffect = 0;
            }
        }

        //left movementBonus 
        //uses depth variables to keep it in place
        if (parallaxDirection == 4)
        {
            movementEffect += parallaxEffect;

            //transform.position = new Vector3(startpos + movementEffect, transform.position.y, transform.position.z);
            transform.position = new Vector3(startpos + movementEffect, depth, depth2);

            if (transform.position.x > startpos + length)
            {
                //removed these for now
                //transform.position = new Vector3(startpos, depth, transform.position.z);
                movementEffect = 0;
            }
        }
        /*
        if (temp > startpos + length)
        {
            //Debug.Log("startpos and length are: " + startpos + " and " + length);

            startpos += length * 2;
            Debug.Log("startpos is: " + startpos);
        }

        else if (temp < startpos - length)
        {
            //Debug.Log("startpos and length are: " + startpos + " and " + length);

            startpos -= length * 2;
            Debug.Log("startpos is: " + startpos);
        }
        */
    }
}
