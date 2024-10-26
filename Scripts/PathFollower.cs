using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//not used atm
public class PathFollower : MonoBehaviour
{
    Node [] PathNode;
    
    //the object to move along the path
    public GameObject player;

    //the speed of moving along the path
    public float moveSpeed;

    //default time
    float timer;

    //current node
    int currentNode;

    //node position holder
    static Vector3 currentPositionHolder;

    // Start is called before the first frame update
    void Start()
    {
        PathNode = GetComponentsInChildren<Node>();
        CheckNode();
                  
    }

    //function to check current node and move to it
    void CheckNode()
    {
        if (currentNode < PathNode.Length)
        {
            timer = 0;
            currentPositionHolder = PathNode[currentNode].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentNode);
        timer += Time.deltaTime * moveSpeed;

        if(player.transform.position != currentPositionHolder)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, currentPositionHolder, timer);
        }
        else if (currentNode < PathNode.Length)
        {
            currentNode++;
            CheckNode();
        }
    }
    
}
