using UnityEngine;
using Radishmouse;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Heatmap : MonoBehaviour
{
    public Vector2Int gridSize;
    public float colorScale; // how many seconds of hovering it takes to max the heatmap
    public int cursorRange; // the range of cells the cursor can affect

    Grid<float> heatMap;
    MeshFilter meshFilter;
    Mesh mesh;
    Color32[] colors;

    void Start()
    {
        // create and center grid
        heatMap = new Grid<float>(gridSize.x, gridSize.y, 1, 1, Vector3.zero, GridAlignMode.MiddleCenter);
        transform.position = (Vector2)gridSize * new Vector2(-0.5f, -0.5f);

        // initialize mesh arrays
        int cellCount = gridSize.x * gridSize.y;
        Vector3[] vertices = new Vector3[cellCount * 4];
        int[] triangles = new int[cellCount * 6];
        colors = new Color32[cellCount * 4];

        // set mesh arrays
        int vertexIterator = 0;
        int triangleIterator = 0;
        for (int x = 0; x < gridSize.x; x++) {
            for (int y = 0; y < gridSize.y; y++)
            {
                vertices[vertexIterator    ] = new Vector3(x, y, 0f);
                vertices[vertexIterator + 1] = new Vector3(x, y + 1f, 0f);
                vertices[vertexIterator + 2] = new Vector3(x + 1f, y, 0f);
                vertices[vertexIterator + 3] = new Vector3(x + 1f, y + 1f, 0f);

                colors[vertexIterator    ] = new Color32(0, 0, 0, 255);
                colors[vertexIterator + 1] = new Color32(0, 0, 0, 255);
                colors[vertexIterator + 2] = new Color32(0, 0, 0, 255);
                colors[vertexIterator + 3] = new Color32(0, 0, 0, 255);

                triangles[triangleIterator    ] = vertexIterator + 0;
                triangles[triangleIterator + 1] = vertexIterator + 1;
                triangles[triangleIterator + 2] = vertexIterator + 2;
                triangles[triangleIterator + 3] = vertexIterator + 1;
                triangles[triangleIterator + 4] = vertexIterator + 3;
                triangles[triangleIterator + 5] = vertexIterator + 2;

                vertexIterator += 4;
                triangleIterator +=  6;
            }
        }

        // set mesh to meshFilter
        mesh = new()
        {
            vertices = vertices,
            triangles = triangles,
            colors32 = colors,
        };
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
    }

    void Update()
    {
        // gets grid coordinates of mouse position
        if (heatMap.GetCoordinatesFromPosition(Utils.GetMouseWorldPosition(), out int cursorX, out int cursorY))
        {
            // loop through all cells within range of the cursor
            for (int xOffset = -cursorRange; xOffset <= cursorRange; xOffset++) {
                for (int yOffset = -cursorRange; yOffset <= cursorRange; yOffset++)
                {
                    int xCoordinate = cursorX + xOffset;
                    int yCoordinate = cursorY + yOffset;

                    if (!heatMap.InRange(xCoordinate, yCoordinate))
                        continue;

                    // calculate what to add to the cell depending on how far away it is from the cursor
                    int cellTaxicabDistanceFromCursor = Mathf.Abs(xOffset) + Mathf.Abs(yOffset);
                    float valueScaler = (float)((cursorRange * 2) - cellTaxicabDistanceFromCursor) / (cursorRange * 2);
                    float easedValueScaler = Mathf.Pow(valueScaler, 3);

                    // add value in grid and set vertex colors in colors array
                    heatMap.AddValue(xCoordinate, yCoordinate, Time.deltaTime * easedValueScaler);
                    UpdateCellColor(xCoordinate, yCoordinate);
                }
            }

            // update colors in mesh (could be optimized with a NativeArray but idc)
            mesh.SetColors(colors);
        }
    }

    // updates all of the vertex colors for a cell
    void UpdateCellColor(int x, int y)
    {
        int bottomLeftVertexIndex = (x * gridSize.y + y)*4;
        Color32 color = LerpNumberToColor(heatMap.GetValue(x, y)/colorScale);
        colors[bottomLeftVertexIndex    ] = color;
        colors[bottomLeftVertexIndex + 1] = color;
        colors[bottomLeftVertexIndex + 2] = color;
        colors[bottomLeftVertexIndex + 3] = color;
    }

    // turns x (0 to 1) to to color (red to green)
    // magic number nonsense I don't feel like expanding and making readable
    Color32 LerpNumberToColor(float x)
    {
        x = Mathf.Clamp01(x) * 765;
        return new Color32(
            (byte)((x <= 510) ? (x <= 255) ? x : 255 : 765 - x), // r
            (byte)((x <= 510) ? (x <= 255) ? 0 : x-255 : 255), // g
            0, // b
        255);
    }
}
