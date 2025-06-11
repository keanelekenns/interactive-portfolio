using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLogUI : MonoBehaviour
{
    public GameObject logItemPrefab;
    public Transform logContainer;
    public int maxMessages = 10;
    public float fadeDuration = 5f;

    private class LogEntry
    {
        public GameObject obj;
        public TextMeshProUGUI text;
        public float timestamp;
    }

    private readonly Queue<LogEntry> logEntries = new();

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        float now = Time.time;

        foreach (var entry in logEntries)
        {
            float age = now - entry.timestamp;
            float alpha = Mathf.Clamp01(1f - age / fadeDuration);

            var color = entry.text.color;
            color.a = alpha;
            entry.text.color = color;
        }

        // Clean up faded logs
        while (logEntries.Count > 0 && Time.time - logEntries.Peek().timestamp > fadeDuration)
        {
            Destroy(logEntries.Dequeue().obj);
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        var obj = Instantiate(logItemPrefab, logContainer);
        var tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = logString;
        tmp.color = Color.white;

        if (type == LogType.Warning) tmp.color = Color.yellow;
        else if (type == LogType.Error || type == LogType.Exception) tmp.color = Color.red;

        logEntries.Enqueue(new LogEntry
        {
            obj = obj,
            text = tmp,
            timestamp = Time.time
        });

        // Enforce max immediately if needed (ignores fade)
        while (logEntries.Count > maxMessages)
        {
            Destroy(logEntries.Dequeue().obj);
        }
    }
}
