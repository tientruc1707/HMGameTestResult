
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    public bool IsBusy { get; private set; }

    public bool CanMoveBack { get; set; }
    private Board m_board;

    private BottomCells m_bottomCells;

    private GameManager m_gameManager;

    private Camera m_cam;

    private bool m_gameOver;

    private int maxBoardY;

    private Cell m_emptyCell;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        maxBoardY = gameSettings.BoardSizeY;
        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);
        m_bottomCells = new BottomCells(gameSettings, this.transform, 5);

        m_emptyCell = m_bottomCells.CurrentEmptyCell;
        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
            case GameManager.eStateGame.VICTORY:
                m_gameOver = true;
                break;
        }
    }


    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;

        if (m_board.IsEmptyBoard())
        {
            m_gameManager.SetState(GameManager.eStateGame.VICTORY);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent<Cell>(out var targetCell))
                {
                    //unable to move from bottom cells
                    if (targetCell.IsEmpty || targetCell.BoardY >= maxBoardY) return;

                    MoveCellToBottom(targetCell);

                }
            }
        }

    }

    private void MoveCellToBottom(Cell cell)
    {
        m_board.MoveItemToBottomCell(cell, m_emptyCell);
        m_bottomCells.MoveToNextEmptyCell();
        if (!m_bottomCells.IsFull)
            m_emptyCell = m_bottomCells.CurrentEmptyCell;
        else
            m_gameManager.SetState(GameManager.eStateGame.GAME_OVER);
    }

    internal IEnumerator AutoWinMode()
    {
        while (!m_gameOver)
        {
            if (m_bottomCells.IsEmptyAll())
            {
                yield return new WaitForSeconds(0.5f);
                Cell cell = m_board.GetCell();
                while (cell == null)
                    cell = m_board.GetCell();
                MoveCellToBottom(cell);
            }
            while (!m_bottomCells.IsFull)
            {
                yield return new WaitForSeconds(0.5f);
                Cell lastCell = m_bottomCells.LastItemCell;
                if (lastCell == null) break;
                Cell cell = m_board.LookForCell(lastCell);
                yield return new WaitForSeconds(0.5f);
                if (cell == null)
                {
                    cell = m_board.GetCell();
                }
                MoveCellToBottom(cell);
            }
        }
    }

    internal IEnumerator AutoLoseMode()
    {

        if (m_bottomCells.IsEmptyAll())
        {
            yield return new WaitForSeconds(0.5f);
            Cell cell = m_board.GetCell();
            while (cell == null)
                cell = m_board.GetCell();
            yield return new WaitForSeconds(0.5f);
            MoveCellToBottom(cell);
        }
        while (!m_bottomCells.IsFull)
        {
            yield return new WaitForSeconds(0.5f);
            Cell cell = m_board.LookForCellExcept(m_bottomCells.LastItemCell);
            while (cell == null)
                cell = m_board.GetCell();
            yield return new WaitForSeconds(0.5f);
            MoveCellToBottom(cell);
        }
    }

}
