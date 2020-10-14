/*
	UMKC CS 449: Nine Men's Morris implementation
    Copyright (C) 2020 Forgetful Wanderers

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    https://github.com/ZDGharst/UMKC_ForgetfulWanderers/
*/

﻿using System.Collections;
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
