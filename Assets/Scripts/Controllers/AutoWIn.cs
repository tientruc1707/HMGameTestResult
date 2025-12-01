using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class AutoWIn : LevelCondition
{

    private BoardController m_board;

    public override void Setup(Text txt, GameManager mngr, BoardController board)
    {
        base.Setup(txt, mngr, board);
        m_board = board;
        UpdateText();
        OnConditionComplete();
        StartCoroutine(m_board.AutoWinMode());
    }
    protected override void UpdateText()
    {
        m_txt.text = "AUTO WIN";
    }
}
