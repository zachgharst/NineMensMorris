using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public Button onePlayerButton;
    public Button twoPlayerButton;

    public static ComputerOpponent comOpp;

    // Start is called before the first frame update
    void Start()
    {
        comOpp = GameObject.Find("ComputerOpponent").GetComponent<ComputerOpponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void start1PGame()
    {
        //comOpp.isActive = true;
        
        SceneManager.LoadScene(1);
    }

    public void start2PGame()
    {

    }
}
