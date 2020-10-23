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

  public List<ActionAndCondition> actions;

  internal string PlayActions(Actor actor, Actor secondary, When when, Item item, out bool silentGood) {
    silentGood = false;
    if (actions == null || actions.Count == 0) return null;

    string badResult = null;
    string goodResult = null;

    foreach (ActionAndCondition ac in actions) {
      Controller.KnowAction(ac.Action);
      if (ac.Condition.IsValid(actor, secondary, item.Item, this.Item, when, 0)) {
        Controller.AddAction(ac.Action, actor, secondary);
        if (ac.Action.type.GoodByDefault())
          silentGood = true;
        else if (!string.IsNullOrEmpty(ac.Action.str))
          goodResult = ac.Action.str;
      }
      else {
        if (badResult == null) badResult = "FIXME ac.Condition.BadResult";
      }
    }
    return goodResult ?? badResult;
  }

}





