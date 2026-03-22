/*
 * UI Flight の始点・終点として扱うアンカー情報を表現する。
 * uGUI / UI Toolkit / screen / world の各経路を単一型に集約して caller を簡潔に保つ。
 */

using UnityEngine;
using UnityEngine.UIElements;

public sealed class UiFlightAnchor
{
    public UiFlightAnchorKind Kind { get; }

    public RectTransform RectTransform { get; }

    public VisualElement VisualElement { get; }

    public Vector2 ScreenPoint { get; }

    public Vector3 WorldPoint { get; }

    public Camera Camera { get; }

    private UiFlightAnchor(
        UiFlightAnchorKind kind,
        RectTransform rectTransform = null,
        VisualElement visualElement = null,
        Vector2 screenPoint = default,
        Vector3 worldPoint = default,
        Camera camera = null
    )
    {
        Kind = kind;
        RectTransform = rectTransform;
        VisualElement = visualElement;
        ScreenPoint = screenPoint;
        WorldPoint = worldPoint;
        Camera = camera;
    }

    public static UiFlightAnchor FromRectTransform(RectTransform rectTransform)
    {
        return new UiFlightAnchor(UiFlightAnchorKind.RectTransform, rectTransform: rectTransform);
    }

    public static UiFlightAnchor FromVisualElement(VisualElement visualElement)
    {
        return new UiFlightAnchor(UiFlightAnchorKind.VisualElement, visualElement: visualElement);
    }

    public static UiFlightAnchor FromScreenPoint(Vector2 screenPoint)
    {
        return new UiFlightAnchor(UiFlightAnchorKind.ScreenPoint, screenPoint: screenPoint);
    }

    public static UiFlightAnchor FromWorldPoint(Vector3 worldPoint, Camera camera = null)
    {
        return new UiFlightAnchor(
            UiFlightAnchorKind.WorldPoint,
            worldPoint: worldPoint,
            camera: camera
        );
    }
}

public enum UiFlightAnchorKind
{
    None,
    RectTransform,
    VisualElement,
    ScreenPoint,
    WorldPoint,
}
