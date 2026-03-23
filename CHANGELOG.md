# Changelog

## [0.3.1] - 2026-03-23

- `Runtime/link.xml` を追加し、FG 側が `UiFlightBridge` から reflection で呼ぶ API を IL2CPP build でも strip しないよう修正。
- Editor では見えるのに実機だけバナナ / ジェム flight が消える症状を、package 側の preservation 設定で塞いだ。

## [0.3.0] - 2026-03-23

- `UiFlightSizeMode` と `ReferenceScreenHeight` を追加し、既定で screen-height 比率に応じたサイズ補正を行うように変更。
- `UiFlightMath.ResolveScaledSize()` と runtime tests を追加し、端末差と raw pixel 切替の回帰を固定。

## [0.1.0] - 2026-03-22

- `RectTransform` / `VisualElement` / スクリーン座標 / ワールド座標対応の UI Flight package を初期実装。
- pooled overlay `Canvas`、ベジェ profile、subtree 前提の package 構成、runtime tests、sample scripts を追加。
