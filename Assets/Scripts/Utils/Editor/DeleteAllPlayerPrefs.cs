using UnityEngine;
using UnityEditor;

public class DeleteAllPlayerPrefs : ScriptableObject {

    [MenuItem ("Player Prefs/Clear all Player Preferences")]
    static void deleteAllExample() {
        if(EditorUtility.DisplayDialog("Delete all player preferences.",
            "Are you sure you want to delete all the player preferences? " +
            "This action cannot be undone.", "Yes", "No")){
                Debug.Log("All preferences deleted");
                PlayerPrefs.DeleteAll();
            }
    }
}