using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ToolTipText : MonoBehaviour
{
    //background canvas for tooltips
    public GameObject popupCanvasObject;

    //for determining the location in relation to cursor
    public Vector3 offset;

    //for changing window size
    public RectTransform popupObject;

    //for keeping it inside the screen?
    public float padding;

    private Canvas popupCanvas;

    private void Awake()
    {
        popupCanvas = popupCanvasObject.GetComponent<Canvas>();
    }
    // Update is called once per frame
    void Update()
    {
        FollowCursor();
        //myCanvas.transform.position = Input.mousePosition + offset;
    }

    private void FollowCursor()
    {
        //doesnt work for canvas, needs to make gameobject
        if(!popupCanvasObject.activeSelf) { return; }

        Vector3 newPos = Input.mousePosition + offset;
        newPos.z = 0f;
        
        float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + popupObject.rect.width * popupCanvas.scaleFactor / 2) - padding;
        if (rightEdgeToScreenEdgeDistance < 0)
        {
            newPos.x += rightEdgeToScreenEdgeDistance;
        }
        
        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - popupObject.rect.width * popupCanvas.scaleFactor / 2) - padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            newPos.x += leftEdgeToScreenEdgeDistance;
        }

        //might need to reverse this
        /*
        float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + popupObject.rect.height * popupCanvas.scaleFactor) - padding;
        if (topEdgeToScreenEdgeDistance < 0)
        {
            newPos.y += topEdgeToScreenEdgeDistance;
        }
        */
        float bottomEdgeToScreenEdgeDistance = 0 - (newPos.y - popupObject.rect.height * popupCanvas.scaleFactor) - padding;
        if (bottomEdgeToScreenEdgeDistance > 0)
        {
            newPos.y += bottomEdgeToScreenEdgeDistance;
        }

        popupObject.transform.position = newPos;
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
    }
    
    //not used
    public void DisplayWindow()
    {
        //text.ToString();
        popupCanvasObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
    }

    //not used
    public void HideWindow()
    {
        popupCanvasObject.SetActive(false);
    }
    
}
