using UnityEditor;
using UnityEngine;

public class EditorNotesGPT : EditorWindow
{
    private string notesContent = "";
    private string lastSavedNotesContent = "";
    private Vector2 scrollPosition;

    // Add menu named "BaaWolf/EditorNotesGPT" to the Unity Editor menu
    [MenuItem("BaaWolf/EditorNotesGPT/Open Notes")]
    public static void ShowWindow()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return; // Don't show window if Unity is in play mode or about to enter play mode
        }

        // Creates a new dockable window or focus if it's already open.
        EditorWindow.GetWindow<EditorNotesGPT>("EditorNotesGPT");
    }

    void OnEnable()
    {
        // Load previously saved notes
        notesContent = EditorPrefs.GetString("MyUnityNotesGPT", "");
        lastSavedNotesContent = notesContent;
    }
    
    // Inside your EditorNotesGPT class
    private void SendChatToOpenAI()
    {
        string notesContent = "user: Hello can give me a one-line example of the ternary operator?";

        // Define the chat dialog
        string[] messages = {
            notesContent,
        };

        string model = "gpt-3.5-turbo";
        float temperature = 0.7f;
        
        // Load the Chat GPT key from EditorPrefs
        string chatGPTKey = EditorPrefs.GetString("ChatGPTKey", "");

        if (string.IsNullOrEmpty(chatGPTKey))
        {
            // Display an error dialog if the key is blank
            EditorUtility.DisplayDialog("Error", "Please specify a Chat GPT key.", "OK");
            return; // Stop further processing
        }

        string response = OpenAIChatHandler.SendChatMessage(chatGPTKey, model, messages, temperature);

        if (response != null)
        {
            // Do something with the AI's response
            notesContent += "\nAI: " + response;
        }

        Debug.Log(notesContent);
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        // Text area for notes
        notesContent = EditorGUILayout.TextArea(notesContent, GUILayout.ExpandHeight(true));

        EditorGUILayout.EndScrollView();

        // Button section
        EditorGUILayout.BeginHorizontal();

        // Save button
        if (GUILayout.Button("Save Notes"))
        {
            SaveNotes();
        }

        // Revert button
        if (GUILayout.Button("Revert"))
        {
            RevertNotes();
        }

        // Add a button to send a chat message to OpenAI
        if (GUILayout.Button("Send Chat to OpenAI"))
        {
            SendChatToOpenAI();
        }

        EditorGUILayout.EndHorizontal();
    }

    void SaveNotes()
    {
        // Save notes using EditorPrefs
        EditorPrefs.SetString("MyUnityNotesGPT", notesContent);
        lastSavedNotesContent = notesContent;
        Debug.Log("Notes Saved!");
    }

    void RevertNotes()
    {
        // Revert notes to last saved content
        notesContent = lastSavedNotesContent;

        // Indicate that GUI has changed
        GUI.changed = true;

        // Force focus to a null control, making TextArea lose focus and potentially update its content
        GUI.FocusControl(null);
    }

    void OnDisable()
    {
        // Automatically save when the window is closed
        SaveNotes();
    }
}
