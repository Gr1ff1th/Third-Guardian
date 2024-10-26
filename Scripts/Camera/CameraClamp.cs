using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    [SerializeField]
    private Transform targetToFollow;

    public float xClamp1value;
    public float xClamp2value;
    public float yClamp1value;
    public float yClamp2value;

    public GameObject roomController;

    private void Start()
    {
        roomController = GameObject.Find("AMMRoomController");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Camera>().isActiveAndEnabled == true && roomController.GetComponent<PhotonRoom>().currentScene > 2)
        {
            if (GameManager.ins.avatars.Count > 0 && GameManager.ins.avatars[GameManager.ins.turnNumber].gameObject != null)
            {
                targetToFollow = GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponent<CharController>().gameObject.transform;

                gameObject.transform.position = new Vector3(Mathf.Clamp(targetToFollow.position.x, xClamp1value, xClamp2value),
                    Mathf.Clamp(targetToFollow.position.y, yClamp1value, yClamp2value),
                    transform.position.z);
            }
        }
    }
}
