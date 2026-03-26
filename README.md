# Flying Gorilla UI Flight

`com.flying-gorilla-studios.ui-flight` は、画面上の任意位置から `uGUI` または `UI Toolkit` ターゲットへ `UnityEngine.UI.Image` を複製して飛ばすための Unity Package です。

## 特徴

- `RectTransform`、`VisualElement`、スクリーン座標、ワールド座標を始点・終点に指定できます。
- 専用 overlay `Canvas` 上で `Image` をプール再利用し、同時複数演出でも GC を抑えます。
- ベジェ軌道、進行カーブ、スケール、フェードを `UiFlightBezierProfile` で調整できます。
- サイズは既定で `Screen.height / 1920` の比率で補正され、端末が変わっても縦画面に対する見た目比率を揃えられます。
- package 単体で完結しており、ゲーム側は sprite や profile、演出条件だけを渡せます。
- `Runtime/link.xml` を同梱しているため、FG 側が reflection bridge で呼ぶ構成でも IL2CPP build で API が strip されません。
- `UiFlight.Warmup()` で既定 service、overlay root、pooled image 1 個を事前生成でき、初回 flight の cold start 切り分けと warmup に使えます。

## クイックスタート

```csharp
var request = new UiFlightRequest
{
    From = UiFlightAnchor.FromRectTransform(sourceImage.rectTransform),
    To = UiFlightAnchor.FromVisualElement(targetElement),
    Sprite = bananaSprite,
    Count = 6,
    Duration = 0.7f,
    SpawnInterval = 0.04f,
    SizeMode = UiFlightSizeMode.ScreenHeightRatio,
    ReferenceScreenHeight = 1920f,
    Profile = bananaProfile,
    OnItemCompleted = (completedCount, totalCount) =>
    {
        Debug.Log($"Arrived {completedCount}/{totalCount}");
    },
};

UiFlight.Play(request);
```

```csharp
UiFlight.Warmup(bananaSprite, new Vector2(96f, 96f));
```

## API

- `UiFlight`
- `IUiFlightService`
- `UiFlightRequest`
- `UiFlightAnchor`
- `UiFlightBezierProfile`
- `UiFlightAnchorUtility`
- `UiFlightMath`
- `UiFlightSizeMode`

## 導入メモ

- embedded package として `Packages/com.flying-gorilla-studios.ui-flight` に置けば、Unity は自動で読み込みます。
- 公開用リポジトリへ同期する場合は `git subtree push --prefix=Packages/com.flying-gorilla-studios.ui-flight <remote> <branch>` を使います。

## 制約

- v0.2.0 も sprite 指定方式です。`VisualElement` の見た目そのものを自動キャプチャして複製する機能は含みません。
- overlay 描画は `Screen Space - Overlay` の専用 `Canvas` に固定しています。
- `UiFlightRequest.SizeMode = UiFlightSizeMode.RawPixels` を指定すると、従来どおり端末補正なしの raw pixel サイズで描画できます。
- reflection bridge で package API を呼ぶ場合でも、同梱の `link.xml` を削除しないでください。
