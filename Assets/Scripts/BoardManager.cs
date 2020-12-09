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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Random = UnityEngine.Random;

public enum Cell { Invalid, Vacant, White, Black };
public enum Player { White, Black };

public class BoardManager : MonoBehaviour
{
    public static int turn = 0;

    public static Player currentPlayer = Player.White;

    public static bool millFormed = false;
    public static bool movingPiece = false;

    public static int whiteUnplacedPieces = 9;
    public static int whiteRemainingPieces = 9;
    public static bool isWhitePhase3 = false;

    public static int blackUnplacedPieces = 9;
    public static int blackRemainingPieces = 9;
    public static bool isBlackPhase3 = false;

    public static int tempRow;
    public static int tempCol;
    public static bool gameOver = false;

    public Sprite man;
    public Sprite manSelected;
    public static GameObject lastSelected;

    private Text whiteEventText;
    private Text blackEventText;
    public static TextManager tManager;

    public static Button mainMenuButton;
    public static Button restartGameButton;


    public static Cell[,] BoardState = {
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        {  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant },
        { Cell.Invalid, Cell.Invalid,  Cell.Vacant,  Cell.Vacant,  Cell.Vacant, Cell.Invalid, Cell.Invalid },
        { Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid,  Cell.Vacant, Cell.Invalid },
        {  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant, Cell.Invalid, Cell.Invalid,  Cell.Vacant },
    };

    public void Start()        // function called when we the scene is first loaded
    {
        whiteEventText = GameObject.Find("WhiteEventText").GetComponent<Text>();
        blackEventText = GameObject.Find("BlackEventText").GetComponent<Text>();
        tManager = GameObject.Find("StatusText").GetComponent<TextManager>();


        InitGame();
    }

