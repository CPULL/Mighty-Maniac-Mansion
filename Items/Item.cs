﻿using UnityEngine;

public class Item : GameItem {
  [HideInInspector] public SpriteRenderer sr;

  private void Awake() {
    sr = GetComponent<SpriteRenderer>();
  }

  private void OnMouseEnter() {
    Controller.SetItem(this);
    sr.color = overColor;
  }
  private void OnMouseExit() {
    Controller.SetItem(null);
    sr.color = normalColor;
  }

  public string Use(Actor actor) {
    if (Usable == Tstatus.Openable && !IsOpen() && IsLocked()) return "It is locked";

    if (Usable == Tstatus.Usable) {
      ActionRes res = PlayActions(actor, null, When.Use, this);
      if (res == null || !res.actionDone) return "It does not work";
      return null;
    }
    else if (Usable == Tstatus.Openable && openStatus == OpenStatus.Open) {
      SetAsClosed(); // FIXME Handle the autolocks here
      ActionRes res = PlayActions(actor, null, When.Use, null);
      if (res != null && !res.actionDone) return res.res;
      return null;
    }
    else if (Usable == Tstatus.Openable && openStatus == OpenStatus.Closed) {
      SetAsOpen();
      ActionRes res = PlayActions(actor, null, When.Use, null);
      if (res != null && !res.actionDone) return res.res;
      return null;
    }

    return "Seems it does nothing";
  }

  internal ActionRes PlayActions(Actor actor, Actor secondary, When when, Item item) {
    if (actions == null || actions.Count == 0) return null;

    ActionRes res = null;
    Item item1 = this;
    Item item2 = (this == item ? null : item);

    foreach (ActionAndCondition ac in actions) {
      Controller.KnowAction(ac.Action);
      if (ac.Condition.IsValid(actor, secondary, item1, item2, when)) {
        ac.Action.RunAction(actor, secondary, this, item);
        if (res == null) res = new ActionRes { actionDone = true, res = null };
        if (!ac.Action.type.GoodByDefault() && !string.IsNullOrEmpty(ac.Action.msg))
          res.res = ac.Action.msg;
      }
      else if (!string.IsNullOrEmpty(ac.Condition.msg)) {
        if (res == null) res = new ActionRes();
        res.res = ac.Condition.msg;
      }
    }
    return res;
  }


  public string UseTogether(Actor actor, Item other) {
    // Case of two items
    string res = null;
    bool done = false;
    foreach (ActionAndCondition ac in actions) {
      if (ac.Condition.IsValid(actor, null, this, other, When.Use)) {
        ActionRes pares = PlayActions(actor, null, When.Use, other);
        if (pares != null) {
          done = pares.actionDone;
          res = pares.res;
        }
      }
      else if (!string.IsNullOrEmpty(ac.Condition.msg) && res == null) res = ac.Condition.msg;
    }
    if (res != null) return res;
    foreach (ActionAndCondition ac in other.actions) {
      if (ac.Condition.IsValid(actor, null, this, other, When.Use)) {
        ActionRes pares = other.PlayActions(actor, null, When.Use, this);
        if (pares != null) {
          done = pares.actionDone;
          res = pares.res;
        }
      }
      else if (!string.IsNullOrEmpty(ac.Condition.msg) && res == null) res = ac.Condition.msg;
    }
    if (!done && res != null) return res;

    // Case of an item and a container
    Container c = other as Container;
    if (c != null) {
      if (c.Usable != Tstatus.Openable && c.openStatus != OpenStatus.Open) return "It is closed";

      if (c.HasItem(this)) {
        // Put item back
        actor.inventory.Remove(this);
        transform.parent = c.transform;
        gameObject.SetActive(true);
        owner = Chars.None;
        return null;
      }
      else
        return "it does not fit";
    }



    return "It does not work...";

    // FIXME Case of an item and a door


  }


  public void ForceOpen(bool val) {
    if (Usable != Tstatus.Openable) return;
    if (val) { // Open
      SetAsOpen();
    }
    else { // Close
      SetAsClosed();
    }
  }


  public void ForceLock(bool val) {
    if (val) { // Lock
      SetAsClosed();
      SetAsLocked();
    }
    else { // Unlock
      SetAsUnlocked();
    }
  }

