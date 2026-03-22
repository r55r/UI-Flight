# UI Flight Package

## 概要

UI Flight は、ゲーム固有の通貨や報酬 icon を package 外の ScriptableObject と caller 側ロジックで管理しつつ、飛行演出の描画と座標解決だけを共通化するための package です。

## 推奨運用

- package 側には汎用ロジックのみを置く
- sprite、色、同時発射数、profile はゲーム側の設定 asset で管理する
- package API をゲーム中へ直接散らさず、Presentation 層の adapter から呼び出す

## サポート対象

- `RectTransform`
- `VisualElement`
- `Vector2 screen point`
- `Vector3 world point + Camera`

## 非対象

- `VisualElement` の live rasterization
- 3D object mesh の追従描画
- DOTween など外部 tween 依存
