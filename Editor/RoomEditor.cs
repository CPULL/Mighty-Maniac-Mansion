using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor {
  SerializedProperty ID, Name;
  SerializedProperty minL, maxR;
  SerializedProperty minY, minS;
  SerializedProperty maxY, maxS;
  SerializedProperty CameraGround;


  void OnEnable() {
    ID = serializedObject.FindProperty("ID");
    Name = serializedObject.FindProperty("RoomName");
    minL = serializedObject.FindProperty("minL");
    maxR = serializedObject.FindProperty("maxR");
    minY = serializedObject.FindProperty("minY");
    maxY = serializedObject.FindProperty("maxY");
    minS = serializedObject.FindProperty("minS");
    maxS = serializedObject.FindProperty("maxS");
    CameraGround = serializedObject.FindProperty("CameraGround");
  }

  public override void OnInspectorGUI() {
    serializedObject.Update();
    float oldw = EditorGUIUtility.labelWidth;

    // ID and Name
    EditorGUIUtility.labelWidth = 30;
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(ID);
    EditorGUIUtility.labelWidth = 80;
    EditorGUILayout.PropertyField(Name);
    EditorGUILayout.EndHorizontal();


    EditorGUIUtility.labelWidth = 70;
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(minL, new GUIContent("Min Left"));
    EditorGUILayout.PropertyField(maxR, new GUIContent("Max Right"));
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 100;
    EditorGUILayout.LabelField("Min");
    EditorGUIUtility.labelWidth = 20;
    EditorGUILayout.PropertyField(minY, new GUIContent("Y"));
    EditorGUILayout.PropertyField(minS, new GUIContent("S"));
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 100;
    EditorGUILayout.LabelField("Max");
    EditorGUIUtility.labelWidth = 20;
    EditorGUILayout.PropertyField(maxY, new GUIContent("Y"));
    EditorGUILayout.PropertyField(maxS, new GUIContent("S"));
    EditorGUILayout.EndHorizontal();

    EditorGUIUtility.labelWidth = 100;
    EditorGUILayout.PropertyField(CameraGround, new GUIContent("Camera Ground"));

    EditorGUILayout.Space();
    if (GUILayout.Button("Move camera here", GUILayout.Width(160))) {
      Vector3 pos = new Vector3((minL.floatValue + maxR.floatValue) / 2, CameraGround.floatValue, -10);
      Camera.main.transform.position = pos;
      SceneView.lastActiveSceneView.pivot = pos;
      SceneView.lastActiveSceneView.Repaint();
    }


  serializedObject.ApplyModifiedProperties();
    EditorGUIUtility.labelWidth = oldw;
  }
}


