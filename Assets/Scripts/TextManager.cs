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
    public static Text whiteText;
    private Text whiteEventText;
    public static Text blackText;
    private Text blackEventText;

    private string playersTurn;
    private string phase;
    public string player1 = "White Player";     // placeholder for username (if wanted)
    public string player2 = "Black Player";

    void Start()
    {
        statusText = GameObject.Find("StatusText").GetComponent<Text>();
        whiteText = GameObject.Find("WhiteText").GetComponent<Text>();
        whiteEventText = GameObject.Find("WhiteEventText").GetComponent<Text>();
        blackText = GameObject.Find("BlackText").GetComponent<Text>();
        blackEventText = GameObject.Find("BlackEventText").GetComponent<Text>();
        whiteText.text = player1;     
        blackText.text = player2;
        whiteEventText.text = BoardManager.whiteRemainingPieces.ToString() + " pieces remaining\n";
        blackEventText.text = BoardManager.blackRemainingPieces.ToString() + " pieces remaining\n";
    }

    void Update()
    {
        playersTurn = BoardManager.currentPlayer == Player.White ? player1 : player2;

        /* Phase 1 text.*/
        if (BoardManager.blackUnplacedPieces > 0)
        {
            phase = "Phase 1";
            updateStatusText("", BoardManager.currentPlayer);
        }

        /* Phase 2 text.*/
        else if (BoardManager.blackRemainingPieces > 3 && BoardManager.whiteRemainingPieces > 3)
        {
            phase = "Phase 2";
            updateStatusText("", BoardManager.currentPlayer);
        }

        /* Phase 3 text.*/
        else
        {
            phase = "Phase 3";
            updateStatusText("", BoardManager.currentPlayer);
        }

    }

    // Updates the Status text box.
    public void updateStatusText(string text, Player player)
    {
        if (!BoardManager.gameOver)
        {
            statusText.text = player + "'s turn";
        }
        else
        {
            statusText.text += text;    // (the current line works but 'statusText.text = text;' does not)
        }
    }
    public void updateEventText(string text, Player player)
    {
        if (player == Player.White)
        {
            whiteEventText.text = BoardManager.whiteRemainingPieces.ToString() + " pieces remaining\n";
            whiteEventText.text += text;
        }
        else
        {
            blackEventText.text = BoardManager.blackRemainingPieces.ToString() + " pieces remaining\n";
            blackEventText.text += text;
        }
    }
}
