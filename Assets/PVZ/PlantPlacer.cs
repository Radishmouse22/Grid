using UnityEngine;
using Radishmouse;

public class PlantPlacer : MonoBehaviour
{
    public Vector2Int gridSize;
    public Vector2 topLeft;
    public Vector2 plantOffset;

    Grid<Plant> plantGrid;
    static GameObject plantPrefab;
    static SeedPacket lastSelected;

    void Start()
    {
        plantGrid = new Grid<Plant>(gridSize.x, gridSize.y, 1, 1, topLeft, GridAlignMode.TopLeft);
    }

    // called from SeedPackets when you click them
    public static void SelectPlant(GameObject plantPrefab, SeedPacket selected)
    {
        // unselect the last packet and select this one (setting the prefab)
        if (lastSelected != null)
            lastSelected.Unselect();
        lastSelected = selected;
        PlantPlacer.plantPrefab = plantPrefab;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            PlacePlant();
    }

    void PlacePlant()
    {
        // gets grid coordinates of mouse position
        if (plantGrid.GetCoordinatesFromPosition(Utils.GetMouseWorldPosition(), out int lawnX, out int lawnY))
        {
            // can't already be a plant there and we must have a plant selected
            if (plantGrid.GetValue(lawnX, lawnY) == null && plantPrefab != null)
            {
                plantGrid.SetValue(lawnX, lawnY,
                    Instantiate(
                        plantPrefab,
                        plantGrid.GetPositionFromCoordinates(lawnX, lawnY) + (Vector3)plantOffset,
                        Quaternion.identity
                    ).GetComponent<Plant>()
                );
            }
        }
    }
}
