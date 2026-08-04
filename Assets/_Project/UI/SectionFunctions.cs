using System;
using System.Collections.Generic;
using UnityEngine;

// Lookup for section contents of type "function" (see DisplaySections.json).
// A section with a function content runs it when clicked instead of opening
// an information modal.
public class SectionFunctions : MonoBehaviour
{
    public static SectionFunctions Instance;
    public MatrixModeController matrixMode;

    private readonly Dictionary<string, Action<Section>> functionLookup = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            functionLookup.Add("toggleMatrixMode", section => matrixMode.Toggle(section));
        }
    }

    public void Call(string functionName, Section caller)
    {
        if (functionLookup.TryGetValue(functionName, out var function))
        {
            function(caller);
        }
        else
        {
            Debug.LogWarning($"Function '{functionName}' not found.");
        }
    }
}
