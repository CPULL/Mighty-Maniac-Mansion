using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameCondition))]
public class GameConditionPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    SerializedProperty type = property.FindPropertyRelative("condition");
    SerializedProperty actor = property.FindPropertyRelative("actor");
    SerializedProperty item = property.FindPropertyRelative("item");
    SerializedProperty otherItem = property.FindPropertyRelative("otherItem");
    SerializedProperty skill = property.FindPropertyRelative("skill");
    SerializedProperty num = property.FindPropertyRelative("num");
    SerializedProperty action = property.FindPropertyRelative("action");
    SerializedProperty when = property.FindPropertyRelative("when");
    SerializedProperty BadResult = property.FindPropertyRelative("BadResult");


    Chars actorVal = (Chars)actor.intValue;
    ItemEnum itemVal = (ItemEnum)item.intValue;
    ItemEnum otherItemVal = (ItemEnum)otherItem.intValue;
    Skill skillVal = (Skill)skill.intValue;
    int numVal = num.intValue;
    Condition conditionVal = (Condition)type.intValue;
    CutsceneID actionVal = (CutsceneID)action.intValue;
    When whenVal = (When)when.intValue;

    string name = GameCondition.CalculateName(conditionVal, actorVal, skillVal, itemVal, otherItemVal, numVal, actionVal, whenVal);

    float partw = (position.width - 75) / 3;
    Rect rectCond = new Rect(position.x, position.y, partw, EditorGUIUtility.singleLineHeight);
    Rect rectType = new Rect(position.x + 75 + 0 * partw, position.y, partw, EditorGUIUtility.singleLineHeight);
    Rect rectWhen = new Rect(position.x + 75 + 1 * partw, position.y, partw, EditorGUIUtility.singleLineHeight);
    Rect rectName = new Rect(position.x + 75 + 2 * partw, position.y, partw, EditorGUIUtility.singleLineHeight);
    Rect rect1, rect2, rect3, rect4, rectResult;

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
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumDisplayNames);
        break;

      case Condition.HasItem:
        rect1 = new Rect(position.x + 10,                               position.y + EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
        rect2 = new Rect(position.x + 10 + position.width / 3,          position.y + EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
        rect3 = new Rect(position.x + 10 + 2 * position.width / 3,      position.y + EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumDisplayNames);
        num.intValue = EditorGUI.IntField(rect2, "Num", item.intValue);
        item.intValue = EditorGUI.Popup(rect3, "Item", item.intValue, item.enumDisplayNames);
        rectResult = new Rect(position.x + 10, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        EditorGUIUtility.labelWidth = 80;
        BadResult.stringValue = EditorGUI.TextField(rectResult, "Bad Result", BadResult.stringValue);
        break;

      case Condition.DoesNotHaveItem:
        rect1 = new Rect(position.x + 10, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
        rect2 = new Rect(position.x + 10 + position.width / 2, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumDisplayNames);
        item.intValue = EditorGUI.Popup(rect2, "Item", item.intValue, item.enumDisplayNames);
        rectResult = new Rect(position.x + 10, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        EditorGUIUtility.labelWidth = 80;
        BadResult.stringValue = EditorGUI.TextField(rectResult, "Bad Result", BadResult.stringValue);
        break;

      case Condition.ActorHasSkill:
        rect1 = new Rect(position.x + 10, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
        rect2 = new Rect(position.x + 10 + position.width / 2, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumDisplayNames);
        skill.intValue = EditorGUI.Popup(rect2, "Skill", skill.intValue, skill.enumDisplayNames);
        rectResult = new Rect(position.x + 10, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        EditorGUIUtility.labelWidth = 80;
        BadResult.stringValue = EditorGUI.TextField(rectResult, "Bad Result", BadResult.stringValue);
        break;

      case Condition.ItemIsOpen:
      case Condition.ItemIsClosed:
      case Condition.ItemIsLocked:
      case Condition.ItemIsUnlocked:
      case Condition.ItemIsCollected:
      case Condition.ItemIsNotCollected:
      case Condition.WithItem:
        rect2 = new Rect(position.x + 10, position.y + 1 * EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        item.intValue = EditorGUI.Popup(rect2, "Item", item.intValue, item.enumDisplayNames);
        rectResult = new Rect(position.x + 10, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        EditorGUIUtility.labelWidth = 80;
        BadResult.stringValue = EditorGUI.TextField(rectResult, "Bad Result", BadResult.stringValue);
        break;

      case Condition.WhenIs:
        break;

      case Condition.GameFlag:
        break;

      case Condition.ItemCouple:
        rect1 = new Rect(position.x + 0 * position.width / 4,    position.y + EditorGUIUtility.singleLineHeight, position.width / 4, EditorGUIUtility.singleLineHeight);
        rect2 = new Rect(position.x + 1 * position.width / 4,    position.y + EditorGUIUtility.singleLineHeight, position.width / 4, EditorGUIUtility.singleLineHeight);
        rect3 = new Rect(position.x + 2 * position.width / 4,    position.y + EditorGUIUtility.singleLineHeight, position.width / 4, EditorGUIUtility.singleLineHeight);
        rect4 = new Rect(position.x + 3 * position.width / 4,    position.y + EditorGUIUtility.singleLineHeight, position.width / 4, EditorGUIUtility.singleLineHeight);
        EditorGUIUtility.labelWidth = 40;
        item.intValue = EditorGUI.Popup(rect1, "Item", item.intValue, item.enumDisplayNames);
        otherItem.intValue = EditorGUI.Popup(rect2, "Item", otherItem.intValue, otherItem.enumDisplayNames);
        actor.intValue = EditorGUI.Popup(rect3, "Actor", actor.intValue, actor.enumDisplayNames);
        skill.intValue = EditorGUI.Popup(rect4, "Skill", skill.intValue, skill.enumDisplayNames);
        rectResult = new Rect(position.x + 10, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width - 10, EditorGUIUtility.singleLineHeight);
        EditorGUIUtility.labelWidth = 80;
        BadResult.stringValue = EditorGUI.TextField(rectResult, "Bad Result", BadResult.stringValue);
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
      case Condition.ItemIsCollected: return h + 2 * l;
      case Condition.ItemIsNotCollected: return h + 2 * l;
      case Condition.ItemIsUnlocked: return h + l;
      case Condition.WithItem: return h + 2 * l;
      case Condition.WhenIs: return h + l;
      case Condition.ItemCouple: return h + 2 * l;
      case Condition.GameFlag: return h + l;
    }
    return (h + l) * 2;
  }
}