using UnityEditor;

public class NotesMenu
{
    [MenuItem("BaaWolf/EditorNotesGPT/Open Notes")]
    public static void OpenNotes()
    {
        EditorNotesGPT.ShowWindow();
    }

    [MenuItem("BaaWolf/EditorNotesGPT/Preferences")]
    public static void OpenNotesPreferences()
    {
        NotesPreferencesWindow.ShowWindow();
    }
}
