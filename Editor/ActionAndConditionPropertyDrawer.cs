using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ActionAndCondition))]
public class ActionAndConditionDrawer : PropertyDrawer {

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    SerializedProperty Condition = property.FindPropertyRelative("Condition");
    SerializedProperty Action = property.FindPropertyRelative("Action");

    Item it = property.serializedObject.targetObject as Item;
    List<ActionAndCondition> actions = it.actions;

    int sbo = property.propertyPath.LastIndexOf('[') + 1;
    int sbc = property.propertyPath.LastIndexOf(']');
    string indexpath = property.propertyPath.Substring(sbo, sbc - sbo);
    int.TryParse(indexpath, out int index);
    string name = index + ") " + actions[index].Condition.ToString() + " -> " + actions[index].Action.ToString();

    float w4 = position.width * .25f;
    float lh = EditorGUIUtility.singleLineHeight;

    Rect titleR = new Rect(position.x, position.y, position.width, lh);
    EditorGUI.LabelField(titleR, "A&C: " + name, EditorStyles.boldLabel);
    property.isExpanded = EditorGUI.Foldout(titleR, property.isExpanded, new GUIContent(""));

    if (property.isExpanded) {
      Rect cRect = new Rect(position.x + 30, position.y + 1 * lh, position.width - 30, 2 * lh);
      EditorGUI.PropertyField(cRect, property.FindPropertyRelative("Condition"));
      Rect aRect = new Rect(position.x + 30, position.y + 3 * lh, position.width - 30, 2 * lh);
      EditorGUI.PropertyField(aRect, property.FindPropertyRelative("Action"));
    }


    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    float h = base.GetPropertyHeight(property, label);
    if (property.isExpanded) return h + 4 * EditorGUIUtility.singleLineHeight;
    return h;
  }
}