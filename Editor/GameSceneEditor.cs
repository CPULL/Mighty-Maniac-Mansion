
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameScene))]
[CanEditMultipleObjects]
public class GameSceneEditor : Editor {

  SerializedProperty Id, Name;


  void OnEnable() {
    Id = serializedObject.FindProperty("id");
    Name = serializedObject.FindProperty("name");
    Debug.Log("fuck");
  }


  public override void OnInspectorGUI() {
    serializedObject.Update();
    float oldw = EditorGUIUtility.labelWidth;
    EditorGUIUtility.labelWidth = 40;

    // ID and Name
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(Id);
    EditorGUILayout.PropertyField(Name);
    EditorGUILayout.EndHorizontal();

    EditorGUIUtility.labelWidth = 50;


    serializedObject.ApplyModifiedProperties();
    EditorGUIUtility.labelWidth = oldw;
  }


}
