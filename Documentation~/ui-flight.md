# UI Flight Package

## 概要

UI Flight は、ゲーム固有の通貨や報酬 icon を package 外の ScriptableObject と caller 側ロジックで管理しつつ、飛行演出の描画と座標解決だけを共通化するための package です。

## 推奨運用

- package 側には汎用ロジックのみを置く
- sprite、色、同時発射数、profile はゲーム側の設定 asset で管理する
- サイズは既定で `Screen.height / ReferenceScreenHeight` による比率補正を使い、同じ設定値でも端末縦サイズに対する見た目比率を揃える
- package API をゲーム中へ直接散らさず、Presentation 層の adapter から呼び出す
- FG 本体のように reflection bridge で呼ぶ場合は、package 同梱の `Runtime/link.xml` で IL2CPP stripping を抑止する

## サイズモード

- 既定は `UiFlightSizeMode.ScreenHeightRatio`
- `ReferenceScreenHeight` の既定値は `1920`
- 従来どおり raw pixel を維持したい演出だけ `UiFlightSizeMode.RawPixels` を使う

## サポート対象

- `RectTransform`
- `VisualElement`
- `Vector2 screen point`
- `Vector3 world point + Camera`

## 非対象

- `VisualElement` の live rasterization
- 3D object mesh の追従描画
- DOTween など外部 tween 依存
