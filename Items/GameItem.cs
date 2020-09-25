using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameItem : MonoBehaviour {
  public ItemEnum Item;
  public string Name;

  public Chars owner;
  public Tstatus Usable;
  public ItemEnum UsableWith;

  public WhatItDoes whatItDoesL = WhatItDoes.Walk;
  public WhatItDoes whatItDoesR = WhatItDoes.Use;

  [TextArea(3, 10)] public string Description;

  public Sprite openImage;
  public Sprite closeImage;
  public Sprite lockImage;
  public Sprite iconImage;
  public Texture2D cursorImage;

  public Vector2 HotSpot;
  public Dir dir;
  public Color32 overColor = new Color32(255, 255, 0, 255);
  public Color32 normalColor = new Color32(255, 255, 255, 255);

  public JustCondition condition;
  public List<ActionAndCondition> actions;

  internal string PlayActions(Actor actor, Actor secondary, When when, Item item = null) {
    if (actions == null || actions.Count == 0) return null;
    bool atLeastOne = false;
    foreach (ActionAndCondition ac in actions) {
      if (ac.Condition.IsValid(actor, secondary, item ?? this, when)) {
        // Here we may check if the action will be refused, just to stop whatever was going on
        Controller.AddAction(ac.Action);
        atLeastOne = true;
      }
    }
    if (atLeastOne) return null;
    return "It does not work";
  }


  public bool VerifyConditions(Actor performer, Actor secondary, When when) {
    // Check if the global condition is satisfied, if not return the defined message
    if (!condition.Condition.IsValid(performer, secondary, this, when))
      return true;

    if (actions == null || actions.Count == 0) return false;
    bool atLeastOne = false;
    foreach (ActionAndCondition ac in actions) {
      if (ac.Condition.IsValid(performer, secondary, this, when)) { 
        atLeastOne = true;
        break;
      }
    }
    return atLeastOne;
  }

  internal bool CheckActions(Actor actor, Item other) {

    foreach(ActionAndCondition ac in actions) {
      if (ac.Condition.Verify(actor, other) == null) return true;
    }
    return false;
  }
}





