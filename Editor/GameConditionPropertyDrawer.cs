using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameCondition))]
public class GameConditionPropertyDrawer : PropertyDrawer {
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
    SerializedProperty when = property.FindPropertyRelative("when");


    string actorVal = actor.enumDisplayNames[actor.intValue];
    ItemEnum itemVal = (ItemEnum)item.intValue;
    Skill skillVal = (Skill)skill.intValue;
    int numVal = num.intValue;
    Condition conditionVal = (Condition)type.intValue;
    ActionEnum actionVal = (ActionEnum)action.intValue;
    When whenVal = (When)when.intValue;

    string name = GameCondition.CalculateName(conditionVal, actorVal, skillVal, itemVal, numVal, actionVal, whenVal);

    float partw = (position.width - 75)/ 3;
    Rect rectCond = new Rect(position.x, position.y, partw, EditorGUIUtility.singleLineHeight);
    Rect rectType = new Rect(position.x + 75 + 0 * partw, position.y, partw, EditorGUIUtility.singleLineHeight);
    Rect rectWhen = new Rect(position.x + 75 + 1 * partw, position.y, partw, EditorGUIUtility.singleLineHeight);
    Rect rectName = new Rect(position.x + 75 + 2 * partw, position.y, partw, EditorGUIUtility.singleLineHeight);
    Rect rect1, rect2, rect3;

    EditorGUI.LabelField(rectCond, "Condition", EditorStyles.boldLabel);
    type.intValue = EditorGUI.Popup(rectType, type.intValue, type.enumDisplayNames);
    if (type.intValue != 0) {
      when.intValue = EditorGUI.Popup(rectWhen, when.intValue, when.enumDisplayNames);
      EditorGUI.LabelField(rectName, name);
    }

    switch ((Condition)type.intValue) {
      case Condition.None: break;

      case Condition.CurrentActorEqual:
      case Condition.CurrentActorNotEqual:
      case Condition.ActorIsAvailable:
      case Condition.RecipientIs:
      case Condition.RecipientIsNot:
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

      case Condition.WhenIs:
        break;

    }

    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  //This will need to be adjusted based on what you are displaying
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    float h = base.GetPropertyHeight(property, label);
    float l = EditorGUIUtility.singleLineHeight;

    SerializedProperty type = property.FindPropertyRelative("condition");
    switch ((Condition)type.intValue) {
      case Condition.None: return h + l;
      case Condition.ActorIsAvailable: return h + l;
      case Condition.CurrentActorEqual: return h + l;
      case Condition.CurrentActorNotEqual: return h + l;
      case Condition.RecipientIs: return h + l;
      case Condition.RecipientIsNot: return h + l;
      case Condition.ActorHasSkill: return h + l;
      case Condition.HasItem: return h + l;
      case Condition.DoesNotHaveItem: return h + l;
      case Condition.ItemIsOpen: return h + l;
      case Condition.ItemIsClosed: return h + l;
      case Condition.ItemIsLocked: return h + l;
      case Condition.ItemIsCollected: return h + l;
      case Condition.ItemIsNotCollected: return h + l;
      case Condition.ItemIsUnlocked: return h + l;
      case Condition.WithItem: return h + l;
      case Condition.ActionCompleted: return h + l;
      case Condition.ActionNotStarted: return h + l;
      case Condition.ActionRunning: return h + l;
      case Condition.WhenIs: return h + l;
    }
    return h + l * 5;
  }
}