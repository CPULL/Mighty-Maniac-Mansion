using UnityEngine;

[System.Serializable]
public class Condition {
  public ConditionType type;
  public int id1;
  public int iv1;
  public float fv1;
  public string sv;
  public bool bv;

  public Condition(string stype, int idi, string ids, int siv, bool sbv, string svs, float svf) {
    type = (ConditionType)System.Enum.Parse(typeof(ConditionType), stype, true);
    if (!System.Enum.IsDefined(typeof(ConditionType), type)) {
      Debug.LogError("Unknown ConditionType: \"" + stype + "\"");
    }

    if (!string.IsNullOrEmpty(ids)) {
      switch(type) {
        case ConditionType.ActorIs:
        case ConditionType.ActorHasSkill:      // ID of actor and ID of skill                                                       (ID1, IV1, BV)
        case ConditionType.ActorInSameRoom:    // ID of item                                                                        (ID1, SV, BV)
        case ConditionType.ActorDistanceLess:  // ID and dist value                                                                 (ID1, FV, BV)
        case ConditionType.ActorXLess:         // ID and dist value                                                                 (ID1, FV, BV)
        case ConditionType.RecipientIs:        // ID of actor                                                                       (ID1, BV)
          if (System.Enum.TryParse<Chars>(ids, out Chars resa)) {
            id1 = (int)resa;
          }
          break;

        case ConditionType.FlagValueIs:
          if (System.Enum.TryParse<GameFlag>(ids, out GameFlag resf)) {
            id1 = (int)resf;
          }
          break;

        case ConditionType.ItemCollected:
        case ConditionType.ItemOpen:
        case ConditionType.UsedWith:
          if (System.Enum.TryParse<ItemEnum>(ids, out ItemEnum resi)) {
            id1 = (int)resi;
          }
          break;
      }
    }
    else
      id1 = idi;


    iv1 = siv;
    sv = svs;
    bv = sbv;
    fv1 = svf;
  }

  public override string ToString() {
    return StringName(type, id1, iv1, fv1, sv, bv);
  }

  public static string StringName(ConditionType type, int id1, int iv1, float fv1, string sv, bool bv) {
    string res = "";

    switch (type) {
      case ConditionType.None: return "<none>";
      case ConditionType.ActorIs: return "Actor is " + (!bv ? "not " : "") + (Chars)id1;
      case ConditionType.ActorHasSkill: return "Actor " + (Chars)id1 + " has " + (!bv ? "not skill " : "skill ") + (Skill)iv1;
      case ConditionType.CurrentRoomIs: return "Room is " + (!bv ? "not " : "") + sv;
      case ConditionType.FlagValueIs: return "Flag " + (GameFlag)id1 + (bv ? " is true" : " is false");
      case ConditionType.StepValueIs: return "Step " + (bv ? " is " : " is not ") + iv1;
      case ConditionType.ItemCollected: return "Item " + (ItemEnum)id1 + (bv ? " is collected by " : " is not collected by ") + (Chars)iv1;
      case ConditionType.ActorInSameRoom: return "Actor " + (Chars)id1 + " is " + (!bv ? "not in " : "in ") + sv;
      case ConditionType.ActorDistanceLess: return "Actor " + (Chars)id1 + " dist " + (bv ? "< " : "> ") + fv1;
      case ConditionType.ActorXLess: return "Actor " + (Chars)id1 + " X " + (bv ? "< " : "> ") + fv1;
      case ConditionType.ItemOpen: return "Item " + (ItemEnum)id1 + (bv ? " is " : " is not ") + (iv1 == 0 ? "Open" : "Locked");
      case ConditionType.RecipientIs: return "Recipient " + (bv ? "is " : "is not ") + (Chars)id1;
      case ConditionType.WhenIs: return "When " + (bv ? "is " : "is not ") + (When)id1;

      case ConditionType.UsedWith: return "Used with " + (bv ? "" : "not ") + (ItemEnum)id1;
    }

    return res;
  }

  public bool IsValid(Chars performer, Chars receiver, ItemEnum item1, ItemEnum item2, When when, int step) {
    Actor p = Controller.GetActor(performer);
    Actor r = Controller.GetActor(receiver);
    Item i1 = AllObjects.FindItemByID(item1);
    Item i2 = AllObjects.FindItemByID(item2);
    return IsValid(p, r, i1, i2, when, step);
  }

  public bool IsValid(Chars performer, Chars receiver, Item item1, Item item2, When when, int step) {
    Actor p = Controller.GetActor(performer);
    Actor r = Controller.GetActor(receiver);
    return IsValid(p, r, item1, item2, when, step);
  }

  public bool IsValid(Actor performer, Actor receiver, ItemEnum item1, ItemEnum item2, When when, int step) {
    Item i1 = AllObjects.FindItemByID(item1);
    Item i2 = AllObjects.FindItemByID(item2);
    return IsValid(performer, receiver, i1, i2, when, step);
  }


