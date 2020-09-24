using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GameCondition {

  // Specific actor
  // Item owned by current actor
  // Interactable active
  public Chars actor;
  public Skill skill;
  public ItemEnum item;
  public int num;
  public Condition condition;
  public ActionEnum action;

  internal string Verify(Actor performer, GameItem obj) {
    Actor a = Controller.GetActor(actor);
    switch (condition) {
      case Condition.None: return null;
      case Condition.CurrentActorEqual: return (actor == performer.id) ? null : "I can't";
      case Condition.CurrentActorNotEqual: return (actor != performer.id) ? null : "I can't";
      case Condition.ActorIsAvailable: if (Controller.WeHaveActorPlaying(actor)) return null; else return "Actor not available";
      case Condition.ActorHasSkill:  return a == null ? "Invalid actor for condition" : a.HasSkill(skill);
      case Condition.HasItem: return a == null ? "Invalid actor for condition" : a.HasItem(item);
      case Condition.DoesNotHaveItem: return a == null ? "Invalid actor for condition" : (a.HasItem(item) == null ? "Has item " + item : null);
      case Condition.ItemIsOpen: return (obj.Usable == Tstatus.OpenableOpen) ? null : "It is closed";
      case Condition.ItemIsClosed: return (obj.Usable == Tstatus.OpenableClosed || obj.Usable == Tstatus.OpenableLocked) ? null : "It is open";
      case Condition.ItemIsLocked: return (obj.Usable == Tstatus.OpenableLocked) ? null : "It is unlocked";
      case Condition.ItemIsUnlocked: return (obj.Usable == Tstatus.OpenableOpen || obj.Usable == Tstatus.OpenableClosed) ? null : "It is locked";
      case Condition.ItemIsCollected: return (obj.owner != Chars.None) ? null : "Already picked";
      case Condition.ItemIsNotCollected: return (obj.owner == Chars.None) ? null : "Not collected";
      case Condition.ActionCompleted: return Controller.ActionStatus(action) == Running.Running ? null : "Cannot start";
      case Condition.ActionNotStarted: return Controller.ActionStatus(action) == Running.NotStarted ? null : "Cannot start";
      case Condition.ActionRunning: return Controller.ActionStatus(action) == Running.Running ? null : "Cannot start";
      case Condition.WithItem: return item == obj.Item ? null : "Not valid item";
    }

    return null;
  }

  public override string ToString() {
    return CalculateName(condition, actor, skill, item, num, action);
  }

  internal static string CalculateName(Condition conditionVal, Chars actorVal, Skill skillVal, ItemEnum itemVal, int numVal, ActionEnum actionVal) {
    switch(conditionVal) {
      case Condition.None: return "No conditions";
      case Condition.CurrentActorEqual: return "Actor is " + actorVal.ToString();
      case Condition.CurrentActorNotEqual: return "Actor is NOT " + actorVal.ToString();
      case Condition.ActorIsAvailable: return " Actor  " + actorVal.ToString() + " is available";
      case Condition.ActorHasSkill: return " Actor  " + actorVal.ToString() + " has " + skillVal.ToString();
      case Condition.HasItem: return "Actor  " + actorVal.ToString() + " has " + numVal + " " + itemVal.ToString();
      case Condition.DoesNotHaveItem: return "Actor  " + actorVal.ToString() + " has NOT item " + itemVal.ToString();
      case Condition.ItemIsOpen: return "Item " + itemVal.ToString() + " is open";
      case Condition.ItemIsClosed: return "Item " + itemVal.ToString() + " is closed";
      case Condition.ItemIsLocked: return "Item " + itemVal.ToString() + " is locked";
      case Condition.ItemIsCollected: return "Item " + itemVal.ToString() + " collected";
      case Condition.ItemIsNotCollected: return "Item " + itemVal.ToString() + " not collected";
      case Condition.ItemIsUnlocked: return "Item " + itemVal.ToString() + " is unlocked";
      case Condition.ActionCompleted: return "Action " + actionVal.ToString() + " is completed";
      case Condition.ActionNotStarted: return "Action " + actionVal.ToString() + " is not started";
      case Condition.ActionRunning: return "Action " + actionVal.ToString() + " is running";
      case Condition.WithItem: return "Usabe with " + itemVal.ToString();
    }
    return conditionVal.ToString() + " NOT Implemented!";
  }
}



public enum Condition {
  None,
  CurrentActorEqual,
  CurrentActorNotEqual,
  ActorIsAvailable,
  ActorHasSkill,
  HasItem,
  DoesNotHaveItem,
  ItemIsOpen,
  ItemIsLocked,
  ItemIsUnlocked,
  ItemIsClosed,
  ActionCompleted,
  ActionNotStarted,
  ActionRunning,
  ItemIsCollected,
  ItemIsNotCollected,
  WithItem
}



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


    Chars actorVal = (Chars)actor.intValue;
    ItemEnum itemVal = (ItemEnum)item.intValue;
    Skill skillVal = (Skill)skill.intValue;
    int numVal = num.intValue;
    Condition conditionVal = (Condition)type.intValue;
    ActionEnum actionVal = (ActionEnum)action.intValue;

    string name = GameCondition.CalculateName(conditionVal, actorVal, skillVal, itemVal, numVal, actionVal);

    Rect rectCond = new Rect(position.x,       position.y, 90,                                              EditorGUIUtility.singleLineHeight);
    Rect rectType = new Rect(position.x + 90,  position.y, position.width / 3 - 50,                          EditorGUIUtility.singleLineHeight);
    Rect rectName = new Rect(position.x + 90 + position.width / 3, position.y, position.width * 2 / 3 - 50,  EditorGUIUtility.singleLineHeight);
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