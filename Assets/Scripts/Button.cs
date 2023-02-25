using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ButtonType
{
    NUMBER,
    FUNCTION,
    INSTANT_FUNCTION,
    DELETE,
    DECIMAL,
    CLEAR,
    EQUALS,
    MEMORY
}

public class Button : MonoBehaviour
{
    public ButtonType buttonType;
    public string buttonValue;
    void OnValidate()
    {
        transform.Find("Text").GetComponent<TextMeshPro>().text = buttonValue;
        transform.Find("Background").GetComponent<SpriteRenderer>().color = GetButtonColor();
        transform.Find("Border").GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
        gameObject.name = buttonValue;
    }
    Color GetButtonColor()
    {
        switch (buttonType)
        {
            case ButtonType.FUNCTION:
                return new Color(0.3f, 0.3f, 1f);
            case ButtonType.INSTANT_FUNCTION:
                return new Color(0f, 0.9f, 0.9f);
            case ButtonType.CLEAR:
                return new Color(1f, 0.3f, 0.3f);
            case ButtonType.EQUALS:
                return new Color(0f, 1f, 0f);
            case ButtonType.MEMORY:
                if (buttonValue == "MR") return new Color(0.6f, 0f, 0f);
                return new Color(0.5f, 0.5f, 0.5f);
            default:
                return new Color(1f, 1f, 1f);
        }
    }
}
