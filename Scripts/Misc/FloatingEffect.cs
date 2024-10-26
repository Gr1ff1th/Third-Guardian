using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    public float floatSpeed;
    public bool isMovingUp;
    public float hasMoved;
    public bool isFoeCombatImage;
    public bool isHeroDisplayImage;

    //leave at 0, if not battlefield foe
    public int isBattlefieldFoeNumber;

    private void Start()
    {
        isMovingUp = true;

        floatSpeed = Random.Range(0.00045f, 0.00055f);
    }

    // Update is called once per frame
    void Update()
    {
        //lets use fixed floatseed for now
        //float finalFloatSpeed = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * floatSpeed * Time.deltaTime;
        float finalFloatSpeed = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * 0.0005f * Time.deltaTime;
        float moveRange = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * 0.0004f;

        //for heroes
        if (GetComponentInParent<CharController>() != null)
        {
            if (isMovingUp == true && GetComponentInParent<CharController>().internalNode != null)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + finalFloatSpeed, transform.position.z);

                hasMoved = transform.position.y - GetComponentInParent<CharController>().internalNode.transform.position.y;

                //if (hasMoved > 0.25f)
                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = false;
                }
            }

            if (isMovingUp == false && GetComponentInParent<CharController>().internalNode != null)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - finalFloatSpeed, transform.position.z);

                hasMoved = GetComponentInParent<CharController>().internalNode.transform.position.y - transform.position.y;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = true;
                }
            }
        }

        //for strategic encounters
        //dunno why we need to separate these tho
        if (GetComponent<StrategicEncounter>() != null)
        {
            //lets test this for overmap encounters
            finalFloatSpeed = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * floatSpeed * Time.deltaTime;

            /* alternate system (kinda too funky tho)
            if (isMovingUp == true)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + finalFloatSpeed);

                hasMoved = transform.position.z - GetComponent<StrategicEncounter>().internalNode.transform.position.z;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = false;
                }
            }

            if (isMovingUp == false)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - finalFloatSpeed);

                hasMoved = GetComponent<StrategicEncounter>().internalNode.transform.position.z - transform.position.y;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = true;
                }
            }
            */
            if (isMovingUp == true)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + finalFloatSpeed, transform.position.z);

                hasMoved = transform.position.y - GetComponent<StrategicEncounter>().internalNode.transform.position.y;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = false;
                }
            }

            if (isMovingUp == false)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - finalFloatSpeed, transform.position.z);

                hasMoved = GetComponent<StrategicEncounter>().internalNode.transform.position.y - transform.position.y;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = true;
                }
            }
        }

        //for foe images (battlefield & encounter display)
        //need different values for foes
        if (GameManager.ins.exploreHandler.GetComponent<CombatHandler>().opponentDefeated == false) //GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.activeSelf &&
        {
            //kinda complicated way of checking if either encounter foe or battlefield foe is dead
            if ((isFoeCombatImage == true && isBattlefieldFoeNumber == 0) || (isFoeCombatImage == true && 
                GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[isBattlefieldFoeNumber - 1].foeDefeated == false))
            {
                finalFloatSpeed = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * 0.0002f * Time.deltaTime;
                moveRange = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * 0.0002f;

                if (isMovingUp == true)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + finalFloatSpeed);

                    hasMoved = transform.position.z - GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().noTimingFoeTarget.transform.position.z;

                    if (hasMoved > moveRange)
                    {
                        hasMoved = 0;
                        isMovingUp = false;
                    }
                }

                if (isMovingUp == false)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - finalFloatSpeed);

                    hasMoved = GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().noTimingFoeTarget.transform.position.z - transform.position.z;

                    if (hasMoved > moveRange)
                    {
                        hasMoved = 0;
                        isMovingUp = true;
                    }
                }
            }
        }

        //for hero "movement"
        if (isHeroDisplayImage == true && GameManager.ins.exploreHandler.GetComponent<CombatHandler>().heroKnockedOut == false)//GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterDisplay.activeSelf &&
        {
            finalFloatSpeed = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * 0.0002f * Time.deltaTime;
            moveRange = GameManager.ins.dialogCanvas.GetComponent<CanvasController>().screenHeight * 0.0002f;

            if (isMovingUp == true)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + finalFloatSpeed);

                hasMoved = transform.position.z - GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().noTimingHeroTarget.transform.position.z;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = false;
                }
            }

            if (isMovingUp == false)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - finalFloatSpeed);

                hasMoved = GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().noTimingHeroTarget.transform.position.z - transform.position.z;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = true;
                }
            }

            /*
            if (isMovingUp == true)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + finalFloatSpeed, transform.position.z);

                hasMoved = transform.position.y - GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().noTimingHeroTarget.transform.position.y;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = false;
                }
            }

            if (isMovingUp == false)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - finalFloatSpeed, transform.position.z);

                hasMoved = GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().noTimingHeroTarget.transform.position.y - transform.position.y;

                if (hasMoved > moveRange)
                {
                    hasMoved = 0;
                    isMovingUp = true;
                }
            }
            */
        }

    }
}
