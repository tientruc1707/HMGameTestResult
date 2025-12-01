using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    private int boardSizeX;

    private int boardSizeY;

    private Cell[,] m_cells;

    private Transform m_root;


    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;
        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[boardSizeX, boardSizeY];
        CreateBoard();
    }

    private void CreateBoard()
    {
        Vector3 origin = new(0, -2, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }

        //set neighbours
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (y + 1 < boardSizeY) m_cells[x, y].NeighbourUp = m_cells[x, y + 1];
                if (x + 1 < boardSizeX) m_cells[x, y].NeighbourRight = m_cells[x + 1, y];
                if (y > 0) m_cells[x, y].NeighbourBottom = m_cells[x, y - 1];
                if (x > 0) m_cells[x, y].NeighbourLeft = m_cells[x - 1, y];
            }
        }
    }

    internal void Fill()
    {
        int totalCells = boardSizeX * boardSizeY;
        int typesCount = Enum.GetValues(typeof(NormalItem.eNormalType)).Length;

        // Tính số lượng mỗi loại (chia hết cho 3)
        int baseCount = totalCells / typesCount;
        int remainder = totalCells % typesCount;

        // Mỗi loại ít nhất có baseCount, một số loại sẽ được +1 để bù dư
        var typeCounts = new Dictionary<NormalItem.eNormalType, int>();
        NormalItem.eNormalType[] allTypes = (NormalItem.eNormalType[])Enum.GetValues(typeof(NormalItem.eNormalType));

        for (int i = 0; i < typesCount; i++)
        {
            int count = baseCount;
            if (i < remainder) count++; // phân phối phần dư cho vài loại đầu tiên

            // Làm tròn xuống để chia hết cho 3
            count = (count / 3) * 3;
            typeCounts[allTypes[i]] = count;
        }
        int currentTotal = typeCounts.Values.Sum();
        int deficit = totalCells - currentTotal;
        for (int i = 0; i < deficit; i += 3)
        {
            // bù thêm 3 cho 1 loại (vẫn giữ chia hết cho 3)
            var typeToBoost = allTypes[i % typesCount];
            typeCounts[typeToBoost] += 3;
        }

        // Tạo danh sách item theo số lượng đã tính
        var itemList = new List<NormalItem>();
        foreach (var kvp in typeCounts)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                NormalItem item = new NormalItem();
                item.SetType(kvp.Key);
                item.SetView();
                item.SetViewRoot(m_root);
                itemList.Add(item);
            }
        }
        itemList = itemList.OrderBy(x => UnityEngine.Random.value).ToList();

        // Gán vào bảng theo thứ tự đã trộn
        int index = 0;
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                Item item = itemList[index++];

                cell.Assign(item);
                cell.ApplyItemPosition(false);
            }
        }
    }

    internal void MoveItemToBottomCell(Cell cell1, Cell cell2)
    {
        Item item = cell1.Item;
        cell1.Free();
        cell2.Assign(item);
        item.View.DOMove(cell2.transform.position, 0.3f);
    }

    internal bool IsEmptyBoard()
    {
        foreach (var cell in m_cells)
        {
            if (!cell.IsEmpty) return false;
        }
        return true;
    }

    internal Cell LookForCellExcept(Cell exceptCell)
    {
        foreach (var cell in m_cells)
        {
            if (!cell.IsSameType(exceptCell) && !cell.IsEmpty)
                return cell;
        }
        return null;
    }

    internal Cell LookForCell(Cell targetCell)
    {
        foreach (var cell in m_cells)
        {
            if (cell.IsSameType(targetCell) && cell != targetCell)
                return cell;
        }
        return null;
    }

    internal Cell GetCell()
    {
        foreach (var cell in m_cells)
        {
            if (!cell.IsEmpty)
                return cell;
        }
        return null;
    }

}
