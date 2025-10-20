using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;


public class EventEditorWindow : EditorWindow
{
    private class EventField
    {
        public string type;
        public string name;
        public EventField(string t, string n) { type = t; name = n; }
    }

    private string structDirectoryPath = "Assets/Scripts/EventSystem/GeneratedEvents/Structs";
    private string listenerDirectoryPath = "Assets/Scripts/EventSystem/GeneratedEvents/Listeners";

    private string newEventName = "NewEvent";
    private List<EventField> fields = new ();

    [MenuItem("Window/Event Editor")]
    public static void ShowWindow()
    {
        GetWindow<EventEditorWindow>("Event Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Event", EditorStyles.boldLabel);

        EditorGUILayout.Space(10);

        newEventName = EditorGUILayout.TextField("Event Name",newEventName);

        GUILayout.Label("Fields:");
        for (int i = 0; i < fields.Count; i++)
        {
            fields[i].type = EditorGUILayout.TextField(fields[i].type);
            fields[i].name = EditorGUILayout.TextField(fields[i].name);

            if (GUILayout.Button("-"))
            {
                fields.RemoveAt(i);
            }
        }

        if (GUILayout.Button("Add Field"))
        {
            fields.Add(new EventField("type", "name"));
        }

        if (GUILayout.Button("Generate Event Struct + Listener"))
        {
            GenerateEvent(newEventName, fields);
        }

        EditorGUILayout.Space();
        GUILayout.Label("Existing Event Types", EditorStyles.boldLabel);
    }


    private void GenerateEvent(string name, List<EventField> fields)
    {
        if (!Directory.Exists(structDirectoryPath))
        {
            Directory.CreateDirectory(structDirectoryPath);
        }

        string structPath = Path.Combine(structDirectoryPath, name + ".cs");

        string structFields = string.Join("\n    ", fields.Select(f => $"public {f.type} {f.name};"));
        string code = $@"
using System;
using UnityEngine;

[System.Serializable]
public struct {name}
{{
    {structFields}
}}
";
        File.WriteAllText(structPath, code);

        AssetDatabase.Refresh();


        GenerateListener(name);
    }

    private void GenerateListener(string structName)
    {
        if (!Directory.Exists(listenerDirectoryPath))
        {
            Directory.CreateDirectory(listenerDirectoryPath);
        }

        string listenerPath = Path.Combine(listenerDirectoryPath, $"{structName}EventListener.cs");
        string code = $@"
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class {structName}UnityEvent : UnityEvent<{structName}> {{ }}

public class {structName}EventListener : EventListener<{structName}, {structName}UnityEvent> {{ }}
";
        File.WriteAllText(listenerPath, code);
        AssetDatabase.Refresh();
    }
}
