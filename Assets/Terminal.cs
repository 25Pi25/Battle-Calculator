using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Data;
public class Terminal : MonoBehaviour
{
    List<string> operationQueue = new List<string> { "0" };
    void Start()
    {
        operationQueue.Capacity = 3;
    }
    bool LookingForNumber() { return operationQueue.Count % 2 == 0; }
    public void AppendItem(ButtonType buttonType, string buttonValue)
    {
        string lastItem = operationQueue[operationQueue.Count - 1];

        switch (buttonType)
        {
            case ButtonType.CLEAR:
                operationQueue.Clear();
                operationQueue.Add("0");
                break;
            case ButtonType.EQUALS:
                EvaluateOperation();
                break;
            case ButtonType.INSTANT_FUNCTION:
                operationQueue[0] =
                    ApplyInstantFunction(buttonValue, EvaluateOperation()).ToString();
                EvaluateOperation();
                break;
        }

        if (!LookingForNumber())
            appendButtonFromNumber(lastItem, buttonType, buttonValue);
        else
            appendButtonFromFunction(lastItem, buttonType, buttonValue);

        UpdateQueueDisplay();
    }

    void appendButtonFromNumber(string lastItem, ButtonType buttonType, string buttonValue)
    {
        switch (buttonType)
        {
            case ButtonType.NUMBER:
                if (lastItem == "0." && buttonValue != "0")
                    AddLastItem(buttonValue);
                else
                    SetLastItem(buttonValue);
                break;

            case ButtonType.FUNCTION:
                if (lastItem == "0.")
                    SetLastItem("0");
                AddToQueue(buttonValue);
                break;

            case ButtonType.DECIMAL:
                SetLastItem("0.");
                break;

            case ButtonType.DELETE:
                if (operationQueue.Count == 1)
                    operationQueue[0] = "0";
                else if (lastItem != "0")
                    SetLastItem("0");
                else
                    operationQueue.RemoveAt(operationQueue.Count - 1);
                break;
        }
    }
    void appendButtonFromFunction(string lastItem, ButtonType buttonType, string buttonValue)
    {
        switch (buttonType)
        {
            case ButtonType.NUMBER:
                AddToQueue(buttonValue);
                break;

            case ButtonType.FUNCTION:
                SetLastItem(buttonValue);
                break;

            case ButtonType.DECIMAL:
                AddToQueue("0.");
                break;

            case ButtonType.DELETE:
                operationQueue.RemoveAt(operationQueue.Count - 1);
                break;
        }
    }

    float ApplyInstantFunction(string buttonValue, float x)
    {
        switch (buttonValue)
        {
            case "1/x":
                return 1 / x;
            case "+/-":
                return x * -1;
            default:
                return x;
        }
    }

    void AddToQueue(string value)
    {
        if (operationQueue.Count == operationQueue.Capacity)
            EvaluateOperation();
        operationQueue.Add(value);
    }
    float EvaluateOperation()
    {
        if (LookingForNumber())
            operationQueue.RemoveAt(operationQueue.Count - 1);

        string toEval = string.Join(" ", operationQueue);
        try
        {
            float result = Convert.ToSingle(new DataTable().Compute(toEval, ""));
            result = float.IsNaN(result) || float.IsInfinity(result) ? 0 :
                Mathf.Round(result * 100) / 100f;

            operationQueue.Clear();
            operationQueue.Add(result.ToString());
            return result;
        }
        catch
        {
            return 0f;
        }
    }

    void SetLastItem(string value) { operationQueue[operationQueue.Count - 1] = value; }
    void AddLastItem(string value) { operationQueue[operationQueue.Count - 1] += value; }

    void UpdateQueueDisplay()
    {
        GameObject.Find("Queue").gameObject.GetComponent<TextMeshPro>().text = string.Join(" ", operationQueue);
    }
}
