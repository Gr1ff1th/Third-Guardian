using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//could be used for special variables for quests etc
public class SpecialVariables : MonoBehaviour
{
    public int currentSceneIndex;

    //store quests taken (or solved) here
    //stored by number in deck
    public List<int> haveTakenQuest;

    //for when objective is completed, but reward not yet claimed
    public List<int> haveCompletedQuestObjective;

    //when quest is completed and returned
    public List<int> haveCompletedQuest;

    //checks if you have the quest alrdy
    public bool CheckIfQuestTaken(int questNumber)
    {
        for(int i = 0; i < haveTakenQuest.Count; i++)
        {
            if (haveTakenQuest[i] == questNumber)
            {
                return true;
            }
        }
        return false;
    }

    //could use this for quests where theres no item to return
    public bool CheckIfQuestObjectiveCompleted(int questNumber)
    {
        for (int i = 0; i < haveCompletedQuestObjective.Count; i++)
        {
            if (haveCompletedQuestObjective[i] == questNumber)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfQuestCompleted(int questNumber)
    {
        for (int i = 0; i < haveCompletedQuest.Count; i++)
        {
            if (haveCompletedQuest[i] == questNumber)
            {
                return true;
            }
        }
        return false;
    }

}
