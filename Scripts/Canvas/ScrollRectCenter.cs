using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectCenter : MonoBehaviour
{
    //old tests
    //note that this should be the artifact canvas now
    //public GameObject content;
    //public GameObject scrollRect;
    //public ScrollRect scroll;

    // lets just use one variable to transfer as new fitmode
    public ContentSizeFitter.FitMode newMode;

    //for setting fitmode back to original state
    //actually lets not do this, at least not yet
    //public ContentSizeFitter.FitMode returnMode;

    public Vector3 startCoords;
    public float startHeight;

    public bool isFoeCardHolder;

    // Start is called before the first frame update
    void Start()
    {
        //ChangeSizeFitter();

        //dunno if we need this anymore?
        //GetStartCoords();
    }

    public void GetStartCoords()
    {
        /* lets try setting the coords of the parent to fix the weird location swap bug
         * 
        startCoords = gameObject.GetComponentInParent<ScrollRect>().gameObject.GetComponent<RectTransform>().position;
        startHeight = gameObject.GetComponentInParent<ScrollRect>().gameObject.GetComponent<RectTransform>().rect.height;
        gameObject.GetComponent<RectTransform>().position = startCoords;
        //gameObject.GetComponent<RectTransform>().Axis.Horizontal = startHeight;
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, startHeight);
        */
    }

    //lets try tinkering with the content size fitter script
    public void ChangeSizeFitter()
    {
        //dunno if the horizontalfit can be changed
        //content.GetComponent<ContentSizeFitter>().FitMode.unconstrained;
        //thisContent = ContentSizeFitter.FitMode.PreferredSize;
        //thisContent = ContentSizeFitter.FitMode.PreferredSize;// m_HorizontalFit = content.GetComponent<ContentSizeFitter>().FitMode.Unconstrained;

        //changes the horizontal fit mode
        gameObject.GetComponent<ContentSizeFitter>().horizontalFit = newMode;

        //set coordinates back to original
        //gameObject.GetComponent<RectTransform>().position = startCoords;
    }

    //lets try tinkering with the content size fitter script
    public void ChangeSizeFitterForEnemyCards()
    {
        int activeFoeCards = 0;

        for(int i = 0; i< GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.childCount; i++)
        {
            if(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.GetChild(i).gameObject.activeSelf == true)
            {
                activeFoeCards += 1;
            }
        }

        if(activeFoeCards < 6)//(GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().foeCardArea.transform.childCount < 6)
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 180f);
        }

        else
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        //changes the horizontal fit mode
        //gameObject.GetComponent<ContentSizeFitter>().horizontalFit = newMode;
    }

    //lets try tinkering with the content size fitter script
    public void ChangeSizeFitterForUsableCards()
    {
        if (GameManager.ins.handCardArea.transform.childCount < 9)
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 240f);
        }

        else
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    //lets try tinkering with the content size fitter script
    public void ChangeSizeFitterForAbilityCards()
    {
        if (GameManager.ins.artifactCardArea.transform.childCount < 10)
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 328f);
        }

        else
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    //lets try tinkering with the content size fitter script
    public void ChangeSizeFitterForEquipmentCards()
    {
        if (GameManager.ins.equipmentCardArea.transform.childCount < 10)
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            //gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 373f);
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 321f);
        }

        else
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    //lets try tinkering with the content size fitter script
    //this is onlycalled from levelupcard area (not the other two)
    public void ChangeSizeFitterForUpgradeCards()
    {
        if (GameManager.ins.levelupCardArea.transform.childCount < 10)
        {
            GameManager.ins.levelupCardArea.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            GameManager.ins.levelupCardArea.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 328f);
        }
        else
        {
            GameManager.ins.levelupCardArea.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        if (GameManager.ins.levelupCardArea2.transform.childCount < 10)
        {
            GameManager.ins.levelupCardArea2.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            GameManager.ins.levelupCardArea2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 328f);
        }
        else
        {
            GameManager.ins.levelupCardArea2.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        if (GameManager.ins.levelupCardArea3.transform.childCount < 10)
        {
            GameManager.ins.levelupCardArea3.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            GameManager.ins.levelupCardArea3.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 328f);
        }

        else
        {
            GameManager.ins.levelupCardArea3.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    //lets try tinkering with the content size fitter script
    public void ChangeSizeFitterForEffectCards()
    {
        if (GameManager.ins.effectCardArea.transform.childCount < 6)
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 155f);
        }

        else
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    //lets try tinkering with the content size fitter script
    public void ChangeSizeFitterForObjectiveCards()
    {
        if (GameManager.ins.objectiveCardArea.transform.childCount < 4)
        {
            //dunno why this shows the bar on unconstrained, while others do not?
            //gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 110f);
            //gameObject.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
        }

        else
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            //gameObject.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        }
    }

    public void ChangeSizeFitterForCombatCards()
    {
        if (GameManager.ins.combatCardArea.transform.childCount < 20)
        {
            //dunno why this shows the bar on unconstrained, while others do not?
            //gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 560f);
            //gameObject.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
        }

        else
        {
            gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            //gameObject.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        }
    }

    /*
     * for setting fitmode back to original state
    public void ReturnSizeFitter()
    {
        //changes the horizontal fit mode
        GetComponent<ContentSizeFitter>().horizontalFit = returnMode;
    }
    */

    /*test codes
     * 
     * 
    //lets try doing this ourselves
    public void SetCenteredContentPosition2()
    {
        //Vector3 newPos = content.position;

        //float width = scrollRect.rect.width; // * rectCanvas.GetComponent<RectTransform>().lossyScale.x;

        //newPos.x = newPos.x + width / 2;

        //transfers the size information from scrollrectposition to content
        RectTransform rt = scrollRect.GetComponent<RectTransform>();
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y);

        //content.GetComponent<RectTransform>().width = scrollRect.rect.width;
    }


    public void ShowInTabControl()
    {
        float normalizePosition = (float)transform.GetSiblingIndex() / (float)scroll.content.transform.childCount;
        scroll.verticalNormalizedPosition = 1 - normalizePosition;
    }

    private Vector3 GetCenteredContentPosition(RectTransform child, ScrollRect scrollRect)
    {
        Vector3[] viewportCorners = new Vector3[4];
        RectTransform viewport = scrollRect.viewport;
        viewport = viewport != null ? viewport : (RectTransform)scrollRect.transform;
        viewport.GetWorldCorners(viewportCorners);
        Vector3 centreWorldPos = ((viewportCorners[1] - viewportCorners[0]) / 2f) + viewportCorners[0];
        float h = centreWorldPos.y - child.position.y;
        Vector3 displacement = new Vector3(0, h, 0);
        Vector3[] contentCorners = new Vector3[4];
        scrollRect.content.GetWorldCorners(contentCorners);

        if (contentCorners[1].y + displacement.y < viewportCorners[1].y)
        {
            displacement.y = viewportCorners[1].y - contentCorners[1].y;
        }
        else if (contentCorners[0].y + displacement.y > viewportCorners[0].y)
        {
            displacement.y = viewportCorners[0].y - contentCorners[0].y;
        }
        return scrollRect.content.position + displacement;
    }

    
    //altered code
    private void SetCenteredContentPosition(RectTransform child, ScrollRect scrollRect)
    {
        Vector3[] viewportCorners = new Vector3[4];
        RectTransform viewport = scrollRect.viewport;
        viewport = viewport != null ? viewport : (RectTransform)scrollRect.transform;
        viewport.GetWorldCorners(viewportCorners);
        Vector3 centreWorldPos = ((viewportCorners[1] - viewportCorners[0]) / 2f) + viewportCorners[0];
        float h = centreWorldPos.y - child.position.y;
        Vector3 displacement = new Vector3(0, h, 0);
        Vector3[] contentCorners = new Vector3[4];
        scrollRect.content.GetWorldCorners(contentCorners);

        if (contentCorners[1].y + displacement.y < viewportCorners[1].y)
        {
            displacement.y = viewportCorners[1].y - contentCorners[1].y;
        }
        else if (contentCorners[0].y + displacement.y > viewportCorners[0].y)
        {
            displacement.y = viewportCorners[0].y - contentCorners[0].y;
        }
        //return scrollRect.content.position + displacement;

        scrollRect.content.position = scrollRect.content.position + displacement;
    }
    */


}
