﻿using UnityEngine;

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
        case ConditionType.ItemCouple:
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

      case ConditionType.ItemCollected:
        break;
      case ConditionType.ActorInSameRoom:
        break;
      case ConditionType.ActorDistanceLess: return "Actor " + (Chars)id1 + " dist " + (bv ? "< " : "> ") + fv1;
      case ConditionType.ActorXLess: return "Actor " + (Chars)id1 + " X " + (bv ? "< " : "> ") + fv1;

      case ConditionType.ItemOpen:
        break;
      case ConditionType.RecipientIs:
        break;
      case ConditionType.WhenIs:
        break;
      case ConditionType.ItemCouple:
        break;
    }

    return res;
  }

  public bool IsValid(Chars actor) {
    switch (type) {
      case ConditionType.None: return true;

      case ConditionType.ActorIs: { // We need an actor to test
        bool res;
        if ((Chars)id1 == Chars.Current) res = GD.c.currentActor.id == actor;
        else if ((Chars)id1 == Chars.Actor1) res = GD.actor1 == actor;
        else if ((Chars)id1 == Chars.Actor2) res = GD.actor2 == actor;
        else if ((Chars)id1 == Chars.Actor3) res = GD.actor3 == actor;
        else if ((Chars)id1 == Chars.KidnappedActor) res = GD.kidnapped == actor;
        else if ((Chars)id1 == Chars.Player) res = (GD.actor1 == actor || GD.actor2 == actor || GD.actor3 == actor);
        else res = actor == (Chars)id1;
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
        bool res = true;
        // FIXME
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
        bool res = true;
        // FIXME

        if (bv)
          return res;
        else
          return !res;
      }
      case ConditionType.ActorDistanceLess:// FIXME
        break;
      case ConditionType.ActorXLess:// FIXME
        break;
      case ConditionType.ItemOpen:// FIXME
        break;
      case ConditionType.RecipientIs:// FIXME
        break;
      case ConditionType.WhenIs:// FIXME
        break;
      case ConditionType.ItemCouple:// FIXME
        break;
    }
    return false; // FIXME
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
  ItemCouple,         // ID of two items                                                                   (ID1, IV1, BV)
}


