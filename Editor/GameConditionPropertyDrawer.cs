using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameCondition))]
public class MyConditionPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);

    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;
    EditorGUIUtility.labelWidth = 50;

    SerializedProperty type = property.FindPropertyRelative("condition");
    SerializedProperty actor = property.FindPropertyRelative("actor");
    SerializedProperty item = property.FindPropertyRelative("item");
    SerializedProperty skill = property.FindPropertyRelative("skill");
    SerializedProperty num = property.FindPropertyRelative("num");
    SerializedProperty action = property.FindPropertyRelative("action");


    string actorVal = actor.enumDisplayNames[actor.intValue];
    ItemEnum itemVal = (ItemEnum)item.intValue;
    Skill skillVal = (Skill)skill.intValue;
    int numVal = num.intValue;
    Condition conditionVal = (Condition)type.intValue;
    ActionEnum actionVal = (ActionEnum)action.intValue;

    string name = GameCondition.CalculateName(conditionVal, actorVal, skillVal, itemVal, numVal, actionVal);

    Rect rectCond = new Rect(position.x, position.y, 90, EditorGUIUtility.singleLineHeight);
    Rect rectType = new Rect(position.x + 90, position.y, position.width / 3 - 50, EditorGUIUtility.singleLineHeight);
    Rect rectName = new Rect(position.x + 90 + position.width / 3, position.y, position.width * 2 / 3 - 50, EditorGUIUtility.singleLineHeight);
    Rect rect1, rect2, rect3;

    EditorGUI.LabelField(rectCond, "Condition", EditorStyles.boldLabel);
    type.intValue = EditorGUI.Popup(rectType, type.intValue, type.enumNames);
    EditorGUI.LabelField(rectName, name);

    switch ((Condition)type.intValue) {
      case Condition.None: break;

      case Condition.CurrentActorEqual:
      case Condition.CurrentActorNotEqual:
      case Condition.ActorIsAvailable:
        rect1 = new Rect(position.x + 10, position.y + EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumNames);
        break;

      case Condition.HasItem:
        rect1 = new Rect(position.x + 10, position.y + EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
        rect2 = new Rect(position.x + 10 + position.width / 3, position.y + EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
        rect3 = new Rect(position.x + 10 + 2 * position.width / 3, position.y + EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumNames);
        num.intValue = EditorGUI.IntField(rect2, "Num", item.intValue);
        item.intValue = EditorGUI.Popup(rect3, "Item", item.intValue, item.enumNames);
        break;

      case Condition.DoesNotHaveItem:
        rect1 = new Rect(position.x + 10, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
        rect2 = new Rect(position.x + 10 + position.width / 2, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumNames);
        item.intValue = EditorGUI.Popup(rect2, "Item", item.intValue, item.enumNames);
        break;

      case Condition.ActorHasSkill:
        rect1 = new Rect(position.x + 10, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
        rect2 = new Rect(position.x + 10 + position.width / 2, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumNames);
        skill.intValue = EditorGUI.Popup(rect2, "Skill", skill.intValue, skill.enumNames);
        break;

      case Condition.ItemIsOpen:
      case Condition.ItemIsClosed:
      case Condition.ItemIsLocked:
      case Condition.ItemIsUnlocked:
      case Condition.ItemIsCollected:
      case Condition.ItemIsNotCollected:
      case Condition.WithItem:
        rect2 = new Rect(position.x + 10, position.y + 1 * EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        item.intValue = EditorGUI.Popup(rect2, "Item", item.intValue, item.enumNames);
        break;

      case Condition.ActionCompleted:
      case Condition.ActionNotStarted:
      case Condition.ActionRunning:
        rect1 = new Rect(position.x + 10, position.y + 1 * EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        action.intValue = EditorGUI.Popup(rect1, "Action", action.intValue, action.enumNames);
        break;

    }

    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  //This will need to be adjusted based on what you are displaying
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    SerializedProperty type = property.FindPropertyRelative("condition");
    switch ((Condition)type.intValue) {
      case Condition.None: return EditorGUIUtility.singleLineHeight * 1.5f;
      case Condition.ActorIsAvailable: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.CurrentActorEqual: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.CurrentActorNotEqual: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ActorHasSkill: return EditorGUIUtility.singleLineHeight * 3.5f;
      case Condition.HasItem: return EditorGUIUtility.singleLineHeight * 3.5f;
      case Condition.DoesNotHaveItem: return EditorGUIUtility.singleLineHeight * 3.5f;
      case Condition.ItemIsOpen: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ItemIsClosed: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ItemIsLocked: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ItemIsCollected: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ItemIsNotCollected: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ItemIsUnlocked: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.WithItem: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ActionCompleted: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ActionNotStarted: return EditorGUIUtility.singleLineHeight * 2.5f;
      case Condition.ActionRunning: return EditorGUIUtility.singleLineHeight * 2.5f;
    }
    return EditorGUIUtility.singleLineHeight * 1;
  }
}