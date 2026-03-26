/*
 * UI Flight 描画専用の overlay Canvas を構築する。
 * runtime service から pooled Image をぶら下げる親として使い、シーン差分を吸収する。
 */

using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public sealed class UiFlightOverlayRoot : MonoBehaviour
{
    private static readonly ProfilerMarker GetOrCreateMarker = new(
        "FG.UiFlight.OverlayRoot.GetOrCreate"
    );

    public RectTransform RootRect { get; private set; }

    public static UiFlightOverlayRoot GetOrCreate(Transform parent)
    {
        using (GetOrCreateMarker.Auto())
        {
            UiFlightOverlayRoot existing = parent.GetComponentInChildren<UiFlightOverlayRoot>(true);
            if (existing != null)
            {
                existing.EnsureInitialized();
                return existing;
            }

            var gameObject = new GameObject(
                "UiFlightOverlayRoot",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(UiFlightOverlayRoot)
            );

            gameObject.transform.SetParent(parent, false);
            var overlayRoot = gameObject.GetComponent<UiFlightOverlayRoot>();
            overlayRoot.EnsureInitialized();
            return overlayRoot;
        }
    }

    private void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        RootRect ??= GetComponent<RectTransform>();

        RootRect.anchorMin = Vector2.zero;
        RootRect.anchorMax = Vector2.one;
        RootRect.offsetMin = Vector2.zero;
        RootRect.offsetMax = Vector2.zero;
        RootRect.pivot = new Vector2(0.5f, 0.5f);

        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue - 32;
        canvas.overrideSorting = true;
    }
}
