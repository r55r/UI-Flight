/*
 * UI Flight package のアンカーをスクリーン座標へ解決する。
 * UI システム差分をこの層へ閉じ込め、飛行描画側は screen-space だけを扱えるようにする。
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public static class UiFlightAnchorUtility
{
    public static bool TryResolveScreenPosition(UiFlightAnchor anchor, out Vector2 screenPosition)
    {
        screenPosition = default;

        if (anchor == null)
        {
            return false;
        }

        switch (anchor.Kind)
        {
            case UiFlightAnchorKind.ScreenPoint:
                screenPosition = anchor.ScreenPoint;
                return true;

            case UiFlightAnchorKind.WorldPoint:
                screenPosition = RectTransformUtility.WorldToScreenPoint(
                    anchor.Camera,
                    anchor.WorldPoint
                );
                return true;

            case UiFlightAnchorKind.RectTransform:
                return TryResolveRectTransform(anchor.RectTransform, out screenPosition);

            case UiFlightAnchorKind.VisualElement:
                return TryResolveVisualElement(anchor.VisualElement, out screenPosition);

            default:
                return false;
        }
    }

    private static bool TryResolveRectTransform(
        RectTransform rectTransform,
        out Vector2 screenPosition
    )
    {
        screenPosition = default;

        if (rectTransform == null)
        {
            return false;
        }

        Vector3 worldCenter = rectTransform.TransformPoint(rectTransform.rect.center);
        Camera camera = ResolveCanvasCamera(rectTransform);
        screenPosition = RectTransformUtility.WorldToScreenPoint(camera, worldCenter);
        return true;
    }

    private static Camera ResolveCanvasCamera(RectTransform rectTransform)
    {
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return null;
        }

        return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    private static bool TryResolveVisualElement(
        VisualElement visualElement,
        out Vector2 screenPosition
    )
    {
        screenPosition = default;

        if (visualElement == null)
        {
            return false;
        }

        Rect elementRect = visualElement.worldBound;
        Vector2 panelCenter = elementRect.center;

        if (TryGetPanelRect(visualElement, out Rect panelRect))
        {
            float normalizedX = Mathf.InverseLerp(panelRect.xMin, panelRect.xMax, panelCenter.x);
            float normalizedY = Mathf.InverseLerp(panelRect.yMin, panelRect.yMax, panelCenter.y);
            screenPosition = new Vector2(
                normalizedX * Screen.width,
                Screen.height - (normalizedY * Screen.height)
            );
            return true;
        }

        screenPosition = new Vector2(panelCenter.x, Screen.height - panelCenter.y);
        return true;
    }

    private static bool TryGetPanelRect(VisualElement visualElement, out Rect panelRect)
    {
        panelRect = default;

        if (visualElement.panel?.visualTree == null)
        {
            return false;
        }

        panelRect = visualElement.panel.visualTree.worldBound;
        return panelRect.width > 0f && panelRect.height > 0f;
    }
}
