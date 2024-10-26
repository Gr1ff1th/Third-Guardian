using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipCanvas : MonoBehaviour
{
    public GameObject tooltipBackground;
    public bool hasExited;

    public void ShowTooltipWithDelay()
    {
        hasExited = false;

        tooltipBackground.SetActive(false);

        // wait for 1 second
        StartCoroutine(WaitForIt());
        
    }

    IEnumerator WaitForIt()
    {
        
        yield return new WaitForSeconds(0.5f);

        if (hasExited == false)
        {
            tooltipBackground.SetActive(true);
        }

    }

    public void CursorExit()
    {
        hasExited = true;
    }

}
