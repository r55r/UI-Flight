/*
 * UI Flight の数式ロジックを検証する。
 * ベジェ終端と spread 計算が崩れると全演出の見た目が壊れるため、最小限の回帰テストを持つ。
 */

using NUnit.Framework;
using UnityEngine;

public sealed class UiFlightMathTests
{
    [Test]
    public void EvaluateCubicBezier_ReturnsStartAndEndAtBounds()
    {
        Vector2 start = new(10f, 20f);
        Vector2 controlPoint1 = new(30f, 120f);
        Vector2 controlPoint2 = new(80f, 150f);
        Vector2 end = new(200f, 240f);

        Assert.That(
            UiFlightMath.EvaluateCubicBezier(start, controlPoint1, controlPoint2, end, 0f),
            Is.EqualTo(start)
        );
        Assert.That(
            UiFlightMath.EvaluateCubicBezier(start, controlPoint1, controlPoint2, end, 1f),
            Is.EqualTo(end)
        );
    }

    [Test]
    public void CalculateSpreadOffset_ReturnsSymmetricOffsets()
    {
        float first = UiFlightMath.CalculateSpreadOffset(0, 3, 30f);
        float middle = UiFlightMath.CalculateSpreadOffset(1, 3, 30f);
        float last = UiFlightMath.CalculateSpreadOffset(2, 3, 30f);

        Assert.That(first, Is.EqualTo(-15f).Within(0.001f));
        Assert.That(middle, Is.EqualTo(0f).Within(0.001f));
        Assert.That(last, Is.EqualTo(15f).Within(0.001f));
    }

    [Test]
    public void ResolveScaledSize_ScreenHeightRatio_ScalesByHeightRatio()
    {
        Vector2 resolved = UiFlightMath.ResolveScaledSize(
            new Vector2(64f, 80f),
            UiFlightSizeMode.ScreenHeightRatio,
            2532f,
            1920f
        );

        Assert.That(resolved.x, Is.EqualTo(84.4f).Within(0.01f));
        Assert.That(resolved.y, Is.EqualTo(105.5f).Within(0.01f));
    }

    [Test]
    public void ResolveScaledSize_RawPixels_ReturnsBaseSize()
    {
        Vector2 resolved = UiFlightMath.ResolveScaledSize(
            new Vector2(64f, 80f),
            UiFlightSizeMode.RawPixels,
            2532f,
            1920f
        );

        Assert.That(resolved.x, Is.EqualTo(64f).Within(0.001f));
        Assert.That(resolved.y, Is.EqualTo(80f).Within(0.001f));
    }

    [Test]
    public void ResolveScaledSize_InvalidReferenceHeight_FallsBackTo1920()
    {
        Vector2 resolved = UiFlightMath.ResolveScaledSize(
            new Vector2(64f, 80f),
            UiFlightSizeMode.ScreenHeightRatio,
            960f,
            0f
        );

        Assert.That(resolved.x, Is.EqualTo(32f).Within(0.001f));
        Assert.That(resolved.y, Is.EqualTo(40f).Within(0.001f));
    }
}
