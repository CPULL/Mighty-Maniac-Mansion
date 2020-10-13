using UnityEditor;
using UnityEngine;

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
    SerializedProperty dist = property.FindPropertyRelative("dist");

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

