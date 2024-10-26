using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float length;
    public float startpos;
    public GameObject cam;
    public float parallaxEffect;

    public float temp;
    public float dist;

    public bool test1;

    // Start is called before the first frame update
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        //Debug.Log("startpos + length is: " + startpos + " + " + length);

        test1 = false;
    }

    // Update is called once per frame
    void Update()
    {
        temp = (cam.transform.position.x * (1 - parallaxEffect));
        dist = (cam.transform.position.x * parallaxEffect);

        if (test1 == false)
        {
            //Debug.Log("temp + dist is: " + temp + " + " + dist);

            test1 = true;
        }

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if(temp > startpos + length)
        {
            //Debug.Log("startpos and length are: " + startpos + " and " + length);

            startpos += length * 2;
            //Debug.Log("startpos is: " + startpos);
        }

        else if(temp < startpos - length)
        {
            //Debug.Log("startpos and length are: " + startpos + " and " + length);

            startpos -= length * 2;
            //Debug.Log("startpos is: " + startpos);
        }
    }
}
