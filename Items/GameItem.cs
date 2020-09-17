using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour {
  public ItemEnum Item;
  public string Name;

  [HideInInspector] public Chars owner;
  public Tstatus Pickable;
  public Tstatus Usable;
  public GameItem UsableWith;
  public Skill RequiredSkill;
  public Tstatus Openable;
  public Tstatus Lockable;

  public WhatItDoes whatItDoesL = WhatItDoes.Walk;
  public WhatItDoes whatItDoesR = WhatItDoes.Use;

  public Sprite yesImage;
  public Sprite noImage;
  public Sprite maskImage;
  public Sprite iconImage;

  public List<ActionAndCondition> actions;



  internal string PlayActions() {
    if (actions == null || actions.Count == 0) return null;
    string fail = null;
    bool atLeastOne = false;
    foreach (ActionAndCondition ac in actions) {
      string res = ac.Condition.Verify();
      if (res == null) {
        Controller.AddAction(ac.Action);
        atLeastOne = true;
      }
      else if (fail == null)
        fail = res;
    }
    return atLeastOne ? null : fail;
  }


}





public enum Skill {
  None,
  Strenght,
  Courage,
  Chef,
  Handyman,
  Geek,
  Nerd,
  Music,
  Writing
}


public enum Tstatus {
  NotApplicable,
  Yes,
  No
}

public enum WhatItDoes {
  Use,
  Read,
  Pick,
  Walk
}






public enum ItemEnum {
  Undefined,
  Sign,
  DoorToFront,



  FrontDoorKey,
  Grass,
  DoorBell,
  Doormat,
  Grate,
  Mailbox,
  MailboxFlag,


}

