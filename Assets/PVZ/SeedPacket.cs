using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// calls the plantplacer when clicked and changes sprites when selected/deselected
public class SeedPacket : MonoBehaviour, IPointerDownHandler
{
    public GameObject plantPrefab;
    public Image border;
    public Sprite unselectedBorder, selectedBorder;

    void Awake()
    {
        border.sprite = unselectedBorder;
    }

    // called when you click the packet
    public void OnPointerDown(PointerEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        PlantPlacer.SelectPlant(plantPrefab, this);
        border.sprite = selectedBorder;
    }

    public void Unselect()
    {
        border.sprite = unselectedBorder;
    }
}