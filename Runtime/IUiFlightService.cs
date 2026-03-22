/*
 * UI Flight package の公開サービス契約を定義する。
 * 呼び出し側は request を組み立て、同期または await 可能な API から演出を再生する。
 */

using System.Threading;
using System.Threading.Tasks;

public interface IUiFlightService
{
    void Play(UiFlightRequest request);

    Task PlayAsync(UiFlightRequest request, CancellationToken cancellationToken = default);
}
