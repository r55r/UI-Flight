/*
 * UI Flight package の既定サービスへのアクセスポイントを提供する。
 * シンプルな利用箇所では static API だけで演出を再生できるようにする。
 */

using System.Threading;
using System.Threading.Tasks;

public static class UiFlight
{
    public static IUiFlightService DefaultService => UiFlightService.GetOrCreateDefaultService();

    public static void Play(UiFlightRequest request)
    {
        DefaultService.Play(request);
    }

    public static Task PlayAsync(
        UiFlightRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return DefaultService.PlayAsync(request, cancellationToken);
    }
}
