using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMoves : LevelCondition
{

    public override void Setup(float value, Text txt, BoardController board)
    {
        base.Setup(value, txt);
        UpdateText();
    }

    protected override void UpdateText()
    {
        m_txt.text = "NORMAL";
    }


}
