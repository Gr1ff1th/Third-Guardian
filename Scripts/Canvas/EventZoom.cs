using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventZoom : MonoBehaviour
{
    //public float changeSpeed;
    public bool isEnlarging;
    //public float hasMoved;
    public float startScale;

    private void Start()
    {
        isEnlarging = true;

        startScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        //lets use fixed floatseed for now
        //float finalFloatSpeed = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * floatSpeed * Time.deltaTime;
        float finalChangeSpeed = 0.04f * Time.deltaTime;
        //float moveRange = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * 0.0004f;

        if (gameObject.activeSelf)
        {
            if (isEnlarging == true)
            {
                transform.localScale = new Vector3(transform.localScale.x + finalChangeSpeed, transform.localScale.y + finalChangeSpeed, transform.position.z);

                //hasMoved = transform.position.y - GetComponentInParent<CharController>().internalNode.transform.position.y;

                //if (hasMoved > 0.25f)
                if (transform.localScale.x > (startScale * 1.4f))
                {
                    //hasMoved = 0;
                    isEnlarging = false;
                }
            }

            if (isEnlarging == false)
            {
                transform.localScale = new Vector3(transform.localScale.x - finalChangeSpeed, transform.localScale.y - finalChangeSpeed, transform.position.z);

                //hasMoved = GetComponentInParent<CharController>().internalNode.transform.position.y - transform.position.y;

                if (transform.localScale.x < (startScale * 1.02f))
                {
                    //hasMoved = 0;
                    isEnlarging = true;
                }
            }
        }
    }
}
