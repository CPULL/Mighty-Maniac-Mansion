using UnityEditor;
using UnityEngine;

public class Door : Item {
  public Room src;
  public Room dst;
  public Vector3 camerapos;
  public Door correspondingDoor;
  public TransitionType transition;

  private void Start() {
    sr.color = normalColor;
  }
}





[CustomEditor(typeof(Door))]
public class DoorEditor : Editor {

  SerializedProperty itemEnum, Name;
  SerializedProperty whatItDoesL, whatItDoesR;
  SerializedProperty Openable, Lockable;
  SerializedProperty Usable, UsableWith;
  SerializedProperty Skill;
  SerializedProperty yesImage, noImage;
  SerializedProperty overColor, normalColor;
  SerializedProperty HotSpot, dir;
  SerializedProperty src, dst, correspondingDoor;
  SerializedProperty camerapos, transition;
  SerializedProperty actions;


  void OnEnable() {
    itemEnum = serializedObject.FindProperty("Item");
    Name = serializedObject.FindProperty("Name");
    whatItDoesL = serializedObject.FindProperty("whatItDoesL");
    whatItDoesR = serializedObject.FindProperty("whatItDoesR");
    Openable = serializedObject.FindProperty("Openable");
    Lockable = serializedObject.FindProperty("Lockable");
    Usable = serializedObject.FindProperty("Usable");
    UsableWith = serializedObject.FindProperty("UsableWith");
    Skill = serializedObject.FindProperty("RequiredSkill");
    yesImage = serializedObject.FindProperty("yesImage");
    noImage = serializedObject.FindProperty("noImage");
    overColor = serializedObject.FindProperty("overColor");
    normalColor = serializedObject.FindProperty("normalColor");
    HotSpot = serializedObject.FindProperty("HotSpot");
    dir = serializedObject.FindProperty("dir");
    src = serializedObject.FindProperty("src");
    dst = serializedObject.FindProperty("dst");
    camerapos = serializedObject.FindProperty("camerapos");
    correspondingDoor = serializedObject.FindProperty("correspondingDoor");
    transition = serializedObject.FindProperty("transition");
    actions = serializedObject.FindProperty("actions");
  }

  public override void OnInspectorGUI() {
    serializedObject.Update();
    float oldw = EditorGUIUtility.labelWidth;
    EditorGUIUtility.labelWidth = 40;

    // ID and Name
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(itemEnum);
    EditorGUILayout.PropertyField(Name);
    EditorGUILayout.EndHorizontal();

    EditorGUIUtility.labelWidth = 50;

    // What does
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(whatItDoesL, new GUIContent("Click L"));
    EditorGUILayout.PropertyField(whatItDoesR, new GUIContent("Click R"));
    EditorGUILayout.EndHorizontal();

    // Openable Lockable Skill
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(Openable, new GUIContent("Open"));
    EditorGUILayout.PropertyField(Lockable, new GUIContent("Lock"));
    EditorGUIUtility.labelWidth = 60;
    EditorGUILayout.PropertyField(Skill, new GUIContent("Req Skill"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    // Usable UsableWith
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(Usable, new GUIContent("Usable"));
    EditorGUILayout.PropertyField(UsableWith, new GUIContent("  with"));
    EditorGUILayout.EndHorizontal();

    // Images
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 24;
    EditorGUILayout.PropertyField(yesImage, new GUIContent("Yes"));
    EditorGUILayout.PropertyField(noImage, new GUIContent("No"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    // Over and Normal colors
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 50;
    EditorGUILayout.PropertyField(overColor, new GUIContent("Over"));
    EditorGUILayout.PropertyField(normalColor, new GUIContent("Normal"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    // HotSpot
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 50;
    EditorGUILayout.PropertyField(HotSpot, new GUIContent("HotSpot"));
    EditorGUIUtility.labelWidth = 20;
    EditorGUILayout.PropertyField(dir, new GUIContent("Dir"), GUILayout.Width(100));
    EditorGUILayout.Space(30, false);
    if (GUILayout.Button("Set", GUILayout.Width(40))) {
      Debug.Log("Set hotspot");
    }
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    // Rooms
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 50;
    EditorGUILayout.PropertyField(src, new GUIContent("Src Room"));
    EditorGUILayout.PropertyField(dst, new GUIContent("Dst Room"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 50;
    EditorGUILayout.PropertyField(correspondingDoor, new GUIContent("Door"));
    EditorGUILayout.PropertyField(camerapos, new GUIContent("Cam"));
    EditorGUILayout.PropertyField(transition, new GUIContent("Trans"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();


    EditorGUILayout.PropertyField(actions);

    serializedObject.ApplyModifiedProperties();
    EditorGUIUtility.labelWidth = oldw;
  }
}


