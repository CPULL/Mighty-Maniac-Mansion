using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Behavior))]
public class BehaviorEditor : PropertyDrawer {
  private static Behavior copied = null;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    Rect labelRect = position;
    labelRect.height = EditorGUIUtility.singleLineHeight;
    //EditorGUI.PrefixLabel(labelRect, label);
    position.y += labelRect.height + EditorGUIUtility.standardVerticalSpacing;
    position.height -= EditorGUIUtility.standardVerticalSpacing + labelRect.height;

    SerializedProperty name = property.FindPropertyRelative("name");
    SerializedProperty ConditionsInOr = property.FindPropertyRelative("ConditionsInOr");
    SerializedProperty Actions = property.FindPropertyRelative("Actions");

    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUIUtility.labelWidth = 70;
    EditorGUI.indentLevel = 1;

    name.isExpanded = EditorGUI.Foldout(labelRect, name.isExpanded, name.stringValue);
    if (name.isExpanded) {
      if (GUI.Button(new Rect(labelRect.x + position.width - 100, labelRect.y, 50, EditorGUIUtility.singleLineHeight), "Copy")) {
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
        if (obj.GetType() == typeof(System.Collections.Generic.List<Behavior>) || obj.GetType().IsArray) {
          // Find the last `[` and ']'
          int pos0 = property.propertyPath.LastIndexOf('[');
          int pos1 = property.propertyPath.LastIndexOf(']');
          string sindex = property.propertyPath.Substring(pos0 + 1, pos1 - pos0 - 1);
          int.TryParse(sindex, out int index);

          copied = ((System.Collections.Generic.List<Behavior>)obj)[index];

          Debug.Log("Copy " + copied.name + " --> *" + index + "*");
        }
      }
      if (GUI.Button(new Rect(labelRect.x + position.width - 50, labelRect.y, 50, EditorGUIUtility.singleLineHeight), "Paste")) {
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
        if (obj.GetType() == typeof(System.Collections.Generic.List<Behavior>) || obj.GetType().IsArray) {
          // Find the last `[` and ']'
          int pos0 = property.propertyPath.LastIndexOf('[');
          int pos1 = property.propertyPath.LastIndexOf(']');
          string sindex = property.propertyPath.Substring(pos0 + 1, pos1 - pos0 - 1);
          int.TryParse(sindex, out int index);

          Behavior newb = ((System.Collections.Generic.List<Behavior>)obj)[index];
          newb.name = copied.name;
          newb.ConditionsInOr = new BehaviorConditionLine[copied.ConditionsInOr.Length];
          for (int i = 0; i < copied.ConditionsInOr.Length; i++) {
            newb.ConditionsInOr[i] = new BehaviorConditionLine(copied.ConditionsInOr[i]);
          }
          newb.Actions = new BehaviorAction[copied.Actions.Length];
          for (int i = 0; i < copied.Actions.Length; i++) {
            newb.Actions[i] = new BehaviorAction(copied.Actions[i]);
          }
          property.serializedObject.UpdateIfRequiredOrScript();
        }


      }
      EditorGUI.indentLevel = 2;
      EditorGUIUtility.labelWidth = 70;
      EditorGUILayout.PropertyField(name, new GUIContent(""));
      EditorGUILayout.PropertyField(ConditionsInOr);
      EditorGUILayout.PropertyField(Actions);
    }
    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }
}

[CustomPropertyDrawer(typeof(BehaviorCondition))]
public class BehaviorConditionPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    /*
    Item collected        | item |       |      | value |     
    Actor in same room    |      | actor |      | value |     
    Flag                  |      |       | flag | value |     
    Distance of actor     |      | actor |      |       | dist
     */

    SerializedProperty type = property.FindPropertyRelative("type");
    SerializedProperty item = property.FindPropertyRelative("item");
    SerializedProperty actor = property.FindPropertyRelative("actor");
    SerializedProperty flag = property.FindPropertyRelative("flag");
    SerializedProperty value = property.FindPropertyRelative("value");
    SerializedProperty dist = property.FindPropertyRelative("num");

    Rect rect1 = new Rect(position.x, position.y, position.width / 3, EditorGUIUtility.singleLineHeight);
    Rect rect2 = new Rect(position.x + 1 * position.width / 3, position.y, position.width / 3, EditorGUIUtility.singleLineHeight);
    Rect rect3 = new Rect(position.x + 2 * position.width / 3, position.y, position.width / 3, EditorGUIUtility.singleLineHeight);

    EditorGUIUtility.labelWidth = 40;
    type.intValue = EditorGUI.Popup(rect1, type.intValue, type.enumDisplayNames);

    switch ((BehaviorConditionType)type.intValue) {

      case BehaviorConditionType.ItemCollected:
        item.intValue = EditorGUI.Popup(rect2, "Item", item.intValue, item.enumDisplayNames);
        value.intValue = EditorGUI.Popup(rect3, "Val", value.intValue, value.enumDisplayNames);
        break;

      case BehaviorConditionType.ActorInSameRoom:
        actor.intValue = EditorGUI.Popup(rect2, "NPC", actor.intValue, actor.enumDisplayNames);
        value.intValue = EditorGUI.Popup(rect3, "Val", value.intValue, value.enumDisplayNames);
        break;

      case BehaviorConditionType.ActorDistanceLess:
        actor.intValue = EditorGUI.Popup(rect2, "NPC", actor.intValue, actor.enumDisplayNames);
        dist.floatValue = EditorGUI.FloatField(rect3, "<", dist.floatValue);
        break;

      case BehaviorConditionType.ActorDistanceMore:
        actor.intValue = EditorGUI.Popup(rect2, "NPC", actor.intValue, actor.enumDisplayNames);
        dist.floatValue = EditorGUI.FloatField(rect3, ">", dist.floatValue);
        break;

      case BehaviorConditionType.Flag:
        flag.intValue = EditorGUI.Popup(rect2, "Flag", flag.intValue, flag.enumDisplayNames);
        value.intValue = EditorGUI.Popup(rect3, "=", value.intValue, value.enumDisplayNames);
        break;

      case BehaviorConditionType.Timed:
        dist.floatValue = EditorGUI.FloatField(rect2, "Secs", dist.floatValue);
        break;
    }

    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  //This will need to be adjusted based on what you are displaying
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    return base.GetPropertyHeight(property, label);
  }
}

