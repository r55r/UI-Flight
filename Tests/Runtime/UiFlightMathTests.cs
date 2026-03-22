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
}
