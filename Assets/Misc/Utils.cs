using TMPro;
using UnityEngine;

namespace Radishmouse
{
public static class Utils
{
    public static TextMeshPro CreateDebugText(
        Transform parent,
        string text,
        Vector3 localPosition = default,
        int fontSize = 40,
        Color? color = null,
        TextAlignmentOptions textAlignment = TextAlignmentOptions.Center,
        int sortingOrder = 5000
    )
    {
        GameObject gameObject = new GameObject("world text object", typeof(TextMeshPro));
        Transform transform = gameObject.transform;

        transform.SetParent(parent, false);
        transform.localPosition = localPosition;

        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = (color == null) ? Color.white : (Color)color;

        gameObject.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        return textMesh;
    }

    public static void DrawRectCenter(Vector2 center, Vector2 extents)
    {
        DrawRectCorners(center + extents, center - extents);
    }

    public static void DrawRectCorners(Vector2 a, Vector2 b)
    {
        Vector2 ab = new Vector2(a.x, b.y);
        Vector2 ba = new Vector2(b.x, a.y);
        Debug.DrawLine(a, ab);
        Debug.DrawLine(ab, b);
        Debug.DrawLine(b, ba);
        Debug.DrawLine(ba, a);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 wp = GetMouseWorldPositionWithZ();
        wp.z = 0;
        return wp;
    }
    public static Vector3 GetMouseWorldPositionWithZ() => GetMouseWorldPositionWithZ(Camera.main);

    public static Vector3 GetMouseWorldPosition(Camera cam)
    {
        Vector3 wp = GetMouseWorldPositionWithZ(cam);
        wp.z = 0;
        return wp;
    }
    public static Vector3 GetMouseWorldPositionWithZ(Camera cam) => cam.ScreenToWorldPoint((Vector2)Input.mousePosition);
}
}