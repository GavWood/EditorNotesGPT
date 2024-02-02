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
    private string notesContentString;

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

    int GetNumberOfLines(string text)
    {
        // Split the text into lines
        string[] lines = text.Split('\n');

        // Return the number of lines
        return lines.Length;
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

        // Calculate the total height of the content within the text area
        float contentHeight = textAreaStyle.CalcHeight(new GUIContent(notesContentString), position.width);

        // Calculate the height of the visible area
        float visibleHeight = position.height; // Assuming 'position.height' is the height of the text area

        // Calculate the estimated line height based on the number of lines and content height
        float estimatedLineHeight = contentHeight / Mathf.Max(1, GetNumberOfLines(notesContentString));

        // Calculate the number of lines that fit in the visible area
        int visibleLines = Mathf.FloorToInt(visibleHeight / estimatedLineHeight);

        // Calculate the overlap (e.g., one line overlap)
        float overlap = estimatedLineHeight * 3; // Overlap of three lines

        // Use visibleLines to determine the scroll increment with one line overlap
        float scrollIncrement = (visibleLines - 1) * estimatedLineHeight - overlap;

        // Clamp the scroll position to ensure it stays within valid bounds
        scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0, Mathf.Max(0, contentHeight - visibleHeight));

        Event e = Event.current;
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.PageUp)
            {
                // Handle Page Up key
                scrollPosition.y -= scrollIncrement; 
                e.Use(); // Mark the event as handled
            }
            else if (e.keyCode == KeyCode.PageDown)
            {
                // Handle Page Down key
                scrollPosition.y += scrollIncrement;
                e.Use(); // Mark the event as handled
            }
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        notesContentString = notesContent.ToString();

        // Convert StringBuilder to string for TextArea display
        GUI.SetNextControlName("notesTextArea"); // Set a control name for the text area
        notesContentString = EditorGUILayout.TextArea(notesContentString, textAreaStyle, GUILayout.ExpandHeight(true));

        EditorGUILayout.EndScrollView();

        // Update StringBuilder with modified text from TextArea
        if (!string.Equals(notesContentString, notesContent.ToString()))
        {
            notesContent.Clear();
            notesContent.Append(notesContentString);
            needsRepaint = true;
        }

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
