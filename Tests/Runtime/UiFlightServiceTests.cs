/*
 * UI Flight の完了通知を検証する。
 * per-item callback が壊れるとゲーム側の逐次加算演出が成立しないため、件数通知だけを固定する。
 */

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class UiFlightServiceTests
{
    [UnityTest]
    public IEnumerator PlayAsync_InvokesOnItemCompletedForEachFlight()
    {
        UiFlightService service = UiFlightService.GetOrCreateDefaultService();
        Sprite sprite = CreateTestSprite();
        var callbacks = new List<Vector2Int>();
        bool completed = false;

        try
        {
            var task = service.PlayAsync(
                new UiFlightRequest
                {
                    From = UiFlightAnchor.FromScreenPoint(new Vector2(120f, 240f)),
                    To = UiFlightAnchor.FromScreenPoint(new Vector2(240f, 480f)),
                    Sprite = sprite,
                    Count = 3,
                    Duration = 0.01f,
                    SpawnInterval = 0f,
                    OnItemCompleted = (completedCount, totalCount) =>
                        callbacks.Add(new Vector2Int(completedCount, totalCount)),
                    OnCompleted = () => completed = true,
                }
            );

            yield return new WaitUntil(() => task.IsCompleted);

            Assert.That(task.IsCompletedSuccessfully, Is.True);
            Assert.That(callbacks, Has.Count.EqualTo(3));
            Assert.That(callbacks[0], Is.EqualTo(new Vector2Int(1, 3)));
            Assert.That(callbacks[1], Is.EqualTo(new Vector2Int(2, 3)));
            Assert.That(callbacks[2], Is.EqualTo(new Vector2Int(3, 3)));
            Assert.That(completed, Is.True);
        }
        finally
        {
            Cleanup(service, sprite);
        }
    }

    [UnityTest]
    public IEnumerator PlayAsync_DoesNotInvokeOnItemCompletedAfterCancellation()
    {
        UiFlightService service = UiFlightService.GetOrCreateDefaultService();
        Sprite sprite = CreateTestSprite();
        int callbackCount = 0;

        try
        {
            var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            var task = service.PlayAsync(
                new UiFlightRequest
                {
                    From = UiFlightAnchor.FromScreenPoint(new Vector2(120f, 240f)),
                    To = UiFlightAnchor.FromScreenPoint(new Vector2(240f, 480f)),
                    Sprite = sprite,
                    Count = 3,
                    StartDelay = 0.1f,
                    Duration = 0.01f,
                    OnItemCompleted = (_, _) => callbackCount++,
                },
                cancellationTokenSource.Token
            );

            cancellationTokenSource.Cancel();
            yield return new WaitUntil(() => task.IsCompleted);

            Assert.That(task.IsCanceled, Is.True);
            Assert.That(callbackCount, Is.EqualTo(0));
        }
        finally
        {
            Cleanup(service, sprite);
        }
    }

    private static Sprite CreateTestSprite()
    {
        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, mipChain: false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
    }

    private static void Cleanup(UiFlightService service, Sprite sprite)
    {
        if (sprite != null)
        {
            Texture2D texture = sprite.texture;
            Object.DestroyImmediate(sprite);
            if (texture != null)
            {
                Object.DestroyImmediate(texture);
            }
        }

        if (service != null)
        {
            Object.DestroyImmediate(service.gameObject);
        }
    }
}
