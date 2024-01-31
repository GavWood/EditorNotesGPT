using UnityEditor;
using UnityEngine;
using System.Text; // Add this using directive for StringBuilder

public class EditorNotesGPT : EditorWindow
{
    private StringBuilder notesContent = new StringBuilder();
    private string lastSavedNotesContent = "";
    private Vector2 scrollPosition;
    private bool wordWrap = true;
    private bool needsRepaint = false;
    private GUIStyle textAreaStyle; // Added private member for the text area style

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
        notesContent.Append(EditorPrefs.GetString("MyUnityNotesGPT", ""));
        lastSavedNotesContent = notesContent.ToString();
    }

    // Inside your EditorNotesGPT class
    private void SendChatToOpenAI()
    {
        GUI.FocusControl(null);

        // Define the chat dialog
        string[] messages = {
            notesContent.ToString(),
        };

        if (string.IsNullOrEmpty(notesContent.ToString()))
        {
            // Display an error dialog if the key is blank
            EditorUtility.DisplayDialog("Error", "Write a programming question in the notes", "OK");
            return; // Stop further processing
        }

        string model = "gpt-3.5-turbo";
        float temperature = 0.7f;

        // Load the Chat GPT key from EditorPrefs
        string chatGPTKey = EditorPrefs.GetString("ChatGPTKey", "");

        if (string.IsNullOrEmpty(chatGPTKey))
        {
            // Display an error dialog if the key is blank
            EditorUtility.DisplayDialog("Error", "Please specify a Chat GPT key", "OK");
            return; // Stop further processing
        }

        string response = OpenAIChatHandler.SendChatMessage(chatGPTKey, model, messages, temperature);

        if (response != null)
        {
            // Do something with the AI's response
            notesContent.Append("\nAI: ").Append(response);

            needsRepaint = true;
        }
        Debug.Log(notesContent.ToString());
    }

    void OnGUI()
    {
        if (textAreaStyle is null)
        {
            textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = wordWrap
            };
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        // Text area for notes with the customized style
        EditorGUILayout.TextArea(notesContent.ToString(), textAreaStyle, GUILayout.ExpandHeight(true));

        EditorGUILayout.EndScrollView();

        // This marks the horizontal buttons
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
        if (GUILayout.Button("Send Text Content to OpenAI"))
        {
            SendChatToOpenAI();
        }

        EditorGUILayout.EndHorizontal();

        // Repaint the window if needed
        if (needsRepaint)
        {
            Repaint();
            needsRepaint = false;
            scrollPosition.y = Mathf.Infinity;
        }
    }

    void SaveNotes()
    {
        // Save notes using EditorPrefs
        EditorPrefs.SetString("MyUnityNotesGPT", notesContent.ToString());
        lastSavedNotesContent = notesContent.ToString();
        Debug.Log("Notes Saved!");
    }

    void RevertNotes()
    {
        // Revert notes to last saved content
        notesContent.Clear();
        notesContent.Append(lastSavedNotesContent);

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
