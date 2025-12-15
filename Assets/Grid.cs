#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

using System;
using UnityEngine;
using TMPro;
using Radishmouse;

/// <summary>
/// 2D rectangle grid that can translate between world and grid coordinates
/// </summary>
public class Grid<T>
{
    readonly int width, height;
    readonly float cellWidth, cellHeight;
    readonly Vector3 origin;
    readonly GridAlignMode alignMode;
    int VerticalAlignment => (byte)alignMode >> 2;
    int HorizontalAlignment => (byte)alignMode & 3;

    readonly T[,] values;
    readonly Action<T, T> onGridValueChanged;

    bool debug;
    TextMeshPro[,] textObjects;

    public Grid(int width, int height, float cellWidth, float cellHeight, Vector3 origin,
        GridAlignMode alignMode = GridAlignMode.BottomLeft,
        Func<int, int, T> onInitializeGridValue = null, // (x : int, y : int) => initialized value : T
        Action<T, T> onGridValueChanged = null // (oldValue : T, newValue : T)
    )
    {
        this.width = width;
        this.height = height;
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;
        this.origin = origin;
        this.alignMode = alignMode;
        this.onGridValueChanged = onGridValueChanged;

        values = new T[width, height];

        // initialize values if necessary
        if (onInitializeGridValue != null)
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    values[x, y] = onInitializeGridValue(x, y);

        debug = false;
    }

    public T GetValue(int x, int y)
    {
        if (!InRange(x, y))
            throw new IndexOutOfRangeException();

        return values[x, y];
    }

    public T SetValue(int x, int y, T newValue)
    {
        if (!InRange(x, y))
            throw new IndexOutOfRangeException();

        T oldValue = values[x, y];
        values[x, y] = newValue;

        onGridValueChanged?.Invoke(oldValue, newValue);

        if (debug) UpdateDebugText(x, y);
        return oldValue;
    }

    // gives the 3d world position of the center of a grid cell
    public Vector3 GetPositionFromCoordinates(int x, int y)
    {
        if (!InRange(x, y))
            throw new IndexOutOfRangeException();

        float posX = origin.x + HorizontalAlignment switch
        {
            0 => (x + 0.5f) * cellWidth, // left
            1 => (x + 0.5f - width / 2f) * cellWidth, // center
            2 => -(x + 0.5f) * cellWidth, // right
        };

        float posY = origin.y + VerticalAlignment switch
        {
            0 => -(y + 0.5f) * cellHeight, // top
            1 => (y + 0.5f - height / 2f) * cellHeight, // middle
            2 => (y + 0.5f) * cellHeight, // bottom
        };

        return new Vector3(posX, posY);
    }

    // gives the cell that contains the point in world space
    public bool GetCoordinatesFromPosition(Vector3 position, out int x, out int y)
    {
        x = HorizontalAlignment switch
        {
            0 => Mathf.FloorToInt((position.x-origin.x)/cellWidth), // left
            1 => Mathf.FloorToInt((position.x-origin.x)/cellWidth + width/2f), // center
            2 => -Mathf.CeilToInt((position.x-origin.x)/cellWidth), // right
        };

        y = VerticalAlignment switch
        {
            0 => -Mathf.CeilToInt((position.y-origin.y)/cellHeight), // top
            1 => Mathf.FloorToInt((position.y-origin.y)/cellHeight + height/2f), // middle
            2 => Mathf.FloorToInt((position.y-origin.y)/cellHeight), // bottom
        };

        return InRange(x, y);
    }

    public bool InRange(int x, int y) => !(x < 0 || y < 0 || x >= width || y >= height);

    void UpdateDebugText(int x, int y)
    {
        textObjects[x, y].text = (values[x, y] == null) ? "NULL" : values[x, y].ToString();
    }

    // enables or disables debug mode and creates/enables the text objects
    // creates them if they don't exist yet and reenables them if they do
    public void SetDebug(bool debug)
    {
        if (debug == this.debug)
            return;
        this.debug = debug;

        if (debug)
        {
            if (textObjects == null)
            {
                textObjects = new TextMeshPro[width, height];

                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        textObjects[x, y] = Utils.CreateDebugText(null, "", GetPositionFromCoordinates(x, y), 4, Color.white);
                        UpdateDebugText(x, y);
                    }

                return;
            }

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    textObjects[x, y].enabled = true;
                    UpdateDebugText(x, y);
                }

            return;
        }

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                textObjects[x, y].enabled = false;
    }

    // draw the outline for the grid for one frame
    // should call in OnDrawGizmos() or Update()
    public void DrawOutlines(Color? color = null)
    {
        if (color == null)
            color = Color.white;

        for (int x = 0; x < width+1; x++)
        {
            float xOffset = HorizontalAlignment switch
            {
                0 => x * cellWidth, // left
                1 => (x - width / 2f) * cellWidth, // center
                2 => -x * cellWidth, // right
            };
            float top = VerticalAlignment switch
            {
                0 => 0, // top
                1 => height * cellHeight / 2f, // middle
                2 => height * cellHeight, // bottom
            };
            float bottom = VerticalAlignment switch
            {
                0 => -height * cellHeight, // top
                1 => -height * cellHeight / 2f, // middle
                2 => 0, // bottom
            };
            Debug.DrawLine(origin + new Vector3(xOffset, top), origin + new Vector3(xOffset, bottom), (Color)color);
        }

        for (int y = 0; y < height+1; y++)
        {
            float yOffset = VerticalAlignment switch
            {
                0 => -y * cellHeight, // top
                1 => (y - height / 2f) * cellHeight, // middle
                2 => y * cellHeight, // bottom
            };
            float left = HorizontalAlignment switch
            {
                0 => 0, // left
                1 => -width * cellWidth / 2f, // center
                2 => -width * cellWidth, // right
            };
            float right = HorizontalAlignment switch
            {
                0 => width * cellWidth, // left
                1 => width * cellWidth / 2f, // center
                2 => 0, // right
            };
            Debug.DrawLine(origin + new Vector3(left, yOffset), origin + new Vector3(right, yOffset), (Color)color);
        }
    }

    public void DestroyDebugTextObjects()
    {
        if (textObjects != null)
        {
            for (int x = 0; x < textObjects.GetLength(0); x++)
                for (int y = 0; y < textObjects.GetLength(1); y++)
                    GameObject.Destroy(textObjects[x, y].gameObject);
        }
    }
}

// using binary allows for:
// int vert = (byte)alignMode >> 2;
// int horizontal = (byte)alignMode & 3;
[System.Serializable]
public enum GridAlignMode : byte
{
    TopLeft      = 0b0000,
    MiddleLeft   = 0b0100,
    BottomLeft   = 0b1000,
    TopCenter    = 0b0001,
    MiddleCenter = 0b0101,
    BottomCenter = 0b1001,
    TopRight     = 0b0010,
    MiddleRight  = 0b0110,
    BottomRight  = 0b1010,
}

public static class GridExtensions
{
    public static void AddValue(this Grid<int> grid, int x, int y, int value) => grid.SetValue(x, y, grid.GetValue(x, y) + value);
    public static void AddValue(this Grid<float> grid, int x, int y, float value) => grid.SetValue(x, y, grid.GetValue(x, y) + value);
}

#pragma warning restore CS8509