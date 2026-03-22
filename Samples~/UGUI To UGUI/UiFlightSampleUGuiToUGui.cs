/*
 * uGUI から uGUI へ飛ばす最小サンプル。
 * source と target の Image を inspector で指定し、button から Play を呼び出す。
 */

using UnityEngine;
using UnityEngine.UI;

public sealed class UiFlightSampleUGuiToUGui : MonoBehaviour
{
    [SerializeField]
    private Image sourceImage;

    [SerializeField]
    private Image targetImage;

    [SerializeField]
    private Sprite spriteOverride;

    [SerializeField]
    private UiFlightBezierProfile profile;

    [SerializeField]
    private int count = 5;

    public void Play()
    {
        if (sourceImage == null || targetImage == null)
        {
            return;
        }

        UiFlight.Play(
            new UiFlightRequest
            {
                From = UiFlightAnchor.FromRectTransform(sourceImage.rectTransform),
                To = UiFlightAnchor.FromRectTransform(targetImage.rectTransform),
                Sprite = spriteOverride != null ? spriteOverride : sourceImage.sprite,
                Size = sourceImage.rectTransform.rect.size,
                Count = count,
                Profile = profile,
            }
        );
    }
}
