using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    public GameObject cam;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //should add the speed variable for each frame update?
        float newLocation = cam.transform.position.x + speed;
            //speed * Time.deltaTime;

        transform.position = new Vector3(newLocation, transform.position.y, transform.position.z);
    }
}
