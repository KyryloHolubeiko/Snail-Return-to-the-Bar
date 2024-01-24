using UnityEngine;

[CreateAssetMenu(menuName = "WildMykola/TextSettings")]
public class TextSettings : ScriptableObject {
    public Font font;
    public int fontSize;
    public Color fontColor;
    public Color fontColorHover;
    public Color fontColorPressed;
    public Color fontColorDisabled;
}