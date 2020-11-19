using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BoardManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            GameObject g = new GameObject("BoardManager");
            g.AddComponent<BoardManager>();
            BoardManager.InitGame();
        }

        [TearDown]
        public void TearDown()
        {
            BoardManager.ResetBoard();
        }

        [Test]
        public void VacantSpacesAtGameStart()
        {
            Assert.AreEqual(BoardManager.BoardState[0, 0], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[0, 3], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[0, 6], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[1, 1], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[1, 3], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[1, 5], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[2, 2], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[2, 3], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[2, 4], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 0], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 1], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 2], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 4], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 5], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 6], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 4], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 5], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[3, 6], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[4, 2], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[4, 3], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[4, 4], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[5, 1], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[5, 3], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[5, 5], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[6, 0], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[6, 3], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[6, 6], Cell.Vacant);
        }

        [Test]
        public void CheckMill()
        {
            GameObject g;

            string[] moves = { "a1", "a4", "d1", "b4", "g1" };

            for (int i = 0; i < moves.Length; i++)
            {
                g = BoardManager.FindIntersection(moves[i]);
                Intersection intersection = g.GetComponent<Intersection>();
                intersection.OnMouseDown();
            }

            Assert.IsTrue(BoardManager.millFormed);
        }

        [Test]
        public void CheckMillFalse()
        {
            GameObject g;

            string[] moves = { "a1", "a4", "a7" };

            for (int i = 0; i < moves.Length; i++)
            {
                g = BoardManager.FindIntersection(moves[i]);
                Intersection intersection = g.GetComponent<Intersection>();
                intersection.OnMouseDown();
            }

            Assert.IsFalse(BoardManager.millFormed);
        }

        [Test]
        public void CheckWinConditionOne()
        {
            GameObject g;

            BoardManager.blackUnplacedPieces = 3;
            BoardManager.whiteUnplacedPieces = 3;
            string[] moves = { "a1", "b2", "d1", "d2", "g1", "d2" };
            BoardManager.blackRemainingPieces = 3;
            BoardManager.whiteRemainingPieces = 3;

            for (int i = 0; i < moves.Length; i++)
            {
                g = BoardManager.FindIntersection(moves[i]);
                Intersection intersection = g.GetComponent<Intersection>();
                intersection.OnMouseDown();
            }



            Assert.IsTrue(BoardManager.gameOver);
        }

        [Test]
        public void CheckWinConditionTwo()
        {
            GameObject g;


            string[] moves = { "a7", "a1", "a4", "g7", "d7", "b2", "b6", "f6", "b4", "c4", "d6", "d5", "c5", "e3", "c3", "d3", "e5", "e4" };


            for (int i = 0; i < moves.Length; i++)
            {
                g = BoardManager.FindIntersection(moves[i]);
                Intersection intersection = g.GetComponent<Intersection>();
                intersection.OnMouseDown();
            }



            Assert.IsTrue(BoardManager.gameOver);
        }

        [Test]
        public void CheckWindConitionFalse()
        {
            GameObject g;


            string[] moves = { "a7", "a1", "a4" };


            for (int i = 0; i < moves.Length; i++)
            {
                g = BoardManager.FindIntersection(moves[i]);
                Intersection intersection = g.GetComponent<Intersection>();
                intersection.OnMouseDown();
            }



            Assert.IsFalse(BoardManager.gameOver);
        }

        [Test]
        public void CheckValidMovePhase1()
        {


            Assert.AreEqual(BoardManager.BoardState[0, 0], Cell.Vacant);

            GameObject g = BoardManager.FindIntersection("a1");
            Intersection intersection = g.GetComponent<Intersection>();
            intersection.OnMouseDown();

            Assert.AreNotEqual(BoardManager.BoardState[0, 0], Cell.Vacant);


        }

        [Test]
        public void CheckValidMovePhase2()
        {

            GameObject g;

            string[] moves = { "a1", "a4", "d2", "b4", "g1", "d1", "b2", "a7", "d7", "f2", "g7", "d3", "c3", "c4", "d5", "f6", "e3", "e5", "d2", "c5" };

            for (int i = 0; i < moves.Length; i++)
            {
                g = BoardManager.FindIntersection(moves[i]);
                Intersection intersection = g.GetComponent<Intersection>();
                intersection.OnMouseDown();
            }

            Assert.AreNotEqual(BoardManager.BoardState[0, 6], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[2, 4], Cell.Vacant);
        }

        [Test]
        public void CheckValidMovePhase3()
        {
            GameObject g;

            BoardManager.isBlackPhase3 = true;
            BoardManager.isWhitePhase3 = true;
            BoardManager.blackRemainingPieces = 3;
            BoardManager.whiteRemainingPieces = 3;
            BoardManager.whiteUnplacedPieces = 0;
            BoardManager.blackUnplacedPieces = 0;



            BoardManager.BoardState[0, 0] = Cell.Black;
            BoardManager.BoardState[1, 1] = Cell.Black;
            BoardManager.BoardState[3, 1] = Cell.Black;
            BoardManager.BoardState[0, 6] = Cell.White;
            BoardManager.BoardState[3, 2] = Cell.White;
            BoardManager.BoardState[5, 1] = Cell.White;

            string[] moves = { "a1", "g7", "g1", "f6" };
            BoardManager.currentPlayer = Player.Black;

            for (int i = 0; i < moves.Length; i++)
            {
                g = BoardManager.FindIntersection(moves[i]);
                Intersection intersection = g.GetComponent<Intersection>();
                intersection.OnMouseDown();
            }

            Assert.AreEqual(BoardManager.BoardState[6, 6], Cell.Black);
            Assert.AreEqual(BoardManager.BoardState[5, 5], Cell.White);
            Assert.AreEqual(BoardManager.BoardState[0, 0], Cell.Vacant);
            Assert.AreEqual(BoardManager.BoardState[0, 6], Cell.Vacant);
        }
    }
}