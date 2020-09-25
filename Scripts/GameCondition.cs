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
    return CalculateName(condition, actor.ToString(), skill, item, num, action);
  }

  public static string CalculateName(Condition conditionVal, string actorVal, Skill skillVal, ItemEnum itemVal, int numVal, ActionEnum actionVal) {
    switch(conditionVal) {
      case Condition.None: return "No conditions";
      case Condition.CurrentActorEqual: return "Actor is " + actorVal;
      case Condition.CurrentActorNotEqual: return "Actor is NOT " + actorVal;
      case Condition.ActorIsAvailable: return " Actor  " + actorVal + " is available";
      case Condition.ActorHasSkill: return " Actor  " + actorVal + " has " + skillVal.ToString();
      case Condition.HasItem: return "Actor  " + actorVal + " has " + numVal + " " + itemVal.ToString();
      case Condition.DoesNotHaveItem: return "Actor  " + actorVal + " has NOT item " + itemVal.ToString();
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



