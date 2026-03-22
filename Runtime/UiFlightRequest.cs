/*
 * UI Flight 演出 1 件分のパラメータを保持する。
 * caller は sprite、anchor、個数、profile を指定するだけで package に描画を委譲できる。
 */

using System;
using UnityEngine;

public sealed class UiFlightRequest
{
    public UiFlightAnchor From { get; set; }

    public UiFlightAnchor To { get; set; }

    public Sprite Sprite { get; set; }

    public Vector2 Size { get; set; }

    public int Count { get; set; } = 1;

    public float Duration { get; set; } = 0.7f;

    public float StartDelay { get; set; }

    public float SpawnInterval { get; set; } = 0.04f;

    public Color Tint { get; set; } = Color.white;

    public UiFlightBezierProfile Profile { get; set; }

    public Action<int, int> OnItemCompleted { get; set; }

    public Action OnCompleted { get; set; }

    internal int ResolveCount()
    {
        return Mathf.Max(1, Count);
    }

    internal float ResolveDuration()
    {
        return Mathf.Max(0.01f, Duration);
    }

    internal Vector2 ResolveSize()
    {
        if (Size.x > 0f && Size.y > 0f)
        {
            return Size;
        }

        if (Sprite != null)
        {
            return Sprite.rect.size;
        }

        return new Vector2(64f, 64f);
    }
}
