
using System;

[System.Serializable]
public class GameCondition {
  public Condition condition;
  public Chars actor;
  public Skill skill;
  public ItemEnum item;
  public ItemEnum otherItem;
  public int num;
  public ActionEnum action;
  public When when = When.Always;
  public string BadResult;

  internal bool VerifyCombinedItems(Actor performer, GameItem obj1, GameItem obj2, When when) {
    if (condition == Condition.None || obj1 == null || obj2 == null) return true;
    if (condition != Condition.ItemCouple) return false;
    if (when != this.when && this.when != When.Always) return false;

    if (actor != Chars.None && performer != null && actor != performer.id) return false;
    if (skill != Skill.None && performer != null && performer.HasSkill(skill) != null) return false;

    return (obj1.Item == item && obj2.Item == otherItem) || (obj2.Item == item && obj1.Item == otherItem);
  }

  internal bool IsValid(Actor performer, Actor secondary, GameItem obj1, GameItem obj2, When when) {
    if (condition == Condition.None) return true;
    if (when != this.when && this.when != When.Always) return false;

    Actor a = Controller.GetActor(actor);
    switch (condition) {
      case Condition.None: return true;
      case Condition.CurrentActorEqual: return actor == performer.id;
      case Condition.CurrentActorNotEqual: return actor != performer.id;
      case Condition.ActorIsAvailable: return Controller.WeHaveActorPlaying(actor);
      case Condition.ActorHasSkill: return a != null && a.HasSkill(skill) == null;
      case Condition.HasItem: return a != null && a.HasItem(item);
      case Condition.DoesNotHaveItem: return a == null || !a.HasItem(item);
      case Condition.ItemIsOpen: return obj1.Usable == Tstatus.OpenableOpen || obj1.Usable == Tstatus.OpenableOpenAutolock;
      case Condition.ItemIsClosed: return obj1.Usable == Tstatus.OpenableClosed || obj1.Usable == Tstatus.OpenableClosedAutolock || obj1.Usable == Tstatus.OpenableLocked || obj1.Usable == Tstatus.OpenableLockedAutolock;
      case Condition.ItemIsLocked: return obj1.Usable == Tstatus.OpenableLocked || obj1.Usable == Tstatus.OpenableLockedAutolock;
      case Condition.ItemIsUnlocked: return obj1.Usable == Tstatus.OpenableOpen || obj1.Usable == Tstatus.OpenableOpenAutolock || obj1.Usable == Tstatus.OpenableClosed || obj1.Usable == Tstatus.OpenableClosedAutolock;
      case Condition.ItemIsCollected: return Controller.IsItemCollected(item);
      case Condition.ItemIsNotCollected: return !Controller.IsItemCollected(item);
      case Condition.ActionCompleted: return Controller.ActionStatus(action) == Running.Completed;
      case Condition.ActionNotStarted: return Controller.ActionStatus(action) == Running.NotStarted;
      case Condition.ActionRunning: return Controller.ActionStatus(action) == Running.Running;
      case Condition.WithItem: return item == obj1.Item;
      case Condition.RecipientIs: return actor == secondary.id;
      case Condition.RecipientIsNot: return actor != secondary.id;
      case Condition.WhenIs: return when == this.when;
      case Condition.ItemCouple: return VerifyCombinedItems(performer, obj1, obj2, when);
    }
    return false;
  }

  public override string ToString() {
    return CalculateName(condition, actor, skill, item, otherItem, num, action, when);
  }

  public static string CalculateName(Condition conditionVal, Chars actorVal, Skill skillVal, ItemEnum itemVal, ItemEnum otherItemVal, int numVal, ActionEnum actionVal, When when) {
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
      case Condition.RecipientIs: return "Recipient is " + actorVal;
      case Condition.RecipientIsNot:return "Recipient is NOT " + actorVal;
      case Condition.WhenIs: return "When " + when;
      case Condition.ItemCouple: return "Items are " + itemVal + " & " + otherItemVal + (actorVal != Chars.None ? " with " + actorVal : "");
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
  WithItem,
  RecipientIs,
  RecipientIsNot,
  WhenIs,
  ItemCouple,
}



