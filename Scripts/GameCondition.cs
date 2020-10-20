
using System;

[System.Serializable]
public class GameCondition {
  public OLDConditionType condition;
  public Chars actor;
  public Skill skill;
  public ItemEnum item;
  public ItemEnum otherItem;
  public int num;
  public CutsceneID action;
  public When when = When.Always;
  public string BadResult;

  internal bool VerifyCombinedItems(Actor performer, GameItem obj1, GameItem obj2, When when) {
    if (condition == OLDConditionType.None || obj1 == null || obj2 == null) return true;
    if (condition != OLDConditionType.ItemCouple) return false;
    if (when != this.when && this.when != When.Always) return false;

    if (actor != Chars.None && performer != null && actor != performer.id) return false;
    if (skill != Skill.None && performer != null && performer.HasSkill(skill) != null) return false;

    return (obj1.Item == item && obj2.Item == otherItem) || (obj2.Item == item && obj1.Item == otherItem);
  }

  internal bool IsValid(Actor performer, Actor secondary, GameItem obj1, GameItem obj2, When when) {
    if (condition == OLDConditionType.None) return true;
    if (when != this.when && this.when != When.Always) return false;

    Actor a = Controller.GetActor(actor);
    switch (condition) {
      case OLDConditionType.None: return true;
      case OLDConditionType.CurrentActorEqual: return actor == performer.id;
      case OLDConditionType.CurrentActorNotEqual: return actor != performer.id;
      case OLDConditionType.ActorIsAvailable: return Controller.WeHaveActorPlaying(actor);
      case OLDConditionType.ActorHasSkill: return a != null && a.HasSkill(skill) == null;
      case OLDConditionType.HasItem: return a != null && a.HasItem(item);
      case OLDConditionType.DoesNotHaveItem: return a == null || !a.HasItem(item);
      case OLDConditionType.ItemIsOpen: return obj1.Usable == Tstatus.OpenableOpen || obj1.Usable == Tstatus.OpenableOpenAutolock;
      case OLDConditionType.ItemIsClosed: return obj1.Usable == Tstatus.OpenableClosed || obj1.Usable == Tstatus.OpenableClosedAutolock || obj1.Usable == Tstatus.OpenableLocked || obj1.Usable == Tstatus.OpenableLockedAutolock;
      case OLDConditionType.ItemIsLocked: return obj1.Usable == Tstatus.OpenableLocked || obj1.Usable == Tstatus.OpenableLockedAutolock;
      case OLDConditionType.ItemIsUnlocked: return obj1.Usable == Tstatus.OpenableOpen || obj1.Usable == Tstatus.OpenableOpenAutolock || obj1.Usable == Tstatus.OpenableClosed || obj1.Usable == Tstatus.OpenableClosedAutolock;
      case OLDConditionType.ItemIsCollected: return Controller.IsItemCollected(item);
      case OLDConditionType.ItemIsNotCollected: return !Controller.IsItemCollected(item);
      case OLDConditionType.WithItem: return item == obj1.Item;
      case OLDConditionType.RecipientIs: return actor == secondary.id;
      case OLDConditionType.RecipientIsNot: return actor != secondary.id;
      case OLDConditionType.WhenIs: return when == this.when;
      case OLDConditionType.ItemCouple: return VerifyCombinedItems(performer, obj1, obj2, when);
      case OLDConditionType.GameFlag: return false; //  "GameFlag FIXME ";
    }
    return false;
  }

  public override string ToString() {
    return CalculateName(condition, actor, skill, item, otherItem, num, action, when);
  }

  public static string CalculateName(OLDConditionType conditionVal, Chars actorVal, Skill skillVal, ItemEnum itemVal, ItemEnum otherItemVal, int numVal, CutsceneID actionVal, When when) {
    switch(conditionVal) {
      case OLDConditionType.None: return "No conditions";
      case OLDConditionType.CurrentActorEqual: return "Actor is " + actorVal;
      case OLDConditionType.CurrentActorNotEqual: return "Actor is NOT " + actorVal;
      case OLDConditionType.ActorIsAvailable: return " Actor  " + actorVal + " is available";
      case OLDConditionType.ActorHasSkill: return " Actor  " + actorVal + " has " + skillVal.ToString();
      case OLDConditionType.HasItem: return "Actor  " + actorVal + " has " + numVal + " " + itemVal.ToString();
      case OLDConditionType.DoesNotHaveItem: return "Actor  " + actorVal + " has NOT item " + itemVal.ToString();
      case OLDConditionType.ItemIsOpen: return "Item " + itemVal.ToString() + " is open";
      case OLDConditionType.ItemIsClosed: return "Item " + itemVal.ToString() + " is closed";
      case OLDConditionType.ItemIsLocked: return "Item " + itemVal.ToString() + " is locked";
      case OLDConditionType.ItemIsCollected: return "Item " + itemVal.ToString() + " collected";
      case OLDConditionType.ItemIsNotCollected: return "Item " + itemVal.ToString() + " not collected";
      case OLDConditionType.ItemIsUnlocked: return "Item " + itemVal.ToString() + " is unlocked";
      case OLDConditionType.WithItem: return "Usabe with " + itemVal.ToString();
      case OLDConditionType.RecipientIs: return "Recipient is " + actorVal;
      case OLDConditionType.RecipientIsNot:return "Recipient is NOT " + actorVal;
      case OLDConditionType.WhenIs: return "When " + when;
      case OLDConditionType.ItemCouple: return "Items are " + itemVal + " & " + otherItemVal + (actorVal != Chars.None ? " with " + actorVal : "");
      case OLDConditionType.GameFlag: return "GameFlag FIXME ";
    }

    return conditionVal.ToString() + " NOT Implemented!";
  }
}



public enum OLDConditionType {
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
  ItemIsCollected,
  ItemIsNotCollected,
  WithItem,
  RecipientIs,
  RecipientIsNot,
  WhenIs,
  ItemCouple,



  GameFlag // FIXME

}



