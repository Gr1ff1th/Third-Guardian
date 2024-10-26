using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ABCanvas : MonoBehaviour
{
    //new interaction orb locations (as of v88)
    //the actual buttons are found at references script
    public GameObject button1;
    public GameObject button1Frame;
    public GameObject button2;
    public GameObject button2Frame;
    public GameObject button3;
    public GameObject button3Frame;
    public GameObject button4;
    public GameObject button4Frame;
    public GameObject button5;
    public GameObject button5Frame;

    /*buttons
    public GameObject a;
    public GameObject b;
    public GameObject c;
    public GameObject d;
    public GameObject e;
    public GameObject f;
    public GameObject g;
    public GameObject h;
    public GameObject i;
    public GameObject j;
    public GameObject k;
    public GameObject l;
    public GameObject m;
    public GameObject n;
    public GameObject o;
    */

    //for imprisonment at grimhold
    public GameObject wait;
    public GameObject bail;

    public GameObject toolTipCanvas;
    //public GameObject toolTipText;

    //save interaction buttons
    //these are unnecessary since v88
    public GameObject interaction1Start;
    public GameObject interaction1Upg;
    public GameObject interaction2Start;
    public GameObject interaction2Upg;

    //public GameObject rightButton;
    //public GameObject leftButton;

    //other special buttons
    public GameObject restButton;
    //public GameObject researchButton;

    //menu canvas (not used anymore?)
    public GameObject menuCanvas;

    //variable to tell if location is upgraded or not
    public bool isUpgraded = false;

    public bool interactionDisabled = false;

    //for borders of the menu
    public float padding;

    public bool showFirstPage;

    //holder for the action menu elements
    public GameObject holder;

    //location interaction cost text
    public TextMeshProUGUI interactionCostText;

    void Awake()
    {
        /*sets the initial interaction(s), no more than 2 is allowed atm, and they must be the first two on the "list"
        if (interaction1Start != null && isUpgraded == false)
        {
            a = interaction1Start;
            a.SetActive(true);
        }

        if (interaction1Upg != null && isUpgraded == true)
        {
            a = interaction1Upg;
            a.SetActive(true);
        }
        
        if (interaction2Start != null && isUpgraded == false)
        {
            b = interaction2Start;
            b.SetActive(true);
        }

        if (interaction2Upg != null && isUpgraded == true)
        {
            b = interaction2Upg;
            b.SetActive(true);
        }
        */
        showFirstPage = true;

        //isUpgraded = false;

        //interactionDisabled = false;
    }

    public void UpgradeInteractions()
    {
        isUpgraded = true;

        /*sets the upgraded interaction(s)
        if (interaction1Upg != null)
        {
            interaction1Start.SetActive(false);
            a = interaction1Upg;
            a.SetActive(true);
        }

        if (interaction2Upg != null)
        {
            interaction2Start.SetActive(false);
            b = interaction2Upg;
            b.SetActive(true);
        }
        */
    }

    /* old code
    public void Activate()
    {
        // deactivates UI panel
        // makes adjacent nodes reachable
        if (holder.gameObject.activeSelf)
        {
            //deactivates ABcanvas
            //menuCanvas.gameObject.SetActive(false);
            holder.gameObject.SetActive(false);

            //removes tooltips when closing action menu
            toolTipCanvas.SetActive(false);

            //make imprisonment check here
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isImprisoned > 0)
            {
                //dont allow movementBonus, if imprisoned
                return;
            }

            //changes reachable colliders from the current node if player still have movementBonus points
            //need to change the avatar statement soon
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().canMove == true)
            {
                GameManager.ins.currentNode.SetReachableNodes(true);
            }

            //close "extra" buttons
            CloseExtraButtons();

            //should enable straight actions when menu is closed
            //GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().Cancel();
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = true;
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.ActivateButtons();
        }

        // in reverse
        // this could also be done in game manager?
        else
        {
            //deactivates overmap buttons
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<CharController>().standingOn.DeactivateButtons();

            showFirstPage = true;

            holder.gameObject.SetActive(true);

            //SpawnMenu();

            GameManager.ins.currentNode.SetReachableNodes(false);

            //should disable straight actions when menu is open
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().straightActionAllowed = false;

            //play sfx
            GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayOpenActionMenu();

            //make imprisonment check here
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isImprisoned > 0)
            {
                wait.gameObject.SetActive(true);
                bail.gameObject.SetActive(true);

                if (a != null)
                    a.gameObject.SetActive(false);
                if (b != null)
                    b.gameObject.SetActive(false);
                if (c != null)
                    c.gameObject.SetActive(false);
                if (d != null)
                    d.gameObject.SetActive(false);
                if (e != null)
                    e.gameObject.SetActive(false);
                if (f != null)
                    f.gameObject.SetActive(false);
                if (g != null)
                    g.gameObject.SetActive(false);
                if (h != null)
                    h.gameObject.SetActive(false);
                if (i != null)
                    i.gameObject.SetActive(false);
                if (j != null)
                    j.gameObject.SetActive(false);
                if (k != null)
                    k.gameObject.SetActive(false);
                if (l != null)
                    l.gameObject.SetActive(false);
                if (m != null)
                    m.gameObject.SetActive(false);
                if (n != null)
                    n.gameObject.SetActive(false);
                if (o != null)
                    o.gameObject.SetActive(false);

                //wait.transform.position = Input.mousePosition;
                //wait.transform.position += transform.TransformDirection(0, 0, 0);

                //bail.transform.position = Input.mousePosition;
                //bail.transform.position += transform.TransformDirection(0, -30, 0);

                //dont show scroll buttons
                //rightButton.gameObject.SetActive(false);
                //leftButton.gameObject.SetActive(false);

                //dont show interaction & reserach buttons
                //interaction1Start.gameObject.SetActive(false);
                //interaction1Upg.gameObject.SetActive(false);
                //researchButton.gameObject.SetActive(false);

                //ignore rest of method
                return;
            }

            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isImprisoned == 0)
            {
                if (wait != null)
                    wait.gameObject.SetActive(false);
                if (bail != null)
                    bail.gameObject.SetActive(false);
            }

            OpenFirstPage();

            

        }
    }

    public void OpenSecondPage()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //closes tooltip
        toolTipCanvas.SetActive(false);

        CloseExtraButtons();

        //interaction1Start.SetActive(false);
        //interaction1Upg.SetActive(false);
        //researchButton.SetActive(false);

        a.SetActive(true);
        b.SetActive(true);
        c.SetActive(true);
        d.SetActive(true);
        e.SetActive(true);
        f.SetActive(true);
        g.SetActive(true);
        h.SetActive(true);

        //set the page change buttons
        //rightButton.SetActive(false);
        //leftButton.SetActive(true);

        //CloseExtraButtons();

        //checks if player has foresight perk, and replaces 7th button with it
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ForesightTest() == true)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().foresightButton.transform.position = g.transform.position;
            g.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().foresightButton.SetActive(true);
        }

        //checks if is standing on smithy, and shows smithing option if so
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().nodeNumber == 1)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().smithingButton.transform.position = a.transform.position;
            a.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().smithingButton.SetActive(true);
        }

        //checks if is standing on temple, and shows scribing option if so
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().nodeNumber == 6)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().scribingButton.transform.position = a.transform.position;
            a.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().scribingButton.SetActive(true);
        }
    }

    public void OpenFirstPageWithButtonSound()
    {
        //play sfx
        GameObject.Find("SFX Player").GetComponent<SFXPlayer>().PlayButton1();

        //close "extra" buttons
        CloseExtraButtons();

        OpenFirstPage();
    }

    public void OpenFirstPage()
    {
        //set the correct interaction cost text
        if(gameObject.GetComponentInParent<Node>().interactCost == 0)
        {
            interactionCostText.text = "<size=16><color=#FFD370>Interaction cost: none";
        }
        if (gameObject.GetComponentInParent<Node>().interactCost == 1)
        {
            interactionCostText.text = "<size=16><color=#FFD370>Interaction cost: <sprite index=11>";
        }
        if (gameObject.GetComponentInParent<Node>().interactCost == 2)
        {
            interactionCostText.text = "<size=16><color=#FFD370>Interaction cost: <sprite index=11><sprite index=11>";
        }
        if (gameObject.GetComponentInParent<Node>().interactCost > 2 || interactionDisabled)
        {
            interactionCostText.text = "<size=16><color=#FFD370>Interaction unavailable";
        }

        //places the UI elements at mouse location
        //this is always explore button on v88
        if (a != null)
        {
            a.gameObject.SetActive(true);

            //a.transform.position = Input.mousePosition;
            //a.transform.position += transform.TransformDirection(0, 0, 0);
            
            if (interaction1Start != null && isUpgraded == false)
            {
                //a = interaction1Start;
                interaction1Start.transform.position = a.transform.position;
                a.SetActive(false);
                interaction1Start.SetActive(true);
                interaction1Upg.SetActive(false);

                if (interactionDisabled == true)
                {
                    interaction1Start.GetComponent<Button>().interactable = false;
                }
                else
                {
                    interaction1Start.GetComponent<Button>().interactable = true;
                }
            }

            if (interaction1Upg != null && isUpgraded == true)
            {
                interaction1Upg.transform.position = a.transform.position;
                a.SetActive(false);
                interaction1Upg.SetActive(true);
                interaction1Start.SetActive(false);

                if (interactionDisabled == true)
                {
                    interaction1Upg.GetComponent<Button>().interactable = false;
                }
                else
                {
                    interaction1Upg.GetComponent<Button>().interactable = true;
                }
            }
            

            GameManager.ins.references.exploreButton.transform.position = a.transform.position;
            a.gameObject.SetActive(false);
            GameManager.ins.references.exploreButton.SetActive(true);

            if (interactionDisabled == true)
            {
                GameManager.ins.references.exploreButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                GameManager.ins.references.exploreButton.GetComponent<Button>().interactable = true;
            }
        }

        //this is always rest button on v88
        if (b != null)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.transform.position = b.transform.position;
            b.gameObject.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.SetActive(true);
        }

         this is always research button on v82
         * not needed now
        if (c != null)
        {
            researchButton.transform.position = c.transform.position;
            c.gameObject.SetActive(false);
            researchButton.SetActive(true);

            if (interactionDisabled == true)
            {
                researchButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                researchButton.GetComponent<Button>().interactable = true;
            }
        }
        
        //this is always attempt quest button in v82
        if (c != null)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().attemptQuestButton.transform.position = c.transform.position;
            c.gameObject.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().attemptQuestButton.SetActive(true);

        }

        //4th button
        if (d != null)
        {
            //GameManager.ins.dialogCanvas.GetComponent<CanvasController>().useIntelligenceButton.transform.position = d.transform.position;
            d.gameObject.SetActive(true);
            //GameManager.ins.dialogCanvas.GetComponent<CanvasController>().useIntelligenceButton.SetActive(true);
        }

        //5th button
        if (e != null)
        {
            //GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.transform.position = e.transform.position;
            e.gameObject.SetActive(true);
            //GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.SetActive(true);

        }

        //check if theres 6th button
        if (f != null)
        {
            f.gameObject.SetActive(true);
        }

        //checks if theres 7th button
        if (g != null)
        {
            g.gameObject.SetActive(true);
        }

        //checks if theres 8th button
        if (h != null)
        {
            h.gameObject.SetActive(true);
        }

        //checks if theres 9th button
        if (i != null)
        {
            i.gameObject.SetActive(true);
        }

        //checks if theres 10th button
        if (j != null)
        {
            j.gameObject.SetActive(true);
        }

        //checks if theres 11th button
        if (k != null)
        {
            k.gameObject.SetActive(true);
        }

        //checks if theres 12th button
        if (l != null)
        {
            l.gameObject.SetActive(true);
        }

        //checks if theres 13th button
        if (m != null)
        {
            m.gameObject.SetActive(true);
        }

        //checks if theres 14th button
        if (n != null)
        {
            n.gameObject.SetActive(true);
            //k.transform.position = Input.mousePosition;
            //k.transform.position += transform.TransformDirection(0, -300, 0);
        }

        //15th button is always investigate at v88
        if (o != null)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().investigateButton.transform.position = o.transform.position;
            o.gameObject.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().investigateButton.SetActive(true);
        }

        //set the page change buttons
        //not needed anymore
        //rightButton.SetActive(false);
        //leftButton.SetActive(false);

        checks if player has foresight perk, and replaces 7th button with it
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ForesightTest() == true)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().foresightButton.transform.position = g.transform.position;
            g.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().foresightButton.SetActive(true);
        }
        
        //checks if is standing on smithy, and shows smithing option if so
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().nodeNumber == 1)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().smithingButton.transform.position = h.transform.position;
            h.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().smithingButton.SetActive(true);
        }

        //checks if is standing on temple, and shows scribing option if so
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.GetComponent<Node>().nodeNumber == 6)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().scribingButton.transform.position = h.transform.position;
            h.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().scribingButton.SetActive(true);
        }

        //checks if player has sentinel perk, and replaces rest button with it (during daytime)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().SentinelTest() == true)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().selfMaintenanceButton.transform.position = b.transform.position;
            b.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().selfMaintenanceButton.SetActive(true);
            return;
        }

        //checks if player has contemplation perk, and replaces rest button with it (during daytime)
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().ContemplationTest() == true && Clock.clock.isNight == false)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().contemplateButton.transform.position = b.transform.position;
            b.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().contemplateButton.SetActive(true);
        }

        //checks if its night, allow sleep option then (disable rest & contemplate button)
        if (Clock.clock.isNight == true)
        {
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().sleepButton.transform.position = b.transform.position;
            b.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().contemplateButton.SetActive(false);
            GameManager.ins.dialogCanvas.GetComponent<CanvasController>().sleepButton.SetActive(true);
        }
    }

    public void SpawnMenu()
    {
        //Vector3 newPos = Input.mousePosition + offset;
        Vector3 newPos = Input.mousePosition;

        float height = menuCanvas.GetComponent<RectTransform>().rect.height * menuCanvas.GetComponent<RectTransform>().lossyScale.y;

        newPos.y = newPos.y - height * 0.25f;

        //makes sure the card canvas cant spawn outside borders
        float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + menuCanvas.GetComponent<RectTransform>().rect.height * menuCanvas.GetComponent<Canvas>().scaleFactor / 2) - padding;
        if (topEdgeToScreenEdgeDistance < 0)
        {
            newPos.y += topEdgeToScreenEdgeDistance;
        }

        float bottomEdgeToScreenEdgeDistance = 0 - (newPos.y - menuCanvas.GetComponent<RectTransform>().rect.height * menuCanvas.GetComponent<Canvas>().scaleFactor / 2) + padding;
        if (bottomEdgeToScreenEdgeDistance > 0)
        {
            newPos.y += bottomEdgeToScreenEdgeDistance;
        }

        //makes sure the card canvas cant spawn outside borders
        float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + menuCanvas.GetComponent<RectTransform>().rect.width * menuCanvas.GetComponent<Canvas>().scaleFactor / 2) - padding;
        if (rightEdgeToScreenEdgeDistance < 0)
        {
            newPos.x += rightEdgeToScreenEdgeDistance;
        }

        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - menuCanvas.GetComponent<RectTransform>().rect.width * menuCanvas.GetComponent<Canvas>().scaleFactor / 2) + padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            newPos.x += leftEdgeToScreenEdgeDistance;
        }

        //changes the position of canvas
        menuCanvas.transform.position = newPos;
    }

    public void CloseExtraButtons()
    {
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().contemplateButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().sleepButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().selfMaintenanceButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().foresightButton.SetActive(false);

        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().seekQuestButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().researchButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().attemptQuestButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().useIntelligenceButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().restButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().investigateButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().smithingButton.SetActive(false);
        GameManager.ins.dialogCanvas.GetComponent<CanvasController>().scribingButton.SetActive(false);

        //new since v88
        GameManager.ins.references.exploreButton.SetActive(false);
    }

    //when leave button is pressed
    public void LeaveButton()
    {
        //Activate();
    }
    */

    //most stuff above this is irrelevant since v88
    //handles activating orbs on overmap (since v88)
    public void ActivateActionOrbs()
    {
        //area transition button, when return to overmap button is also available
        //for v94
        if (button1 != null)
        {
            if(button1.GetComponent<Action>() != null)
            {
                //lets make transition specific check here (could be done better)
                if(button1.GetComponent<Action>().sceneToLoad != 0)
                {
                    button1.SetActive(true);
                    button1Frame.SetActive(true);
                }
            }
        }

        //explore / area transition button
        //was general interact button
        if (button2 != null && PhotonRoom.room.mapType == 2)
        {
            //GameManager.ins.references.OvermapExploreButton.SetActive(true);
            button2.SetActive(true);
            button2Frame.SetActive(true);
        }

        //only avaiable when youre not exploring
        //chooses the right rest button
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == false)
        {
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == false)
            {
                if (Clock.clock.isNight == false)
                {
                    //meditation test
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(101) == false)
                    {
                        GameManager.ins.references.RestButton.transform.position = button3.transform.position;
                        GameManager.ins.references.RestButton.SetActive(true);

                        button3Frame.SetActive(true);
                    }
                    else
                    {
                        GameManager.ins.references.RestButton2.transform.position = button3.transform.position;
                        GameManager.ins.references.RestButton2.SetActive(true);

                        button3Frame.SetActive(true);
                    }
                }
                else
                {
                    //meditation test
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(101) == false)
                    {
                        GameManager.ins.references.SleepButton.transform.position = button3.transform.position;
                        GameManager.ins.references.SleepButton.SetActive(true);
                        button3Frame.SetActive(true);
                    }
                    else
                    {
                        GameManager.ins.references.SleepButton2.transform.position = button3.transform.position;
                        GameManager.ins.references.SleepButton2.SetActive(true);

                        button3Frame.SetActive(true);
                    }
                }
            }
        }

        //different case when exploring, or for sentinels
        if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().isExploring == true || 
            GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == true)
        {
            //sentinel test
            if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(12) == true)
            {
                GameManager.ins.references.SelfMaintenanceButton.transform.position = button3.transform.position;

                GameManager.ins.references.SelfMaintenanceButton.SetActive(true);
                button3Frame.SetActive(true);
            }

            //special case for bonfires
            //same options as in overmap
            else if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().standingOn.isBonfire == true)
            {
                if (Clock.clock.isNight == false)
                {
                    //meditation test
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(101) == false)
                    {
                        GameManager.ins.references.RestButton.transform.position = button3.transform.position;
                        GameManager.ins.references.RestButton.SetActive(true);

                        button3Frame.SetActive(true);
                    }
                    else
                    {
                        GameManager.ins.references.RestButton2.transform.position = button3.transform.position;
                        GameManager.ins.references.RestButton2.SetActive(true);

                        button3Frame.SetActive(true);
                    }
                }
                else
                {
                    //meditation test
                    if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(101) == false)
                    {
                        GameManager.ins.references.SleepButton.transform.position = button3.transform.position;
                        GameManager.ins.references.SleepButton.SetActive(true);
                        button3Frame.SetActive(true);
                    }
                    else
                    {
                        GameManager.ins.references.SleepButton2.transform.position = button3.transform.position;
                        GameManager.ins.references.SleepButton2.SetActive(true);

                        button3Frame.SetActive(true);
                    }
                }
            }
            else
            {
                //meditation test
                if (GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().HasPassiveTest(101) == false)
                {
                    GameManager.ins.references.WaitButton.transform.position = button3.transform.position;
                    GameManager.ins.references.WaitButton.SetActive(true);
                    button3Frame.SetActive(true);
                }
                else
                {
                    GameManager.ins.references.WaitButton2.transform.position = button3.transform.position;
                    GameManager.ins.references.WaitButton2.SetActive(true);
                    button3Frame.SetActive(true);
                }
            }
        }

        // interact button (for redoing interactions for v90)
        if (button4 != null)
        {
            if (gameObject.GetComponentInParent<Node>().CheckEncounters() == true)
            {
                //GameManager.ins.references.OvermapExploreButton.transform.position = button2.transform.position;

                //GameManager.ins.references.OvermapExploreButton.SetActive(true);
                button4.SetActive(true);
                button4Frame.SetActive(true);
            }
        }
        
    }

    //handles deactivating orbs on overmap
    public void DeactivateActionOrbs()
    {
        GameManager.ins.references.OvermapExploreButton.SetActive(false);
        GameManager.ins.references.RestButton.SetActive(false);
        GameManager.ins.references.SleepButton.SetActive(false);
        GameManager.ins.references.ContemplateButton.SetActive(false);
        GameManager.ins.references.SelfMaintenanceButton.SetActive(false);
        GameManager.ins.references.QuickMaintenanceButton.SetActive(false);
        GameManager.ins.references.WaitButton.SetActive(false);

        GameManager.ins.references.RestButton2.SetActive(false);
        GameManager.ins.references.SleepButton2.SetActive(false);
        GameManager.ins.references.WaitButton2.SetActive(false);

        //button1.SetActive(false);
        if (button1 != null)
        {
            button1.SetActive(false);
        }
        if (button2 != null)
        {
            button2.SetActive(false);
        }
        if (button3 != null)
        {
            button3.SetActive(false);
        }
        if (button4 != null)
        {
            button4.SetActive(false);
        }
        //button5.SetActive(false);

        button1Frame.SetActive(false);
        button2Frame.SetActive(false);
        button3Frame.SetActive(false);
        button4Frame.SetActive(false);
        button5Frame.SetActive(false);
    }
}
