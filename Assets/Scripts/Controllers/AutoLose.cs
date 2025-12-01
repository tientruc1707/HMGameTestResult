using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoLose : LevelCondition
{

    private BoardController m_board;

    public override void Setup(Text txt, GameManager mngr, BoardController board)
    {
        base.Setup(txt, mngr, board);
        m_board = board;
        m_board.CanMoveBack = false;
        UpdateText();
        StartCoroutine(m_board.AutoLoseMode());
    }

    protected override void UpdateText()
    {
        m_txt.text = "AUTO LOSE";
    }
}
