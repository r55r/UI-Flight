/*
 * UI Flight 演出で使う数式処理を集約する。
 * ベジェ評価や curve 評価を独立させ、runtime tests からも直接検証できるようにする。
 */

using UnityEngine;

public static class UiFlightMath
{
    public static Vector2 EvaluateCubicBezier(
        Vector2 start,
        Vector2 controlPoint1,
        Vector2 controlPoint2,
        Vector2 end,
        float t
    )
    {
        float clamped = Mathf.Clamp01(t);
        float inverse = 1f - clamped;

        return (inverse * inverse * inverse * start)
            + (3f * inverse * inverse * clamped * controlPoint1)
            + (3f * inverse * clamped * clamped * controlPoint2)
            + (clamped * clamped * clamped * end);
    }

    public static float EvaluateCurve(AnimationCurve curve, float time, float fallbackValue)
    {
        return curve == null ? fallbackValue : curve.Evaluate(Mathf.Clamp01(time));
    }

    public static float CalculateSpreadOffset(int index, int count, float spread)
    {
        if (count <= 1 || Mathf.Approximately(spread, 0f))
        {
            return 0f;
        }

        float normalized = (index / (float)(count - 1)) - 0.5f;
        return normalized * spread;
    }

    public static Vector2 ResolveScaledSize(
        Vector2 baseSize,
        UiFlightSizeMode sizeMode,
        float currentScreenHeight,
        float referenceScreenHeight
    )
    {
        if (sizeMode == UiFlightSizeMode.RawPixels)
        {
            return baseSize;
        }

        float safeReferenceHeight = referenceScreenHeight > 0f ? referenceScreenHeight : 1920f;
        float safeCurrentHeight =
            currentScreenHeight > 0f ? currentScreenHeight : safeReferenceHeight;
        float scale = safeCurrentHeight / safeReferenceHeight;
        return baseSize * scale;
    }
}
