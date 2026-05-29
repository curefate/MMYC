using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class XRLogCollector : MonoBehaviour
{
    public TextMeshProUGUI logText;
    public bool isLogEnabled = true;
    public bool isWarningEnabled = true;
    public bool isErrorEnabled = true;
    public bool isSimpleErrorMessage;

    private Queue<string> logQueue = new Queue<string>();
    public int maxLogs = 50;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string formatted = $"[{type}] {logString}";

        if (type == LogType.Error || type == LogType.Exception)
        {
            if (!isSimpleErrorMessage)
            {
                formatted += "\n" + stackTrace;
            }
        }

        if (!isLogEnabled && type == LogType.Log) return;
        if (!isWarningEnabled && type == LogType.Warning) return;
        if (!isErrorEnabled && (type == LogType.Error || type == LogType.Exception)) return;

        logQueue.Enqueue(formatted);

        while (logQueue.Count > maxLogs)
        {
            logQueue.Dequeue();
        }

        UpdateXRUI();
    }

    void UpdateXRUI()
    {
        logText.text = string.Join("\n", logQueue);
    }
}