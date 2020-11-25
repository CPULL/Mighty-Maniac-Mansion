using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class GameItemEditor : Editor {

  SerializedProperty ID, Name;
  SerializedProperty whatItDoesL, whatItDoesR;
  SerializedProperty Usable, openStatus;
  bool showDescription = false;
  SerializedProperty Description, Owner;
  SerializedProperty openImage, closeImage, lockImage, iconImage;
  SerializedProperty overColor, normalColor;
  SerializedProperty HotSpot, dir;
  SerializedProperty actions;


  void OnEnable() {
    ID = serializedObject.FindProperty("ID");
    Name = serializedObject.FindProperty("Name");
    whatItDoesL = serializedObject.FindProperty("whatItDoesL");
    whatItDoesR = serializedObject.FindProperty("whatItDoesR");
    Usable = serializedObject.FindProperty("Usable");
    openStatus = serializedObject.FindProperty("openStatus");
    Description = serializedObject.FindProperty("Description");
    Owner = serializedObject.FindProperty("owner");
    openImage = serializedObject.FindProperty("openImage");
    closeImage = serializedObject.FindProperty("closeImage");
    lockImage = serializedObject.FindProperty("lockImage");
    iconImage = serializedObject.FindProperty("iconImage");
    overColor = serializedObject.FindProperty("overColor");
    normalColor = serializedObject.FindProperty("normalColor");
    HotSpot = serializedObject.FindProperty("HotSpot");
    dir = serializedObject.FindProperty("dir");
    actions = serializedObject.FindProperty("actions");
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
    EditorGUILayout.PropertyField(openStatus, new GUIContent("  Open?"));
    EditorGUILayout.EndHorizontal();

    // Description and Owner (collapsible)
    showDescription = EditorGUILayout.Foldout(showDescription, "Description and Owner");
    if (showDescription) {
      EditorGUILayout.PropertyField(Description);
      EditorGUILayout.PropertyField(Owner);
    }

    // Images
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 15;
    EditorGUILayout.PropertyField(openImage, new GUIContent("O"));
    EditorGUILayout.PropertyField(closeImage, new GUIContent("C"));
    EditorGUILayout.PropertyField(lockImage, new GUIContent("L"));
    EditorGUIUtility.labelWidth = 30;
    EditorGUILayout.PropertyField(iconImage, new GUIContent("Icon"));
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
      Item item = target as Item;
      if (item.transform.childCount == 0) {
        Debug.LogError("Missing spawn point for " + item.name);
        return;
      }
      Transform spawn = item.transform.GetChild(0);
      Debug.Log(spawn.name + " is at " + spawn.transform.position);
      item.HotSpot = spawn.transform.position;
    }
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    EditorGUIUtility.labelWidth = 120;


    EditorGUILayout.PropertyField(actions);


    /*/ Actions detailed
    actions.isExpanded = EditorGUILayout.Foldout(actions.isExpanded, "Actions");
    if (actions.isExpanded) {
      EditorGUI.indentLevel++;
      SerializedProperty arraySizeProp = actions.FindPropertyRelative("Array.size");
      EditorGUILayout.PropertyField(arraySizeProp);

      for (int i = 0; i < arraySizeProp.intValue; i++) {
        SerializedProperty actionLine = actions.GetArrayElementAtIndex(i);
        SerializedProperty cond = actionLine.FindPropertyRelative("Condition");
        SerializedProperty act = actionLine.FindPropertyRelative("Action");
        string name = GetTargetObjectName(cond) + " -> " + GetTargetObjectName(act);
        EditorGUILayout.PropertyField(actionLine, new GUIContent(i.ToString() + ") " + name), true);
      }
      EditorGUI.indentLevel--;
    }
    */


    serializedObject.ApplyModifiedProperties();
    EditorGUIUtility.labelWidth = oldw;
  }

  private string GetTargetObjectName(SerializedProperty act) {
    string path = act.propertyPath.Replace(".Array.data[", "[");
    object obj = act.serializedObject.targetObject;
    string[] elements = path.Split('.');
    foreach (string element in elements) {
      if (element.Contains("[")) {
        string elementName = element.Substring(0, element.IndexOf("["));
        int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
        obj = GetValue_Imp(obj, elementName, index);
      }
      else {
        obj = GetValue_Imp(obj, element);
      }
    }
    if (obj == null) return "NULL";
    return obj.ToString();
  }

  private static object GetValue_Imp(object source, string name) {
    if (source == null)
      return null;
    var type = source.GetType();

    while (type != null) {
      var f = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
      if (f != null)
        return f.GetValue(source);

      var p = type.GetProperty(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
      if (p != null)
        return p.GetValue(source, null);

      type = type.BaseType;
    }
    return null;
  }

  private static object GetValue_Imp(object source, string name, int index) {
    System.Collections.IEnumerable enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
    if (enumerable == null) return null;
    var enm = enumerable.GetEnumerator();

    for (int i = 0; i <= index; i++) {
      if (!enm.MoveNext()) return null;
    }
    return enm.Current;
  }
}

