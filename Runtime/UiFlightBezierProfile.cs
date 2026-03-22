/*
 * UI Flight のベジェ軌道と見た目カーブを ScriptableObject で保持する。
 * ゲーム側は複数 profile asset を作り分けて演出差分を管理できる。
 */

using UnityEngine;

[CreateAssetMenu(
    fileName = "UiFlightBezierProfile",
    menuName = "Flying Gorilla/UI Flight/Bezier Profile"
)]
public sealed class UiFlightBezierProfile : ScriptableObject
{
    [SerializeField]
    private Vector2 startControlOffset = new(0f, 220f);

    [SerializeField]
    private Vector2 endControlOffset = new(0f, 120f);

    [SerializeField]
    private float horizontalSpread = 36f;

    [SerializeField]
    private AnimationCurve progressCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private AnimationCurve scaleCurve = new(
        new Keyframe(0f, 0.85f),
        new Keyframe(0.2f, 1.05f),
        new Keyframe(0.8f, 1f),
        new Keyframe(1f, 0.72f)
    );

    [SerializeField]
    private AnimationCurve alphaCurve = new(
        new Keyframe(0f, 0f),
        new Keyframe(0.1f, 1f),
        new Keyframe(0.85f, 1f),
        new Keyframe(1f, 0f)
    );

    public Vector2 StartControlOffset => startControlOffset;

    public Vector2 EndControlOffset => endControlOffset;

    public float HorizontalSpread => horizontalSpread;

    public AnimationCurve ProgressCurve => progressCurve;

    public AnimationCurve ScaleCurve => scaleCurve;

    public AnimationCurve AlphaCurve => alphaCurve;
}