  internal bool HasActions(When when) {
    foreach(ActionAndCondition ac in actions) {
      if (ac.Condition.type != ConditionType.None && (When)ac.Condition.id == when) return true;
    }
    return false;
  }

  internal string GetOpenStatus() {
    if (Usable != Tstatus.Openable) return "";

    if (openStatus == OpenStatus.Open) return "Is open";
    if (lockStatus == LockStatus.Unlocked) return "Is closed";
    return "Is locked";
  }

  internal int GetOpeningStatus() {
    if (Usable != Tstatus.Openable) return -1;
    if (openStatus == OpenStatus.Open) {
      if (lockStatus == LockStatus.Unlocked) return 0;
      if (lockStatus == LockStatus.UnlockedAutolock) return 1;
      if (lockStatus == LockStatus.Locked) return 2;
      if (lockStatus == LockStatus.Autolock) return 3;
    }
    if (openStatus == OpenStatus.Closed) {
      if (lockStatus == LockStatus.Unlocked) return 4;
      if (lockStatus == LockStatus.UnlockedAutolock) return 5;
      if (lockStatus == LockStatus.Locked) return 6;
      if (lockStatus == LockStatus.Autolock) return 7;
    }
    return 0;
  }

  internal void SetOpeningStatus(int val) {
    switch (val) {
      case 0: ForceOpen(true); ForceLock(false); break;
      case 1: ForceOpen(true); ForceLock(false); break;
      case 2: ForceOpen(true); ForceLock(true); break;
      case 3: ForceOpen(true); ForceLock(true); break;

      case 4: ForceOpen(false); ForceLock(false); break;
      case 5: ForceOpen(false); ForceLock(false); break;
      case 6: ForceOpen(false); ForceLock(true); break;
      case 7: ForceOpen(false); ForceLock(true); break;
    }
  }



  internal bool IsOpen() {
    return Usable == Tstatus.Openable && openStatus != OpenStatus.Closed;
  }

  internal bool IsLocked() {
    if (Usable != Tstatus.Openable) return false;
    return lockStatus == LockStatus.Locked || lockStatus == LockStatus.Autolock;
  }

  private void SetAsOpen() {
    bool sound = openStatus == OpenStatus.Closed;
    openStatus = OpenStatus.Open;
    sr.sprite = openImage;
    Door door = this as Door;
    if (door != null) {
      if (door.correspondingDoor != null) {
        door.correspondingDoor.openStatus = OpenStatus.Open;
        door.correspondingDoor.sr.sprite = door.correspondingDoor.openImage;
      }
      if (sound && door.OpenSound != null && door.Audio != null) {
        door.Audio.clip = door.OpenSound;
        door.Audio.Play();
      }
    }
    else {
      Container c = this as Container;
      if (c != null) {
        foreach (Item item in c.items)
          if (item.owner == Chars.None)
            item.gameObject.SetActive(true);
        if (sound && c.OpenSound != null && c.Audio != null) {
          c.Audio.clip = c.OpenSound;
          c.Audio.Play();
        }
      }
    }
  }
  private void SetAsClosed() {
    bool sound = openStatus == OpenStatus.Open;
    if (openStatus == OpenStatus.Open) {
      openStatus = OpenStatus.Closed;
      sr.sprite = closeImage;
      if (lockStatus == LockStatus.UnlockedAutolock) lockStatus = LockStatus.Autolock;
    }
    Door door = this as Door;
    if (door != null) {
      if (door.correspondingDoor != null && door.correspondingDoor.openStatus == OpenStatus.Open) {
        door.correspondingDoor.openStatus = OpenStatus.Closed;
        door.correspondingDoor.sr.sprite = door.correspondingDoor.openImage;
        if (door.correspondingDoor.lockStatus == LockStatus.UnlockedAutolock) door.correspondingDoor.lockStatus = LockStatus.Autolock;
      }
      if (sound && door.OpenSound != null && door.Audio != null) {
        door.Audio.clip = door.OpenSound;
        door.Audio.Play();
      }
    }
    else {
      Container c = this as Container;
      if (c != null) {
        foreach (Item item in c.items)
          if (item.owner == Chars.None)
            item.gameObject.SetActive(true);
        if (sound && c.OpenSound != null && c.Audio != null) {
          c.Audio.clip = c.OpenSound;
          c.Audio.Play();
        }
      }
    }
  }

