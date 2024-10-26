using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StrategicEncounter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //keeps track on which location the npc is located
    public Node standingOn;

    //keeping track of the internal node where the npc is located
    public InternalNode internalNode;

    //when mouse over element
    [TextArea(5, 20)]
    public string infoText;

    [TextArea(5, 20)]
    public string unidentifiedText;

    // something to click on
    [HideInInspector]
    public Collider col;

    //stores the encounter tied to this object
    public Encounter2 encounter;

    //flag variable for checking if the encounter has been checked this turn or not
    //might need this?
    public bool isChecked;

    //might need boolean for this
    public bool isIdentified;

    public bool isVisibleAtStart;

    //special case for "encounters" which cant be interacted with
    public bool NonInteractable;

    //special case for "encounters" which cant be interacted with
    public bool hasStealth;

    //for removing foe temporarily from multi-combat
    //public bool isTemporarilyRemoved;

    //special case to remove buttons, which have alrdy been used while on the minimap instance
    public bool[] removeExhaustableButtons;

    //could use this to make sure same encounter cant reduce encounter count twice
    //public int timesToCheck;

    public GameObject identifiedImage;
    public GameObject unIdentifiedImage;

    //keeps track of number on original list
    public int originalListNumber;

    //doesnt reduce encounter count when resolved, if true (used by continue strategic encounters)
    public bool dontReduceEncounterCount;

    //combat stats should be stored here (so theyre individualized properly)
    public int currentEnergy;
    public int attack;
    public int arcanePower;

    //for v0.7.1.
    public int maxEnergy;
    public int maxAttack;
    public int maxArcanePower;

    public int defense;
    public int resistance;

    //for daze handling
    public bool isDazed;
    public bool isStunned;

    //for second wind encounters (which have different stats than original form)
    //in v0.7.1. this is only quiron
    public bool secondWindActivated;

    //for v99 (replaces location node checks for stores)
    //37=wilforge, 28=smithy, 31=inn, 44=factory,
    //47=temple, 16=coven, 11=guildhouse, 1=mike
    public int storeNumber;

    // Start is called before the first frame update
    void Start()
    {
        removeExhaustableButtons = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false};

        //col = GetComponent<Collider>();

        //test
        //EnableCollider();
        //DisableCollider();

        if(isVisibleAtStart == true)
        {
            if(isIdentified == true)
            {
                identifiedImage.SetActive(true);
                unIdentifiedImage.SetActive(false);
            }
            if (isIdentified == false)
            {
                identifiedImage.SetActive(false);
                unIdentifiedImage.SetActive(true);
            }
        }
    }

    //might need this?
    public void EnableCollider()
    {
        col.enabled = true;
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }

    public void EnableEncounter()
    {
        EnableCollider();
    }

    /*works for colliders of this class only
    void OnMouseOver()
    {
        GameManager.ins.toolTipBackground.SetActive(true);

        if (isIdentified == true)
        {
            GameManager.ins.toolTipText.text = infoText.ToString();
        }
        else
        {
            GameManager.ins.toolTipText.text = unidentifiedText.ToString();
        }
    }

    //works for colliders of this class only
    void OnMouseExit()
    {
        GameManager.ins.toolTipBackground.SetActive(false);
    }
    */

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.ins.toolTipBackground.SetActive(true);

        if (isIdentified == true)
        {
            GameManager.ins.toolTipText.text = infoText.ToString();
        }
        else
        {
            GameManager.ins.toolTipText.text = unidentifiedText.ToString();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.ins.toolTipBackground.SetActive(false);
    }
}
