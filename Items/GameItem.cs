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

  internal bool PlayActions(Actor actor, Actor secondary, When when, Item item = null) {
    if (actions == null || actions.Count == 0) return false;
    bool atLeastOne = false;
    foreach (ActionAndCondition ac in actions) {
      Controller.KnowAction(ac.Action);
      if (ac.Condition.IsValid(actor, secondary, item ?? this, when)) {
        Controller.AddAction(ac.Action, actor, secondary, item);
        atLeastOne = true;
      }
    }
    return atLeastOne;
  }

  public bool VerifyMainCondition(Actor performer, Actor secondary, When when) {
    return condition.Condition.IsValid(performer, secondary, this, when);
  }

  internal bool CheckCombinedActions(Actor actor, Item other) {
    foreach(ActionAndCondition ac in actions) {
      return ac.Condition.VerifyCombinedItems(actor, other, When.Use);
    }
    return false;
  }
}





