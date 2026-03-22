/*
 * ワールド座標から UI Toolkit へ飛ばすサンプル。
 * pickup の transform を始点にし、HUD の VisualElement へ吸い込む構成を最小コードで示す。
 */

using UnityEngine;
using UnityEngine.UIElements;

public sealed class UiFlightSampleWorldToUiToolkit : MonoBehaviour
{
    [SerializeField]
    private Transform sourceTransform;

    [SerializeField]
    private Camera sourceCamera;

    [SerializeField]
    private UIDocument targetDocument;

    [SerializeField]
    private string targetElementName = "banana-storage-chip";

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private UiFlightBezierProfile profile;

    public void Play()
    {
        if (sourceTransform == null || targetDocument == null || sprite == null)
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
                From = UiFlightAnchor.FromWorldPoint(sourceTransform.position, sourceCamera),
                To = UiFlightAnchor.FromVisualElement(targetElement),
                Sprite = sprite,
                Count = 6,
                Profile = profile,
            }
        );
    }
}
