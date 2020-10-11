using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Text textTest;

    void Start()
    {
        textTest = GetComponent<Text>();
    }

    void Update()
    {
        string playersTurn = BoardManager.currentPlayer == Player.White ? "White" : "Black";
        textTest.text = BoardManager.blackUnplacedPieces.ToString() + " pieces remaining for black" +
            "\n\n\n\n\n\n\n\n\n\n\n\nPhase 1\n" +
            playersTurn + "'s turn\n\n\n\n\n\n\n\n\n\n\n\n" +
            BoardManager.whiteUnplacedPieces.ToString() + " pieces remaining for white";
    }
}
