/*
 * UI Flight package の既定実装を提供する。
 * 専用 overlay Canvas と Image プールを常駐させ、複数演出を軽量に再生する。
 */

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public sealed class UiFlightService : MonoBehaviour, IUiFlightService
{
    private const string DefaultServiceObjectName = "UiFlightService";

    private static UiFlightService defaultService;

    private readonly List<PooledImage> pooledImages = new();
    private UiFlightOverlayRoot overlayRoot;

    public static UiFlightService GetOrCreateDefaultService()
    {
        if (defaultService != null)
        {
            return defaultService;
        }

        defaultService = Object.FindAnyObjectByType<UiFlightService>();
        if (defaultService != null)
        {
            defaultService.EnsureInitialized();
            return defaultService;
        }

        var gameObject = new GameObject(DefaultServiceObjectName, typeof(UiFlightService));
        DontDestroyOnLoad(gameObject);
        defaultService = gameObject.GetComponent<UiFlightService>();
        defaultService.EnsureInitialized();
        return defaultService;
    }

    public void Play(UiFlightRequest request)
    {
        _ = PlayAsync(request);
    }

    public Task PlayAsync(UiFlightRequest request, CancellationToken cancellationToken = default)
    {
        if (!TryValidateRequest(request))
        {
            return Task.CompletedTask;
        }

        EnsureInitialized();

        var completionSource = new TaskCompletionSource<bool>();
        StartCoroutine(PlayRequestCoroutine(request, cancellationToken, completionSource));
        return completionSource.Task;
    }

    private void Awake()
    {
        if (defaultService != null && defaultService != this)
        {
            Destroy(gameObject);
            return;
        }

        defaultService = this;
        DontDestroyOnLoad(gameObject);
        EnsureInitialized();
    }

    private void OnDestroy()
    {
        if (defaultService == this)
        {
            defaultService = null;
        }
    }

    private void EnsureInitialized()
    {
        overlayRoot ??= UiFlightOverlayRoot.GetOrCreate(transform);
    }

    private static bool TryValidateRequest(UiFlightRequest request)
    {
        return request?.Sprite != null && request.From != null && request.To != null;
    }

    private IEnumerator PlayRequestCoroutine(
        UiFlightRequest request,
        CancellationToken cancellationToken,
        TaskCompletionSource<bool> completionSource
    )
    {
        if (request.StartDelay > 0f)
        {
            yield return WaitForSecondsOrCancellation(request.StartDelay, cancellationToken);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            completionSource.TrySetCanceled();
            yield break;
        }

        int totalCount = request.ResolveCount();
        int completedCount = 0;

        void CompleteOne()
        {
            completedCount++;
            request.OnItemCompleted?.Invoke(completedCount, totalCount);
            if (completedCount < totalCount || completionSource.Task.IsCompleted)
            {
                return;
            }

            request.OnCompleted?.Invoke();
            completionSource.TrySetResult(true);
        }

        for (int index = 0; index < totalCount; index++)
        {
            StartCoroutine(
                PlaySingleFlightCoroutine(
                    request,
                    index,
                    totalCount,
                    cancellationToken,
                    CompleteOne
                )
            );

            bool shouldWait = index < totalCount - 1 && request.SpawnInterval > 0f;
            if (shouldWait)
            {
                yield return WaitForSecondsOrCancellation(request.SpawnInterval, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                completionSource.TrySetCanceled();
                yield break;
            }
        }

        if (totalCount == 0)
        {
            request.OnCompleted?.Invoke();
            completionSource.TrySetResult(true);
        }
    }

    private IEnumerator PlaySingleFlightCoroutine(
        UiFlightRequest request,
        int index,
        int totalCount,
        CancellationToken cancellationToken,
        System.Action onCompleted
    )
    {
        var pooledImage = AcquireImage();
        pooledImage.Image.sprite = request.Sprite;
        pooledImage.Image.color = request.Tint;
        pooledImage.Image.preserveAspect = true;
        pooledImage.RectTransform.sizeDelta = request.ResolveSize();

        if (!UiFlightAnchorUtility.TryResolveScreenPosition(request.From, out Vector2 startScreen))
        {
            startScreen = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }

        Vector2 lastResolvedTarget = startScreen;
        if (UiFlightAnchorUtility.TryResolveScreenPosition(request.To, out Vector2 initialTarget))
        {
            lastResolvedTarget = initialTarget;
        }

        float duration = request.ResolveDuration();
        float elapsed = 0f;
        float spreadOffset = ResolveSpreadOffset(request.Profile, index, totalCount);
        Vector2 size = request.ResolveSize();

        while (elapsed < duration)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                ReleaseImage(pooledImage);
                yield break;
            }

            if (
                UiFlightAnchorUtility.TryResolveScreenPosition(
                    request.To,
                    out Vector2 currentTarget
                )
            )
            {
                lastResolvedTarget = currentTarget;
            }

            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            float curvedTime = UiFlightMath.EvaluateCurve(
                request.Profile?.ProgressCurve,
                normalizedTime,
                normalizedTime
            );

            Vector2 controlPoint1 =
                startScreen + ResolveStartControlOffset(request.Profile, spreadOffset);
            Vector2 controlPoint2 =
                lastResolvedTarget + ResolveEndControlOffset(request.Profile, spreadOffset);
            Vector2 screenPosition = UiFlightMath.EvaluateCubicBezier(
                startScreen,
                controlPoint1,
                controlPoint2,
                lastResolvedTarget,
                curvedTime
            );

            PositionImage(pooledImage.RectTransform, screenPosition);

            float scale = UiFlightMath.EvaluateCurve(
                request.Profile?.ScaleCurve,
                normalizedTime,
                1f
            );
            pooledImage.RectTransform.sizeDelta = size * scale;

            Color color = request.Tint;
            color.a *= UiFlightMath.EvaluateCurve(request.Profile?.AlphaCurve, normalizedTime, 1f);
            pooledImage.Image.color = color;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        PositionImage(pooledImage.RectTransform, lastResolvedTarget);
        ReleaseImage(pooledImage);
        onCompleted?.Invoke();
    }

    private IEnumerator WaitForSecondsOrCancellation(
        float seconds,
        CancellationToken cancellationToken
    )
    {
        float remaining = Mathf.Max(0f, seconds);
        while (remaining > 0f)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void PositionImage(RectTransform rectTransform, Vector2 screenPosition)
    {
        if (
            !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                overlayRoot.RootRect,
                screenPosition,
                null,
                out Vector2 localPoint
            )
        )
        {
            return;
        }

        rectTransform.anchoredPosition = localPoint;
    }

    private PooledImage AcquireImage()
    {
        for (int index = 0; index < pooledImages.Count; index++)
        {
            if (!pooledImages[index].GameObject.activeSelf)
            {
                pooledImages[index].GameObject.SetActive(true);
                return pooledImages[index];
            }
        }

        var gameObject = new GameObject(
            $"UiFlightImage_{pooledImages.Count}",
            typeof(RectTransform),
            typeof(Image)
        );

        gameObject.transform.SetParent(overlayRoot.RootRect, false);

        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        var pooledImage = new PooledImage(
            gameObject,
            rectTransform,
            gameObject.GetComponent<Image>()
        );
        pooledImages.Add(pooledImage);
        return pooledImage;
    }

    private static void ReleaseImage(PooledImage pooledImage)
    {
        pooledImage.GameObject.SetActive(false);
    }

    private static Vector2 ResolveStartControlOffset(
        UiFlightBezierProfile profile,
        float spreadOffset
    )
    {
        Vector2 baseOffset = profile != null ? profile.StartControlOffset : new Vector2(0f, 220f);
        baseOffset.x += spreadOffset;
        return baseOffset;
    }

    private static Vector2 ResolveEndControlOffset(
        UiFlightBezierProfile profile,
        float spreadOffset
    )
    {
        Vector2 baseOffset = profile != null ? profile.EndControlOffset : new Vector2(0f, 120f);
        baseOffset.x += spreadOffset * 0.5f;
        return baseOffset;
    }

    private static float ResolveSpreadOffset(
        UiFlightBezierProfile profile,
        int index,
        int totalCount
    )
    {
        float spread = profile != null ? profile.HorizontalSpread : 36f;
        return UiFlightMath.CalculateSpreadOffset(index, totalCount, spread);
    }

    private sealed class PooledImage
    {
        public PooledImage(GameObject gameObject, RectTransform rectTransform, Image image)
        {
            GameObject = gameObject;
            RectTransform = rectTransform;
            Image = image;
        }

        public GameObject GameObject { get; }

        public RectTransform RectTransform { get; }

        public Image Image { get; }
    }
}
