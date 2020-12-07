/*
 *  UMKC CS 449: Nine Men's Morris implementation
 *  Copyright (C) 2020 Forgetful Wanderers
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  https://github.com/ZDGharst/UMKC_ForgetfulWanderers/
 */

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    private Text statusText;
    private Text whiteText;
    private Text whiteEventText;
    private Text blackText;
    private Text blackEventText;

    private string playersTurn;
    private string phase;
    private string player1 = "White Player";     // placeholder for username (if wanted)
    private string player2 = "Black Player";

    void Start()
    {
        statusText = GetComponent<Text>();
        whiteText = GameObject.Find("WhiteText").GetComponent<Text>();
        whiteEventText = GameObject.Find("WhiteEventText").GetComponent<Text>();
        blackText = GameObject.Find("BlackText").GetComponent<Text>();
        blackEventText = GameObject.Find("BlackEventText").GetComponent<Text>();
        whiteText.text = player1;     
        blackText.text = player2;
        whiteEventText.text = "";
        blackEventText.text = "";

        /*
        private Text whiteEventText;
        private Text blackEventText;
        whiteEventText = GameObject.Find("WhiteEventText").GetComponent<Text>();
        blackEventText = GameObject.Find("BlackEventText").GetComponent<Text>();
        */
    }

    void Update()
    {
        playersTurn = BoardManager.currentPlayer == Player.White ? player1 : player2;
        whiteEventText.text = BoardManager.whiteRemainingPieces.ToString() + " pieces remaining\n";
        blackEventText.text = BoardManager.blackRemainingPieces.ToString() + " pieces remaining\n";

        /* Phase 1 text.*/
        if (BoardManager.blackUnplacedPieces > 0)
        {
            phase = "Phase 1";
            updateStatusText(playersTurn);

            //statusText.text = BoardManager.blackUnplacedPieces.ToString() + " pieces remaining for black" +
            //    "Phase 1\n" +
            //    playersTurn + "'s turn\n" +
            //    BoardManager.whiteUnplacedPieces.ToString() + " pieces remaining for white";
        }

        /* Phase 2 text.*/
        else if (BoardManager.blackRemainingPieces > 3 && BoardManager.whiteRemainingPieces > 3)
        {
            phase = "Phase 2";
            updateStatusText(playersTurn);

            //statusText.text = BoardManager.blackRemainingPieces.ToString() + " pieces remaining for black" +
            //    "Phase 2\n" +
            //    playersTurn + "'s turn\n" +
            //    BoardManager.whiteRemainingPieces.ToString() + " pieces remaining for white";
        }

        /* Phase 3 text.*/
        else
        {
            phase = "Phase 3";
            updateStatusText(playersTurn);

            //statusText.text = BoardManager.blackRemainingPieces.ToString() + " pieces remaining for black" +
            //    "Phase 3\n" +
            //    playersTurn + "'s turn\n" +
            //    BoardManager.whiteRemainingPieces.ToString() + " pieces remaining for white";
        }

    }

    // Updates the Status text box.
    private void updateStatusText(string player)
    {
        statusText.text = player + "'s turn";
    }
    
    public void updateEventText(string text, Player player)
    {
        if (player == Player.White)
        {
            whiteEventText.text += text;
        }
        else
        {
            blackEventText.text += text;
        }
    }
}
