/*
 * UI Flight のアンカー解決を検証する。
 * screen/world/uGUI の最小経路が正しく解決できないと演出開始位置が破綻するため、主要ケースだけを固定する。
 */

using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public sealed class UiFlightAnchorUtilityTests
{
    [Test]
    public void TryResolveScreenPosition_ReturnsGivenScreenPoint()
    {
        var anchor = UiFlightAnchor.FromScreenPoint(new Vector2(123f, 456f));

        bool resolved = UiFlightAnchorUtility.TryResolveScreenPosition(anchor, out Vector2 point);

        Assert.That(resolved, Is.True);
        Assert.That(point, Is.EqualTo(new Vector2(123f, 456f)));
    }

    [Test]
    public void TryResolveScreenPosition_ResolvesRectTransformCenter()
    {
        var canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(RectTransform));
        var rectObject = new GameObject("Target", typeof(RectTransform));

        try
        {
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var canvasRect = canvasObject.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1080f, 1920f);

            var rectTransform = rectObject.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasRect, false);
            rectTransform.sizeDelta = new Vector2(100f, 120f);
            rectTransform.anchoredPosition = new Vector2(40f, 80f);

            bool resolved = UiFlightAnchorUtility.TryResolveScreenPosition(
                UiFlightAnchor.FromRectTransform(rectTransform),
                out Vector2 point
            );

            Vector2 expected = RectTransformUtility.WorldToScreenPoint(
                null,
                rectTransform.TransformPoint(rectTransform.rect.center)
            );

            Assert.That(resolved, Is.True);
            Assert.That(point.x, Is.EqualTo(expected.x).Within(0.01f));
            Assert.That(point.y, Is.EqualTo(expected.y).Within(0.01f));
        }
        finally
        {
            Object.DestroyImmediate(rectObject);
            Object.DestroyImmediate(canvasObject);
        }
    }
}
