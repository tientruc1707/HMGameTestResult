using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCells
{
    private List<Cell> m_cells;
    private Transform m_root;
    private int m_currentIndex = 0;
    public Cell CurrentEmptyCell { get; set; }
    public Cell LastItemCell
    {
        get
        {
            if (m_currentIndex - 1 >= 0 && m_currentIndex - 1 < m_cells.Count)
                return m_cells[m_currentIndex - 1];
            return null;
        }
    }
    
    public bool IsFull = false;

    public BottomCells(GameSettings gameSettings, Transform transform, int count)
    {
        m_root = transform;
        m_cells = new(count);
        int Ypos = gameSettings.BoardSizeY + 1;
        CreateBottomCells(count, Ypos);
    }

    private void CreateBottomCells(int count, int Ypos)
    {
        Vector3 origin = new(0f, -3.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int i = 0; i < count; i++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);
            go.transform.position = origin + new Vector3(i, 0f, 0f);
            go.transform.SetParent(m_root);

            Cell cell = go.GetComponent<Cell>();
            cell.Setup(i, Ypos);

            m_cells.Add(cell);
        }

        for (int i = 0; i < count - 1; i++)
        {
            m_cells[i].NeighbourRight = m_cells[i + 1];
            m_cells[i + 1].NeighbourLeft = m_cells[i];
        }

        CurrentEmptyCell = m_cells[m_currentIndex];
    }

    internal void MoveToNextEmptyCell()
    {

        if (CurrentEmptyCell.Item != null)
        {
            m_cells[m_currentIndex] = CurrentEmptyCell;
            ClearCells();
            m_currentIndex++;
            if (m_currentIndex < m_cells.Count)
            {
                CurrentEmptyCell = m_cells[m_currentIndex];
            }
            else
            {
                IsFull = true;
            }
        }

    }

    public bool IsEmptyAll()
    {
        return m_cells[0].Item == null;
    }

    public void ClearCells()
    {
        if (m_cells[2].Item == null) return;
        //m_cells.Sort((a, b) => b.Item.IsSameType(a.Item) ? 1 : 0);
        if (m_cells[m_currentIndex].Item.IsSameType(m_cells[m_currentIndex - 1].Item) &&
            m_cells[m_currentIndex].Item.IsSameType(m_cells[m_currentIndex - 2].Item))
        {
            m_cells[m_currentIndex].ExplodeItem();
            m_cells[m_currentIndex - 1].ExplodeItem();
            m_cells[m_currentIndex - 2].ExplodeItem();
            m_currentIndex -= 3;
        }
        if (m_currentIndex >= m_cells.Count) IsFull = true;

    }

}
