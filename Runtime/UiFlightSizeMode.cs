/*
 * UI Flight のサイズ解決方式を定義する。
 * 端末差を吸収したい演出と raw pixel を維持したい演出を request 単位で切り替えられるようにする。
 */

public enum UiFlightSizeMode
{
    ScreenHeightRatio,
    RawPixels,
}
