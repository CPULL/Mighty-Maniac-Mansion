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

  public GameCondition condition;
  public List<ActionAndCondition> actions;

  internal string PlayActions(Actor actor, Actor secondary, When when, Item item, out bool silentGood) {
    silentGood = false;
    if (actions == null || actions.Count == 0) return null;

    string badResult = null;
    string goodResult = null;

    foreach (ActionAndCondition ac in actions) {
      Controller.KnowAction(ac.Action);
      if (ac.Condition.IsValid(actor, secondary, item, this, when)) {
        Controller.AddAction(ac.Action, actor, secondary);
        if (string.IsNullOrEmpty(ac.Action.str)) 
          silentGood |= ac.Action.type.GoodByDefault();
        else
          goodResult = ac.Action.str;
      }
      else {
        if (badResult == null) badResult = ac.Condition.BadResult;
      }
    }
    return goodResult ?? badResult;
  }

  public bool VerifyMainCondition(Actor performer, Actor secondary, GameItem otherItem, When when) {
    return condition.IsValid(performer, secondary, this, otherItem, when);
  }

}





