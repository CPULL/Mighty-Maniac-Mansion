using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Container))]
public class ContainerEditor : Editor {

  SerializedProperty ID, Name;
  SerializedProperty whatItDoesL, whatItDoesR;
  SerializedProperty Usable;
  SerializedProperty openImage, closeImage, lockImage, iconImage;
  SerializedProperty overColor, normalColor;
  SerializedProperty HotSpot, dir;
  SerializedProperty items, actions;
  SerializedProperty openSound, closeSound, lockSound, unlockSound;
  SerializedProperty openStatus, lockStatus;
  bool showDescription = false;
  SerializedProperty Description, Owner;


  void OnEnable() {
    ID = serializedObject.FindProperty("ID");
    Name = serializedObject.FindProperty("Name");
    whatItDoesL = serializedObject.FindProperty("whatItDoesL");
    whatItDoesR = serializedObject.FindProperty("whatItDoesR");
    Usable = serializedObject.FindProperty("Usable");
    openImage = serializedObject.FindProperty("openImage");
    openSound = serializedObject.FindProperty("OpenSound");
    closeImage = serializedObject.FindProperty("closeImage");
    closeSound = serializedObject.FindProperty("CloseSound");
    lockImage = serializedObject.FindProperty("lockImage");
    iconImage = serializedObject.FindProperty("iconImage");
    lockSound = serializedObject.FindProperty("LockSound");
    unlockSound = serializedObject.FindProperty("UnlockSound");
    overColor = serializedObject.FindProperty("overColor");
    normalColor = serializedObject.FindProperty("normalColor");
    HotSpot = serializedObject.FindProperty("HotSpot");
    dir = serializedObject.FindProperty("dir");
    items = serializedObject.FindProperty("containedItems");
    actions = serializedObject.FindProperty("actions");
    openStatus = serializedObject.FindProperty("openStatus");
    lockStatus = serializedObject.FindProperty("lockStatus");
    Description = serializedObject.FindProperty("Description");
    Owner = serializedObject.FindProperty("owner");
  }


  public override void OnInspectorGUI() {
    serializedObject.Update();
    float oldw = EditorGUIUtility.labelWidth;
    EditorGUIUtility.labelWidth = 40;

    // ID and Name
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(ID);
    EditorGUILayout.PropertyField(Name);
    EditorGUILayout.EndHorizontal();

    EditorGUIUtility.labelWidth = 50;

    // What does
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(whatItDoesL, new GUIContent("Click L"));
    EditorGUILayout.PropertyField(whatItDoesR, new GUIContent("Click R"));
    EditorGUILayout.EndHorizontal();

    // Usable UsableWith
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(Usable, new GUIContent("Usable"));
    EditorGUILayout.EndHorizontal();

    // Description and Owner (collapsible)
    showDescription = EditorGUILayout.Foldout(showDescription, "Description and Owner");
    if (showDescription) {
      EditorGUILayout.PropertyField(Description);
      EditorGUILayout.PropertyField(Owner);
    }

    // Open/Lock status
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 70;
    EditorGUILayout.PropertyField(openStatus, new GUIContent("Open status"));
    EditorGUILayout.PropertyField(lockStatus, new GUIContent("Lock stauts"));
    EditorGUILayout.EndHorizontal();


    // Images
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 35;
    EditorGUILayout.PropertyField(openImage, new GUIContent("Open"));
    EditorGUILayout.PropertyField(closeImage, new GUIContent("Close"));
    EditorGUILayout.PropertyField(lockImage, new GUIContent("Op2"));
    EditorGUILayout.PropertyField(iconImage, new GUIContent("Cl2"));
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
    EditorGUILayout.PropertyField(dir, new GUIContent("Dir"), GUILayout.Width(70));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.Space(30, false);
    if (GUILayout.Button("Set", GUILayout.Width(40))) {
      Item container = target as Container;
      if (container.transform.childCount == 0) {
        Debug.LogError("Missing spawn point for " + container.name);
        return;
      }
      Transform spawn = container.transform.GetChild(0);
      Debug.Log(spawn.name + " is at " + spawn.transform.position);
      container.HotSpot = spawn.transform.position;
    }
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 60;
    EditorGUILayout.PropertyField(openSound, new GUIContent("Open Snd"));
    EditorGUILayout.PropertyField(closeSound, new GUIContent("Close Snd"));
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(lockSound, new GUIContent("Lock Snd"));
    EditorGUILayout.PropertyField(unlockSound, new GUIContent("Unlock Snd"));
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.PropertyField(items);

    EditorGUILayout.PropertyField(actions);

    serializedObject.ApplyModifiedProperties();
    EditorGUIUtility.labelWidth = oldw;
  }
}


