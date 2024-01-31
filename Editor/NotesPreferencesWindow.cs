using UnityEditor;
using UnityEngine;

public class NotesPreferencesWindow : EditorWindow
{
    public string chatGPTKey = "no ChatGPT key specified"; // Default key

    [MenuItem("BaaWolf/EditorNotesGPT/Preferences")]
    public static void ShowWindow()
    {
        NotesPreferencesWindow window = GetWindow<NotesPreferencesWindow>("Editor Notes GPT Preferences");
        window.LoadPreferences(); // Load preferences when opening the window
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Notes Preferences", EditorStyles.boldLabel);

        chatGPTKey = EditorGUILayout.TextField("Chat GPT API key:", chatGPTKey);

        if (GUILayout.Button("Save Preferences"))
        {
            SavePreferences(); // Save preferences when the "Save Preferences" button is clicked
            Close();
        }
    }

    void LoadPreferences()
    {
        // Load chatGPT key from EditorPrefs
        chatGPTKey = EditorPrefs.GetString("ChatGPTKey", "");
    }

    void SavePreferences()
    {
        // Save the chatGPT key to EditorPrefs
        EditorPrefs.SetString("ChatGPTKey", chatGPTKey);
    }
}
