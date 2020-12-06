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
    public Text textTest;
    public Text whiteText;
    public Text blackText;

    void Start()
    {
        textTest = GetComponent<Text>();
    }

    void Update()
    {
        /* Phase 1 text.*/
        if (BoardManager.blackUnplacedPieces > 0)
        {
            string playersTurn = BoardManager.currentPlayer == Player.White ? "White" : "Black";
            textTest.text = BoardManager.blackUnplacedPieces.ToString() + " pieces remaining for black" +
                "\n\n\n\n\n\n\n\n\n\n\n\nPhase 1\n" +
                playersTurn + "'s turn\n\n\n\n\n\n\n\n\n\n\n\n" +
                BoardManager.whiteUnplacedPieces.ToString() + " pieces remaining for white";
        }

        /* Phase 2 text.*/
        else if (BoardManager.blackRemainingPieces > 3 && BoardManager.whiteRemainingPieces > 3)
        {
            string playersTurn = BoardManager.currentPlayer == Player.White ? "White" : "Black";
            textTest.text = BoardManager.blackRemainingPieces.ToString() + " pieces remaining for black" +
                "\n\n\n\n\n\n\n\n\n\n\n\nPhase 2\n" +
                playersTurn + "'s turn\n\n\n\n\n\n\n\n\n\n\n\n" +
                BoardManager.whiteRemainingPieces.ToString() + " pieces remaining for white";
        }

        /* Phase 3 text.*/
        else
        {
            string playersTurn = BoardManager.currentPlayer == Player.White ? "White" : "Black";
            textTest.text = BoardManager.blackRemainingPieces.ToString() + " pieces remaining for black" +
                "\n\n\n\n\n\n\n\n\n\n\n\nPhase 3\n" +
                playersTurn + "'s turn\n\n\n\n\n\n\n\n\n\n\n\n" +
                BoardManager.whiteRemainingPieces.ToString() + " pieces remaining for white";
        }

    }
}
