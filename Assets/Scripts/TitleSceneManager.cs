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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void start1PGame()
    {
        ComputerOpponent.isActive = true;
        inputField1.SetActive(true);
        inputField2.SetActive(false);
        start.SetActive(true);
    }

    public void start2PGame()
    {
        ComputerOpponent.isActive = false;
        inputField1.SetActive(true);
        inputField2.SetActive(true);
        start.SetActive(true);
    }

    public void loadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void updateUsernames()
    {

    }
}
