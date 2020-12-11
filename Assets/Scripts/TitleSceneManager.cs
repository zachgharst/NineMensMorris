using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public Button onePlayerButton;
    public Button twoPlayerButton;
    public GameObject start;
    public GameObject inputField1;
    public GameObject inputField2;
    public static Text User1Text;
    public static Text User2Text;

    // Start is called before the first frame update
    void Start()
    {
        //User1Text = GameObject.Find("Username1Text").GetComponent<Text>();
        //User2Text = GameObject.Find("Username2Text").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void start1PGame()
    {
        ComputerOpponent.isActive = true;
        loadGame();
        //inputField1.SetActive(true);
        //inputField2.SetActive(false);
        //start.SetActive(true);
    }

    public void start2PGame()
    {
        ComputerOpponent.isActive = false;
        loadGame();
        //inputField1.SetActive(true);
        //inputField2.SetActive(true);
        //start.SetActive(true);
    }

    public void loadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void updateUsernames()
    {

    }
}
