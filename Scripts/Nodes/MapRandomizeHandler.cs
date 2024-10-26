using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRandomizeHandler : MonoBehaviour
{
    //index:
    //0= hidden grove / hexwood
    //1=first farmland
    //2=structure1 (h. faustus/coven) (in south mid faewood)
    //3=the other option (h. faustus/coven) (in north mid faewood)
    //4=return transit from h. faustus 
    //5=return transit from coven
    //6=oldmines or brevirs pass
    //7=forest fort or burial mound
    //8=structure at mountains south mid
    //9=structure at wilforge outskirts
    //10=smithy return location
    //11=factory return location
    //12=firstborn fort or moltenrock cavern

    public int[] randomTransits;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //called at start of game
    public void RandomizeTransits()
    {
        randomTransits = new int[50];

        //0 is grove, 1 is hexwood
        int faewood1 = Random.Range(0, 2);
        randomTransits[0] = faewood1;

        //for testing
        int farmland1 = Random.Range(0, 2);
        randomTransits[1] = farmland1;

        //0 is house of faustus, 1 is coven
        int structure1 = Random.Range(0, 2);
        randomTransits[2] = structure1;

        //the two structures can swap places
        if(structure1 == 0)
        {
            randomTransits[3] = 1;
            //return transits
            //index 4 is h. faustus, 5 is coven
            //value 0 is south faewood, 1 is north faewood
            randomTransits[4] = 0;
            randomTransits[5] = 1;
        }
        if (structure1 == 1)
        {
            randomTransits[3] = 0;
            randomTransits[4] = 1;
            randomTransits[5] = 0;
        }

        //0 is oldmines, 1 is brevirs pass
        int passage1 = Random.Range(0, 2);
        randomTransits[6] = passage1;

        //0 is forest fort, 1 is burial mound
        int structure2 = Random.Range(0, 2);
        randomTransits[7] = structure2;

        //0 is house of smithy, 1 is factory
        int structure3 = Random.Range(0, 2);
        randomTransits[8] = structure3;

        //the two structures can swap places
        if (structure3 == 0)
        {
            randomTransits[9] = 1;
            //return transits
            //index 10 is smithy, 11 is factory
            //value 0 is mountain south mid, 1 outskirts
            randomTransits[10] = 0;
            randomTransits[11] = 1;
        }
        if (structure3 == 1)
        {
            randomTransits[9] = 0;
            randomTransits[10] = 1;
            randomTransits[11] = 0;
        }

        //0 is firstborn fort, 1 is moltenrock cavern
        int structure4 = Random.Range(0, 2);
        randomTransits[12] = structure4;
    }
}
