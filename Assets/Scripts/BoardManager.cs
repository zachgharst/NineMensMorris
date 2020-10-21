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

public enum Cell { Invalid, Vacant, White, Black };
public enum Player { White, Black };

public class BoardManager : MonoBehaviour
{
    public static Player currentPlayer = Player.White;
    public static bool millFormed = false;
    public static bool movingPiece = false;
    public static int[] pieceSelected = { -1, -1 };

    public static int whiteUnplacedPieces = 9;
    public static int whiteRemainingPieces = 9;

    public static int blackUnplacedPieces = 9;
    public static int blackRemainingPieces = 9;


    public Sprite man;

    public static Cell[,] BoardState = {
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        {  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
    };

    void InitGame()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == Cell.Vacant)
                    CreateIntersections(i, j);
            }
        }

    }

    void ResetBoard()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] != Cell.Invalid)
                    BoardState[i, j] = Cell.Vacant;
            }
        }
        foreach (Intersection i in Resources.FindObjectsOfTypeAll(typeof(Intersection)) as Intersection[])
        {
            i.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }

        currentPlayer = Player.White;
        millFormed = false;
        movingPiece = false;
        pieceSelected[0] = -1;
        pieceSelected[1] = -1;

        whiteUnplacedPieces = 9;
        whiteRemainingPieces = 9;

        blackUnplacedPieces = 9;
        blackRemainingPieces = 9;
    }

    private void CreateIntersections(int x, int y)
    {
        GameObject g = new GameObject((char)(x + 97) + "" + (y + 1));
        g.transform.position = new Vector2(x - 3, y - 3);
        g.transform.SetParent(this.transform);

        Intersection.CreateComponent(g, x, y);

        g.AddComponent<BoxCollider>();

        var s = g.AddComponent<SpriteRenderer>();
        s.sprite = man;
        s.color = new Color(0, 0, 0, 0);
    }

    void GameOver()
    {
        return;
    }

    void Start()
    {
        InitGame();
    }

    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            ResetBoard();
        }

        if (whiteRemainingPieces < 3 || blackRemainingPieces < 3)
            GameOver();
    }
}
