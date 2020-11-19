using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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
    }
}
