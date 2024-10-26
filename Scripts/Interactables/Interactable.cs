using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//just in case
//[RequireComponent(typeof(Prop))]

    // make this abstract class?
public abstract class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
    }

    public virtual void Interact()
    {
        Debug.Log("Opening action options at " + name);
    }

    //not used atm
    public virtual void Interact(int b)
    {
       b = 0;
    }
}