  private void SetAsUnlocked() {
    bool soundC = openStatus == OpenStatus.Open;
    bool soundUl = lockStatus == LockStatus.Locked || lockStatus == LockStatus.Autolock;
    if (lockStatus == LockStatus.Locked) lockStatus = LockStatus.Unlocked;
    if (lockStatus == LockStatus.Autolock) lockStatus = LockStatus.UnlockedAutolock;
    sr.sprite = closeImage;
    Door door = this as Door;
    if (door != null) {
      if (door.correspondingDoor != null) {
        if (door.correspondingDoor.openStatus == OpenStatus.Open) door.correspondingDoor.openStatus = OpenStatus.Closed;
        if (door.correspondingDoor.lockStatus == LockStatus.Locked) {
          door.correspondingDoor.lockStatus = LockStatus.Unlocked;
          door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
        }
        else if (door.correspondingDoor.lockStatus == LockStatus.Autolock) {
          door.correspondingDoor.lockStatus = LockStatus.UnlockedAutolock;
          door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
        }
        if (soundC && door.CloseSound != null && door.Audio != null) {
          door.Audio.clip = door.CloseSound;
          door.Audio.Play();
        }
        else if (soundUl && door.UnlockSound != null && door.Audio != null) {
          door.Audio.clip = door.UnlockSound;
          door.Audio.Play();
        }
      }
    }
    else {
      Container c = this as Container;
      if (c != null) {
        foreach (Item item in c.items)
          if (item.owner == Chars.None)
            item.gameObject.SetActive(false);
        if (soundC && c.CloseSound != null && c.Audio != null) {
          c.Audio.clip = c.CloseSound;
          c.Audio.Play();
        }
        else if (soundUl && c.UnlockSound != null && c.Audio != null) {
          c.Audio.clip = c.UnlockSound;
          c.Audio.Play();
        }
      }
    }
  }

  private void SetAsLocked() {
    bool soundC = openStatus == OpenStatus.Open;
    bool soundUl = lockStatus == LockStatus.Unlocked || lockStatus == LockStatus.UnlockedAutolock;
    if (lockStatus == LockStatus.Unlocked) lockStatus = LockStatus.Locked;
    if (lockStatus == LockStatus.UnlockedAutolock) lockStatus = LockStatus.Autolock;
    sr.sprite = closeImage;
    Door door = this as Door;
    if (door != null) {
      if (door.correspondingDoor != null) {
        if (door.correspondingDoor.openStatus == OpenStatus.Open) door.correspondingDoor.openStatus = OpenStatus.Closed;
        if (door.correspondingDoor.lockStatus == LockStatus.Unlocked) {
          door.correspondingDoor.lockStatus = LockStatus.Locked;
          door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
        }
        else if (door.correspondingDoor.lockStatus == LockStatus.UnlockedAutolock) {
          door.correspondingDoor.lockStatus = LockStatus.Autolock;
          door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
        }
        if (soundC && door.CloseSound != null && door.Audio != null) {
          door.Audio.clip = door.CloseSound;
          door.Audio.Play();
        }
        else if (soundUl && door.LockSound != null && door.Audio != null) {
          door.Audio.clip = door.LockSound;
          door.Audio.Play();
        }
      }
    }
    else {
      Container c = this as Container;
      if (c != null) {
        foreach (Item item in c.items)
          if (item.owner == Chars.None)
            item.gameObject.SetActive(false);
      }
    }
  }

  internal void ForceStatus(int val) {
    switch (val) {
      case 0: // Open
        ForceOpen(true);
        break;
      case 1: // Close
        ForceOpen(false);
        break;
      case 2: // Lock
        ForceLock(true);
        break;
      case 3: // Unlock
        ForceLock(false);
        break;
    }
  }
  internal void Give(Actor giver, Actor receiver) {
    // FIXME here we should check the conditions of the actions


    PlayActions(giver, receiver, When.Give, this);
  }
}


