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

public class Intersection : MonoBehaviour
{
    public int row;
    public int column;

    public static Intersection CreateComponent(GameObject location, int c, int r)
    {
        Intersection i = location.AddComponent<Intersection>();
        i.row = r;
        i.column = c;
        return i;
    }

    void OnMouseDown()
    {
        Cell currentPlayerCell = BoardManager.currentPlayer == Player.White ? Cell.White : Cell.Black;
        Cell oppositePlayerCell = BoardManager.currentPlayer == Player.White ? Cell.Black : Cell.White;

        if (BoardManager.millFormed == true)
        {
            if(BoardManager.BoardState[row, column] == oppositePlayerCell)
            {
                BoardManager.Mill();
            }
            else
            {
                print("Please click on an opposing piece");
            }
        }

        else if (BoardManager.blackUnplacedPieces > 0)
        {
            if (BoardManager.BoardState[row, column] != Cell.Vacant)
                return;
            BoardManager.Phase1Action(gameObject, row, column);
        }

        else if (BoardManager.blackRemainingPieces > 3 && BoardManager.whiteRemainingPieces > 3)
        {
            BoardManager.Phase2();
        }

        else
        {
            BoardManager.Phase3();
        }
    }
}
