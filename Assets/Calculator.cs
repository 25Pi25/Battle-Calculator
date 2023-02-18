using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    [SerializeField] GameObject button;
    void Start()
    {
        for (int i=1;i<=10;i++)
        {
            CreateButton(transform, (i % 10).ToString());
        }
        CreateButton(transform, ".");
        CreateButton(transform, "DEL");
    }

    void CreateButton(Transform childTransform, string buttonValue)
    {
        GameObject newButton = Instantiate(button, childTransform);
        newButton.GetComponent<Button>().LoadButtonInfo(buttonValue);
    }
}