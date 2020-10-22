using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameCondition))]
public class ConditionPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    SerializedProperty type = property.FindPropertyRelative("type");
    SerializedProperty id1 = property.FindPropertyRelative("id1");
    SerializedProperty iv1 = property.FindPropertyRelative("iv1");
    SerializedProperty fv1 = property.FindPropertyRelative("fv1");
    SerializedProperty sv = property.FindPropertyRelative("sv");
    SerializedProperty bv = property.FindPropertyRelative("bv");

    string name = Condition.StringName((ConditionType)type.intValue, id1.intValue, iv1.intValue, fv1.floatValue, sv.stringValue, bv.boolValue);

    float partw = (position.width - 75) / 3;

    EditorGUI.LabelField(position, "Condition: " + name, EditorStyles.boldLabel);

    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  //This will need to be adjusted based on what you are displaying
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    float h = base.GetPropertyHeight(property, label);
    float l = EditorGUIUtility.singleLineHeight;
    return h + l;

  }
}