    public static void InitGame()
    {
        GameObject g = GameObject.Find("BoardManager");
        BoardManager b = g.GetComponent<BoardManager>();
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == Cell.Vacant)
                {
                    b.CreateIntersections(i, j);
                }
            }
        }

        ResetBoard();
    }

    /* Create intersections at game start. */
    public void CreateIntersections(int x, int y)
    {
        /* Create a blank object with the name and position corresponding to the location of the intersection. */
        GameObject g = new GameObject((char)(x + 97) + "" + (y + 1));
        g.transform.position = new Vector2(x - 3, y - 3);
        g.transform.SetParent(this.transform);

        /* Speciality constructor. */
        Intersection.CreateComponent(g, x, y);

        /* Make intersection clickable. */
        g.AddComponent<BoxCollider>();

        /* Add sprite details to intersection. */
        var s = g.AddComponent<SpriteRenderer>();
        s.sprite = man;
        s.color = new Color(0, 0, 0, 0);
    }

    public static GameObject FindIntersection(string str)
    {
        return GameObject.Find(str);
    }

    public static string CreateIntersectionName(int row, int col)
    {
        return (char)(col + 97) + "" + (row + 1);
    }

    /* Reset the game back to a fresh start. */
    public static void ResetBoard()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] != Cell.Invalid)
                {
                    BoardState[i, j] = Cell.Vacant;
                }
            }
        }
        foreach (Intersection i in Resources.FindObjectsOfTypeAll(typeof(Intersection)) as Intersection[])
        {
            i.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        //    i.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
        //    i.GetComponent<SpriteRenderer>().size = new Vector2(3.5f, 1.5f);
        }

        turn = 0;
        currentPlayer = Player.White;

        millFormed = false;
        movingPiece = false;

        whiteUnplacedPieces = 9;
        whiteRemainingPieces = 9;
        isWhitePhase3 = false;

        blackUnplacedPieces = 9;
        blackRemainingPieces = 9;
        isBlackPhase3 = false;

        gameOver = false;

        ComputerOpponent.Reset();
    }

    public static Player GetOppositePlayer()
    {
        //tManager.updateEventText("", currentPlayer);
        if (currentPlayer == Player.White)
        {
            return Player.Black;
        }
        return Player.White;
    }

    public static bool CheckMill(Player p, int row, int column)
    {
        Cell cellCast = p == Player.White ? Cell.White : Cell.Black;
        bool intersectionPartOfMill = false;

        /* If we're in row 4, we have six spaces to check instead of three; special case. */
        if (row == 3)
        {
            if(column < 3)
            {
                if(BoardState[3, 0] == cellCast && BoardState[3, 1] == cellCast && BoardState[3, 2] == cellCast)
                {
                    intersectionPartOfMill = true;
                }
            }
            else
            {
                if (BoardState[3, 4] == cellCast && BoardState[3, 5] == cellCast && BoardState[3, 6] == cellCast)
                {
                    intersectionPartOfMill = true;
                }
            }
        }

        /* The general case for rows. */
        else
        {
            /* Check row for a mill. */
            int piecesInLine = 0;
            for (int i = 0; i < 7; i++)
            {
                if (BoardState[row, i] == cellCast)
                {
                    piecesInLine++;
                }
            }
            if (piecesInLine == 3)
            {
                intersectionPartOfMill = true;
            }
        }

        /* If we're in column d, we have six spaces to check instead of three; special case. */
        if (column == 3)
        {
            if (row < 3)
            {
                if (BoardState[0, 3] == cellCast && BoardState[1, 3] == cellCast && BoardState[2, 3] == cellCast)
                {
                    intersectionPartOfMill = true;
                }
            }
            else
            {
                if (BoardState[4, 3] == cellCast && BoardState[5, 3] == cellCast && BoardState[6, 3] == cellCast)
                {
                    intersectionPartOfMill = true;
                }
            }
        }

        /* The general case for columns. */
        else
        {
            /* Check row for a mill. */
            int piecesInLine = 0;
            for (int i = 0; i < 7; i++)
            {
                if (BoardState[i, column] == cellCast)
                {
                    piecesInLine++;
                }
            }
            if (piecesInLine == 3)
            {
                intersectionPartOfMill = true;
            }
        }

        return intersectionPartOfMill;
    }

    /* A man that is part of a mill can only be removed if all pieces of that player are part of a mill. This method checks to see if all pieces that a player owns are part of a mill. */
    public static bool AllMenInMill()
    {
        Player playerToCheck = GetOppositePlayer();
        Cell oppositePlayerCell = BoardManager.currentPlayer == Player.White ? Cell.Black : Cell.White;

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (BoardState[i, j] == oppositePlayerCell && !CheckMill(playerToCheck, i, j))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /* Returns true if there is at least one neighbor that is vacant to the input intersection. */
    public static bool HasAvailableVacantNeighbor(int row, int col)
    {
        /* Check the neighbor to the left. */
        for(int i = col - 1; i > -1; i--)
        {
            if (row == 3 && i == 3)
            {
                break;
            }
            if (BoardState[row, i] != Cell.Invalid)
            {
                if (BoardState[row, i] == Cell.Vacant)
                {
                    return true;
                }
                break;
            }
        }

        /* Check the neighbor to the right. */
        for(int i = col + 1; i < 7; i++)
        {
            if (row == 3 && i == 3)
            {
                break;
            }
            if (BoardState[row, i] != Cell.Invalid)
            {
                if (BoardState[row, i] == Cell.Vacant)
                {
                    return true;
                }
                break;
            }
        }

        /* Check the neighbor above. */
        for(int i = row + 1; i < 7; i++)
        {
            if (i == 3 && col == 3)
            {
                break;
            }
            if (BoardState[i, col] != Cell.Invalid)
            {
                if (BoardState[i, col] == Cell.Vacant)
                {
                    return true;
                }
                break;
            }
        }

        /* Check the neighbor below. */
        for(int i = row - 1; i > -1; i--)
        {
            if (i == 3 && col == 3)
            {
                break;
            }
            if (BoardState[i, col] != Cell.Invalid)
            {
                if (BoardState[i, col] == Cell.Vacant)
                {
                    return true;
                }
                break;
            }
        }

        return false;
    }

    /* Checks if a player has any moves available. */
    public static bool CheckAvailableMove(Player p)
    {
        Cell playerCell = p == Player.White ? Cell.White : Cell.Black;
        if( (blackUnplacedPieces > 0) ||
            (p == Player.White && isWhitePhase3) ||
            (p == Player.Black && isBlackPhase3) )
        {
            return true;

        }

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if(BoardState[i, j] == playerCell)
                {
                    if(HasAvailableVacantNeighbor(i, j))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /* Phase 1: Player is adding a piece to the board. */
    public static void Phase1Placement(GameObject g, int row, int column)
    {
        var s = g.GetComponent<SpriteRenderer>();

        if (currentPlayer == Player.White)
        {
            BoardState[row, column] = Cell.White;
            s.color = new Color(1, 1, 1, 1);
            whiteUnplacedPieces--;
        }

        else
        {
            BoardState[row, column] = Cell.Black;
            s.color = new Color(0.3f, 0.3f, 0.3f, 1);
            blackUnplacedPieces--;
        }

        millFormed = CheckMill(currentPlayer, row, column);
        if (millFormed != true)
        {
            currentPlayer = GetOppositePlayer();
            turn++;
        }

        /* During the transition to phase 2, check if white has any available moves going
         * into their turn. If they don't, black wins. */
        if (blackUnplacedPieces == 0)
        {
            if(!CheckAvailableMove(Player.White))
            {
                GameOver(Player.Black);
            }
        }
    }

    /* Phase 2/3: Player is selecting a piece to move. */
    public static void PieceSelection(GameObject g, int row, int column)
    {
        GameObject go2 = GameObject.Find("BoardManager");
        BoardManager b = go2.GetComponent<BoardManager>();
        var s = g.GetComponent<SpriteRenderer>();

        lastSelected = g;
        tempRow = row;
        tempCol = column;

        if(BoardState[row, column] == Cell.Black || BoardState[row, column] == Cell.White)
        {
            s.sprite = b.manSelected;
            movingPiece = true;
        }

        else
        {
            tManager.updateEventText("Invalid spot\nPlease try again", currentPlayer);
        }
    }

    /* Phase 2/3: Player is moving a piece they have already selected. */
    public static void PieceMovement(GameObject g, int row, int column)
    {
        GameObject go2 = GameObject.Find("BoardManager");
        BoardManager b = go2.GetComponent<BoardManager>();
        var s = g.GetComponent<SpriteRenderer>();

        if (currentPlayer == Player.White)
        {

            if (CheckSamePosition(row, tempRow, column, tempCol))
            {
                BoardState[row, column] = Cell.White;
                movingPiece = false;

                s.color = new Color(1, 1, 1, 1);
                s.sprite = b.man;
            }

            else if (isWhitePhase3 || isAdjacent(tempRow, tempCol, row, column))
            {
                BoardState[tempRow, tempCol] = Cell.Vacant;
                BoardState[row, column] = Cell.White;
                movingPiece = false;

                s.color = new Color(1, 1, 1, 1);
                lastSelected.GetComponent<SpriteRenderer>().sprite = b.man;
                lastSelected.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

                millFormed = CheckMill(currentPlayer, row, column);
                if (millFormed != true)
                {
                    currentPlayer = GetOppositePlayer();
                    turn++;
                }
            }

            else
            {
                tManager.updateEventText("Invalid spot\nPlease try again", currentPlayer);
            }

            /* After moving a piece, check if the opposing player has been locked in. */
            if (!CheckAvailableMove(Player.Black))
            {
                GameOver(Player.White);
            }
        }

        else
        {
            if (CheckSamePosition(row, tempRow, column, tempCol))
            {
                BoardState[row, column] = Cell.Black;
                movingPiece = false;

                s.color = new Color(0.3f, 0.3f, 0.3f, 1);
                s.sprite = b.man;
            }

            else if (isBlackPhase3 || isAdjacent(tempRow, tempCol, row, column))
            {
                BoardState[tempRow, tempCol] = Cell.Vacant;
                BoardState[row, column] = Cell.Black;
                movingPiece = false;

                s.color = new Color(0.3f, 0.3f, 0.3f, 1);
                lastSelected.GetComponent<SpriteRenderer>().sprite = b.man;
                lastSelected.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

                millFormed = CheckMill(currentPlayer, row, column);
                if (millFormed != true)
                {
                    currentPlayer = GetOppositePlayer();
                    turn++;
                }
            }

            else
            {
                tManager.updateEventText("Invalid spot\nPlease try again", currentPlayer);
            }

            /* After moving a piece, check if the opposing player has been locked in. */
            if (!CheckAvailableMove(Player.White))
            {
                GameOver(Player.Black);
            }
        }
    }

    /* Remove piece after validating that the piece can be removed by milling player. 
     * Then, check for phase 3 condition or GameOver condition. */
    public static void Mill(GameObject g, int row, int column)
    {
        /* Remove the piece from the board, set the cell to vacant, and reduce the remaining pieces of that player. */
        BoardState[row, column] = Cell.Vacant;
        g.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        if(currentPlayer == Player.White)
        {
            blackRemainingPieces--;
        }
        else
        {
            whiteRemainingPieces--;
        }

        /* Consume the mill and swap control. */
        millFormed = false;

        /* Check phase 3 and game over conditions. */
        if (currentPlayer == Player.White)
        {
            if (blackRemainingPieces == 3)
            {
                isBlackPhase3 = true;
            }

            if (blackRemainingPieces < 3 || !CheckAvailableMove(Player.Black))
            {
                GameOver(Player.White);
            }
        }
        else
        {
            if (whiteRemainingPieces == 3)
            {
                isWhitePhase3 = true;
            }

            if (whiteRemainingPieces < 3 || !CheckAvailableMove(Player.White))
            {
                GameOver(Player.Black);
            }
        }

        currentPlayer = GetOppositePlayer();
        turn++;
    }



    /* Initiates game over sequence; draw if no player. */
    public static void GameOver()
    {
        tManager.updateStatusText("\nThe game is a draw. Press R to start a new game.", currentPlayer);

        return;
    }

    /* Initiates game over sequence; parameter p is the winning player. */
    public static void GameOver(Player p)
    {
        gameOver = true;
<<<<<<< HEAD
        //tManager.updateStatusText("\nGame over! The winner is " + currentPlayer + "\nPress R to start a new game.", currentPlayer);
        print("game over, the winner is:" + p);
=======
        tManager.updateStatusText("\nGame over! The winner is " + currentPlayer + "\nPress R to start a new game.", currentPlayer);
>>>>>>> ed5d84693be9053f7c963e356c25065979c25458

        return;
    }

    public static bool CheckSamePosition(int r1, int r2, int c1, int c2)
    {
        if (r1 == r2 && c1 == c2)
        {
            return true;
        }

        return false;
    }

    public void Update()       // called every frame
    {
        if (Input.GetKeyDown("r"))
        {
            ResetBoard();
        }

        if (Input.GetKeyDown("o"))
        {
            ComputerOpponent.isActive = true;
        }

        if (turn > 100)
        {
            GameOver();
        }
    }

    public static List<string> getAdjacencyList(int row, int col)
    {
        List<string> adjacencyList = new List<string>();
        string intersectionName;

        /* Find the neighbor to the left. */
        for (int i = col - 1; i > -1; i--)
        {
            if (row == 3 && i == 3)
            {
                break;
            }
            if (BoardState[row, i] != Cell.Invalid)
            {
                intersectionName = CreateIntersectionName(row, i);
                adjacencyList.Add(intersectionName);
                break;
            }
        }

        /* Find the neighbor to the right. */
        for (int i = col + 1; i < 7; i++)
        {
            if (row == 3 && i == 3)
            {
                break;
            }
            if (BoardState[row, i] != Cell.Invalid)
            {
                intersectionName = CreateIntersectionName(row, i);
                adjacencyList.Add(intersectionName);
                break;
            }
        }

        /* Find the neighbor above. */
        for (int i = row + 1; i < 7; i++)
        {
            if (i == 3 && col == 3)
            {
                break;
            }
            if (BoardState[i, col] != Cell.Invalid)
            {
                intersectionName = CreateIntersectionName(i, col);
                adjacencyList.Add(intersectionName);
                break;
            }
        }

        /* Find the neighbor below. */
        for (int i = row - 1; i > -1; i--)
        {
            if (i == 3 && col == 3)
            {
                break;
            }
            if (BoardState[i, col] != Cell.Invalid)
            {
                intersectionName = CreateIntersectionName(i, col);
                adjacencyList.Add(intersectionName);
                break;
            }
        }

        return adjacencyList;
    }
    
    public static bool isAdjacent(int row, int column, int tarRow, int tarCol)
    {
        List<string> adjacencies = getAdjacencyList(row, column);
        string targetName = CreateIntersectionName(tarRow, tarCol);

        for (int i = 0; i < adjacencies.Count; i++)
        {
            if (adjacencies[i] == targetName)
            {
                return true;
            }
        }

        return false;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ResetButton()
    {
        ResetBoard();
    }
}
