/*
 * uGUI から UI Toolkit へ飛ばすサンプル。
 * targetElementName で VisualElement を引き、UI Toolkit ターゲット解決の最小形を示す。
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public sealed class UiFlightSampleUGuiToUiToolkit : MonoBehaviour
{
    [SerializeField]
    private Image sourceImage;

    [SerializeField]
    private UIDocument targetDocument;

    [SerializeField]
    private string targetElementName = "banana-storage-chip";

    [SerializeField]
    private Sprite spriteOverride;

    [SerializeField]
    private UiFlightBezierProfile profile;

    public void Play()
    {
        if (sourceImage == null || targetDocument == null)
        {
            return;
        }

        VisualElement targetElement = targetDocument.rootVisualElement?.Q(targetElementName);
        if (targetElement == null)
        {
            return;
        }

        UiFlight.Play(
            new UiFlightRequest
            {
                From = UiFlightAnchor.FromRectTransform(sourceImage.rectTransform),
                To = UiFlightAnchor.FromVisualElement(targetElement),
                Sprite = spriteOverride != null ? spriteOverride : sourceImage.sprite,
                Size = sourceImage.rectTransform.rect.size,
                Count = 4,
                Profile = profile,
            }
        );
    }
}
