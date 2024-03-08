using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize = 16;
    private Vector3 gridPosition = Vector3.zero;
    // grid height == list
    // grid width == array;
    private List<Cell[]> grid;
    //private 

    //public Grid(int width, int height, int cellSize)
    //{

    //}

    void Awake()
    {
        grid = new List<Cell[]>();
        for (int i = 0; i < height; i++)
        {
            grid.Add(new Cell[width]);
            for (int j = 0; j < width; j++)
            {
                grid[i][j] = new Cell();
            }
        }
    }

    public Vector3 GetWorldPosition(int x , int y)
    {
        return new Vector3(x, y) * cellSize + gridPosition;
    }
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x - gridPosition.x) / cellSize;
        y = Mathf.FloorToInt(worldPosition.y - gridPosition.y) / cellSize;
    }
    public Cell GetCell(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return grid[x][y];
        return null;
    }
    public Cell GetCell(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetCell(x, y);
    }
    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }
    public List<Cell[]> GetGrid()
    {
        return grid;
    }
}
