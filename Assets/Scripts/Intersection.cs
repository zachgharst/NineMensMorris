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
    private int row;
    private int column;
    
    /* Special pointer constructor. */
    public static Intersection CreateComponent(GameObject location, int c, int r)
    {
        Intersection i = location.AddComponent<Intersection>();
        i.column = c;
        i.row = r;
        return i;
    }

    void OnMouseDown()
    {
        /* Get the cell equivalent for the opposite player. */
        Cell oppositePlayerCell = BoardManager.currentPlayer == Player.White ? Cell.Black : Cell.White;

        /* A mill has been formed and this click represents the removal of a piece. */
        if (BoardManager.millFormed == true)
        {
            /* If a mill has been formed, then the click must be on an opposing cell. */
            if(BoardManager.BoardState[row, column] == oppositePlayerCell)
            {
                BoardManager.Mill(gameObject, row, column);
            }
            else
            {
                print("Please click on an opposing piece");
            }
        }

        /* If black has unplayed pieces, still in phase 1. */
        else if (BoardManager.blackUnplacedPieces > 0)
        {
            if (BoardManager.BoardState[row, column] != Cell.Vacant)
                return;
            BoardManager.Phase1Action(gameObject, row, column);
        }

        /* If both players have played all their pieces, and their remaining pieces are below 3, phase 2. */
        else if (BoardManager.blackRemainingPieces > 3 && BoardManager.whiteRemainingPieces > 3)
        {
            BoardManager.Phase2();
        }

        /* If none of these conditions are fulfilled, must be phase 3. */
        else
        {
            BoardManager.Phase3();
        }
    }
}
