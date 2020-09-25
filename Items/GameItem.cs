using System.Collections.Generic;
using UnityEditor;
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



  internal string PlayActions(Actor actor, WhatItDoes what, Item item = null) {
    if (actions == null || actions.Count == 0) return null;
    string fail = null;
    bool atLeastOne = false;
    foreach (ActionAndCondition ac in actions) {
      if (ac.Action.when != what) continue;
      string res = ac.Condition.Verify(actor, item ?? this);
      if (res == null) {
        Controller.AddAction(ac.Action);
        atLeastOne = true;
      }
      else if (fail == null)
        fail = res;
    }
    if (atLeastOne) return null;
    if (fail != null) return fail;
    return "No valid actions";
  }


  public string VerifyConditions(Actor actor) {
    if (actions == null || actions.Count == 0) return null;
    string fail = null;
    bool atLeastOne = false;
    foreach (ActionAndCondition ac in actions) {
      string res = ac.Condition.Verify(actor, this);
      if (res == null) {
        atLeastOne = true;
        break;
      }
      else if (fail == null)
        fail = res;
    }
    return atLeastOne ? null : fail;
  }

  internal bool CheckActions(Actor actor, Item other) {

    foreach(ActionAndCondition ac in actions) {
      if (ac.Condition.Verify(actor, other) == null) return true;
    }
    return false;
  }
}





