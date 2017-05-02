 // MyScriptEditor.cs
 using UnityEditor;
 
 [CustomEditor(typeof(StasisChamber))] 
 public class StasisChamberEditor : Editor {
 
     public override void OnInspectorGUI() {
         StasisChamber myTarget = (StasisChamber) target;
         myTarget.animalTypeTag = EditorGUILayout.TagField("Animal type tag:", myTarget.animalTypeTag);
     }
 }