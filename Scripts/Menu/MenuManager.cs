using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { set; get; }

    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connectMenu;

    public GameObject serverPrefab;
    public GameObject clientPrefab;

    //stores players name after removing the name field menu
    public InputField nameInput;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMenu.SetActive(false);

        //keep this manager between scenes
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectButton()
    {
        mainMenu.SetActive(false);
        connectMenu.SetActive(true);
    }

    public void HostButton()
    {
        try
        {
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();

            //Same as for the client
            //might need to keep this as class field?
            Client c = Instantiate(clientPrefab).GetComponent<Client>();

            //gets hosts name
            c.clientName = nameInput.text;
            c.isHost = true;

            if (c.clientName == "")
                c.clientName = "Host";

            //localhost?
            c.ConnectToServer("127.0.0.1", 6321);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }

        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }

    public void ConnectToServerButton()
    {
        string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        
        //is this really my local ip address?
        if (hostAddress == "")
            hostAddress = "127.0.0.1";

        try
        {
            //might need to keep this as class field?
            Client c = Instantiate(clientPrefab).GetComponent<Client>();

            //gets clients name
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Client";

            c.ConnectToServer(hostAddress, 6321);

            //if the loop goes this far, client should be connected
            connectMenu.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void BackButton()
    {
        mainMenu.SetActive(true);
        connectMenu.SetActive(false);
        serverMenu.SetActive(false);

        //destroys serverobject on backbutton
        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);

        //destroys clientobject on backbutton
        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("CharacterSelectionScene");
    }

}