  public bool IsValid(Actor performer, Actor receiver, Item item1, Item item2, When when, int step) {
    switch (type) {
      case ConditionType.None: return true;

      case ConditionType.ActorIs: { // We need an actor to test
        bool res;
        if ((Chars)id1 == Chars.Current) res = (GD.c.currentActor == performer || GD.c.currentActor == receiver);
        else if ((Chars)id1 == Chars.Actor1) res = (GD.c.actor1 == performer || GD.c.actor1 == receiver);
        else if ((Chars)id1 == Chars.Actor2) res = (GD.c.actor2 == performer || GD.c.actor2 == receiver);
        else if ((Chars)id1 == Chars.Actor3) res = (GD.c.actor3 == performer || GD.c.actor3 == receiver);
        else if ((Chars)id1 == Chars.KidnappedActor) res = (GD.c.kidnappedActor == performer || GD.c.kidnappedActor == receiver);
        else if ((Chars)id1 == Chars.Player) res = (GD.c.actor1 == performer || GD.c.actor2 == performer || GD.c.actor3 == performer);
        else res = performer.id == (Chars)id1;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ActorHasSkill: {
        bool res = Controller.GetActor((Chars)id1).HasSkill((Skill)iv1);
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.CurrentRoomIs: {
        bool res = sv.ToLowerInvariant().Equals(GD.c.currentRoom.ID.ToLowerInvariant());
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.FlagValueIs: {
        return AllObjects.CheckFlag((GameFlag)id1, bv ? FlagValue.Yes : FlagValue.No);
      }

      case ConditionType.StepValueIs: {
        bool res = iv1 == step;
        if (bv)
          return res;
        else
          return !res;
      }


      case ConditionType.ItemCollected: {
        bool res = true;
        // FIXME

        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ActorInSameRoom: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) return false;
        bool res = a.currentRoom.ID.ToLowerInvariant().Equals(sv.ToLowerInvariant());
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ActorDistanceLess: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) return false;
        bool res = Vector2.Distance(a.transform.position, performer.transform.position) < fv1;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ActorXLess: {
        Actor a = Controller.GetActor((Chars)id1);
        if (a == null) return false;
        bool res = a.transform.position.x < fv1;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.ItemOpen:
        if (iv1 == 0) {
          if (bv)
            return item1.IsOpen();
          else
            return !item1.IsOpen();
        }
        else {
          if (bv)
            return item1.IsLocked();
          else
            return !item1.IsLocked();
        }

      case ConditionType.RecipientIs: {
        Chars id = (Chars)id1;
        if (id == Chars.Player) {
          if (bv)
            return (GD.c.actor1 == receiver) || (GD.c.actor2 == receiver) || (GD.c.actor3 == receiver);
          else
            return (GD.c.actor1 != receiver) && (GD.c.actor2 != receiver) && (GD.c.actor3 != receiver);
        }
        if (id == Chars.Enemy) {
          if (bv)
            return (receiver.id == Chars.Fred) || (receiver.id == Chars.Edna) || (receiver.id == Chars.Ed) || (receiver.id == Chars.Ted) || (receiver.id == Chars.Edwige) || (receiver.id == Chars.GreenTentacle) || (receiver.id == Chars.PurpleTentacle) || (receiver.id == Chars.BlueTentacle) || (receiver.id == Chars.PurpleMeteor);
          else
            return (receiver.id != Chars.Fred) && (receiver.id != Chars.Edna) && (receiver.id != Chars.Ed) && (receiver.id != Chars.Ted) && (receiver.id != Chars.Edwige) && (receiver.id != Chars.GreenTentacle) && (receiver.id != Chars.PurpleTentacle) && (receiver.id != Chars.BlueTentacle) && (receiver.id != Chars.PurpleMeteor);
        }
        if (bv)
          return ((Chars)id1 == receiver.id);
        else
          return ((Chars)id1 != receiver.id);
      }

      case ConditionType.WhenIs: {
        bool res = (When)id1 == when;
        if (bv)
          return res;
        else
          return !res;
      }

      case ConditionType.UsedWith: {
        if (item2 != null) {
          if (bv)
            return ((ItemEnum)id1 == item2.Item);
          else
            return ((ItemEnum)id1 != item2.Item);
        }
        else if (item2 != null) {
          if (bv)
            return ((ItemEnum)id1 == item1.Item);
          else
            return ((ItemEnum)id1 != item1.Item);
        }
        else return false;
      }
    }
    return false;
  }
}



public enum ConditionType {
  None,
  ActorIs,            // ID of the actor like "player", "actor1", etc. And ID of the value that should be  (ID1, IV1, BV) -> BV true is actor is, false if actor should not be
  ActorHasSkill,      // ID of actor and ID of skill                                                       (ID1, IV1, BV)
  CurrentRoomIs,      // String name of the room                                                           (SV, BV)
  FlagValueIs,        // ID of flag, and value                                                             (ID1, IV1, BV)
  StepValueIs,        // ID of step and value                                                              (ID1, IV1, BV)
  ItemCollected,      // ID of item                                                                        (ID1, BV)
  ActorInSameRoom,    // ID of item                                                                        (ID1, SV, BV)
  ActorDistanceLess,  // ID and dist value                                                                 (ID1, FV, BV)
  ActorXLess,         // ID and dist value                                                                 (ID1, FV, BV)
  ItemOpen,           // ID of item, value for open, closed, locked                                        (ID1, IV1)
  RecipientIs,        // ID of actor                                                                       (ID1, BV)
  WhenIs,             // ID of action (give, pick, use, etc.)                                              (ID1, BV)
  UsedWith,           // ID of items                                                                       (ID1, BV)
}



