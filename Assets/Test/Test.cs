using UnityEngine;
using Radishmouse;

public class Test : MonoBehaviour
{
    public Vector2Int gridSize;
    public Vector2 cellSize;
    public Vector2 origin;
    public Transform originMarker;
    public GridAlignMode alignMode;

    Grid<int> grid;
    bool debug = true;
    bool shouldReset = false;

    void OnValidate()
    {
        if (Application.isPlaying)
            shouldReset = true;
    }

    void Start() => ResetGrid();

    void Update()
    {
        if (shouldReset)
        {
            ResetGrid();
            shouldReset = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            debug = !debug;
            grid.SetDebug(debug);
        }

        // if (Input.GetMouseButtonDown(0))
            if (grid.GetCoordinatesFromPosition(Utils.GetMouseWorldPosition(), out int cursorX, out int cursorY))
                grid.AddValue(cursorX, cursorY, 1);
    }

    void ResetGrid()
    {
        originMarker.position = origin;
        grid?.DestroyDebugTextObjects();
        grid = new Grid<int>(gridSize.x, gridSize.y, cellSize.x, cellSize.y, origin, alignMode);
        grid.SetDebug(debug);
    }

    void OnDrawGizmos()
    {
        grid?.DrawOutlines();
    }
}
