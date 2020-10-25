using UnityEngine;

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
    if (Usable == Tstatus.OpenableLocked || Usable == Tstatus.OpenableLockedAutolock) return "It is locked";

    if (Usable == Tstatus.Usable) {
      ActionRes res = PlayActions(actor, null, When.Use, this);
      if (res == null || !res.actionDone) return "It does not work";
      return null;
    }
    else if (Usable == Tstatus.OpenableOpen) {
      SetAsClosedUnlocked();
      ActionRes res = PlayActions(actor, null, When.Use, null);
      if (res != null && !res.actionDone) return res.res;
      return null;
    }
    else if (Usable == Tstatus.OpenableOpenAutolock) {
      SetAsLockedAuto();
      ActionRes res = PlayActions(actor, null, When.Use, null);
      if (res != null && !res.actionDone) return res.res;
      return null;
    }
    else if (Usable == Tstatus.OpenableClosed) {
      SetAsOpen();
      ActionRes res = PlayActions(actor, null, When.Use, null);
      if (res != null && !res.actionDone) return res.res;
      return null;
    }
    else if (Usable == Tstatus.OpenableClosedAutolock) {
      SetAsOpenAuto();
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
      if (c.Usable != Tstatus.OpenableOpen && c.Usable != Tstatus.OpenableOpenAutolock) return "It is closed";

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

  public string Open(bool val) {
    if (val) { // Open
      if (Usable == Tstatus.OpenableClosed) SetAsOpen();
      else if (Usable == Tstatus.OpenableClosedAutolock) SetAsOpen();
      else if (Usable == Tstatus.OpenableOpen) return null;
      else if (Usable == Tstatus.OpenableOpenAutolock) return null;
      else if (Usable == Tstatus.OpenableLocked) return "It is locked.";
      else if (Usable == Tstatus.OpenableLockedAutolock) return "It is locked.";
      else return "Cannot open it";
      return null;
    }
    else { // Close
      if (Usable == Tstatus.OpenableClosed) return null;
      else if (Usable == Tstatus.OpenableClosedAutolock) return null;
      else if (Usable == Tstatus.OpenableOpen) SetAsClosedUnlocked();
      else if (Usable == Tstatus.OpenableOpenAutolock) SetAsClosedUnlockedAuto();
      else if (Usable == Tstatus.OpenableLocked) return null;
      else if (Usable == Tstatus.OpenableLockedAutolock) return null;
      else return "Does not work";
      return this.Name + " locked";
    }
  }

  public void ForceOpen(bool val) {
    if (val) { // Open
      if (Usable == Tstatus.OpenableClosed) SetAsOpen();
      else if (Usable == Tstatus.OpenableClosedAutolock) SetAsOpen();
      else if (Usable == Tstatus.OpenableOpen) SetAsOpen();
      else if (Usable == Tstatus.OpenableOpenAutolock) SetAsOpenAuto();
      else if (Usable == Tstatus.OpenableLocked) SetAsOpen();
      else if (Usable == Tstatus.OpenableLockedAutolock) SetAsOpenAuto();
      return;
    }
    else { // Close
      if (Usable == Tstatus.OpenableClosed) SetAsClosedUnlocked();
      else if (Usable == Tstatus.OpenableClosedAutolock) SetAsClosedUnlockedAuto();
      else if (Usable == Tstatus.OpenableOpen) SetAsClosedUnlocked();
      else if (Usable == Tstatus.OpenableOpenAutolock) SetAsClosedUnlockedAuto();
      else if (Usable == Tstatus.OpenableLocked) SetAsClosedUnlocked();
      else if (Usable == Tstatus.OpenableLockedAutolock) SetAsClosedUnlockedAuto();
      return;
    }
  }

  public string Lock(bool val) {
    if (val) { // Lock
      if (Usable == Tstatus.OpenableClosed) SetAsLocked();
      else if (Usable == Tstatus.OpenableClosedAutolock) SetAsLockedAuto();
      else if (Usable == Tstatus.OpenableOpen) SetAsLocked();
      else if (Usable == Tstatus.OpenableOpenAutolock) SetAsLockedAuto();
      else if (Usable == Tstatus.OpenableLocked) return "Already locked";
      else if (Usable == Tstatus.OpenableLockedAutolock) return "Already locked";
      else return "Does not work";
      return Name + " locked.";
    }
    else { // Unlock
      if (Usable == Tstatus.OpenableClosed) return "Already unlocked";
      else if (Usable == Tstatus.OpenableClosedAutolock) return "Already unlocked";
      else if (Usable == Tstatus.OpenableOpen) return "Already unlocked";
      else if (Usable == Tstatus.OpenableOpenAutolock) return "Already unlocked";
      else if (Usable == Tstatus.OpenableLocked) SetAsClosedUnlocked();
      else if (Usable == Tstatus.OpenableLockedAutolock) SetAsClosedUnlockedAuto();
      else return "Does not work";
      return Name + " unlocked!";
    }
  }

  public void ForceLock(bool val) {
    if (val) { // Lock
      if (Usable == Tstatus.OpenableClosed) SetAsLocked();
      else if (Usable == Tstatus.OpenableClosedAutolock) SetAsLockedAuto();
      else if (Usable == Tstatus.OpenableOpen) SetAsLocked();
      else if (Usable == Tstatus.OpenableOpenAutolock) SetAsLockedAuto();
      else if (Usable == Tstatus.OpenableLocked) SetAsLocked();
      else if (Usable == Tstatus.OpenableLockedAutolock) SetAsLockedAuto();
    }
    else { // Unlock
      if (Usable == Tstatus.OpenableClosed) SetAsClosedUnlocked();
      else if (Usable == Tstatus.OpenableClosedAutolock) SetAsClosedUnlockedAuto();
      else if (Usable == Tstatus.OpenableOpen) SetAsOpen();
      else if (Usable == Tstatus.OpenableOpenAutolock) SetAsOpenAuto();
      else if (Usable == Tstatus.OpenableLocked) SetAsClosedUnlocked();
      else if (Usable == Tstatus.OpenableLockedAutolock) SetAsClosedUnlockedAuto();
    }
  }

  internal bool HasActions(When when) {
    foreach(ActionAndCondition ac in actions) {
      if (ac.Condition.type != ConditionType.None && (When)ac.Condition.id == when) return true;
    }
    return false;
  }

  internal string GetOpenStatus() {
    switch (Usable) {
      case Tstatus.OpenableOpen: return "Is open";
      case Tstatus.OpenableClosed: return "Is closed";
      case Tstatus.OpenableLocked: return "Is locked";
      case Tstatus.OpenableLockedAutolock: return "Is locked";
      case Tstatus.OpenableOpenAutolock: return "Is open";
      case Tstatus.OpenableClosedAutolock: return "Is closed";
    }
    return "";
  }

  internal bool IsOpen() {
    switch (Usable) {
      case Tstatus.OpenableOpen: return true;
      case Tstatus.OpenableOpenAutolock: return true;
      default: return false;
    }
  }

  internal bool IsLocked() {
    switch (Usable) {
      case Tstatus.OpenableLocked: return true;
      case Tstatus.OpenableLockedAutolock: return true;
      case Tstatus.OpenableClosedAutolock: return true;
      default: return false;
    }

  }

  private void SetAsOpen() {
    bool sound = Usable != Tstatus.OpenableOpen && Usable != Tstatus.OpenableOpenAutolock;
    Usable = Tstatus.OpenableOpen;
    sr.sprite = openImage;
    Door door = this as Door;
    if (door != null) {
      if (door.correspondingDoor.Usable == Tstatus.OpenableClosedAutolock || door.correspondingDoor.Usable == Tstatus.OpenableLockedAutolock || door.correspondingDoor.Usable == Tstatus.OpenableOpenAutolock)
        door.correspondingDoor.Usable = Tstatus.OpenableOpenAutolock;
      else
        door.correspondingDoor.Usable = Tstatus.OpenableOpen;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.openImage;
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
  private void SetAsOpenAuto() {
    bool sound = Usable != Tstatus.OpenableOpen && Usable != Tstatus.OpenableOpenAutolock;
    Usable = Tstatus.OpenableOpenAutolock;
    sr.sprite = openImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableOpenAutolock;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.openImage;
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
      }
    }
  }
  private void SetAsClosedUnlocked() {
    bool soundC = Usable == Tstatus.OpenableOpen || Usable == Tstatus.OpenableOpenAutolock;
    bool soundUl = Usable == Tstatus.OpenableLocked || Usable == Tstatus.OpenableLockedAutolock;
    Usable = Tstatus.OpenableClosed;
    sr.sprite = closeImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableClosed;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
      if (soundC && door.CloseSound != null && door.Audio != null) {
        door.Audio.clip = door.CloseSound;
        door.Audio.Play();
      }
      else if (soundUl && door.UnlockSound != null && door.Audio != null) {
        door.Audio.clip = door.UnlockSound;
        door.Audio.Play();
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
  private void SetAsClosedUnlockedAuto() {
    bool soundC = Usable == Tstatus.OpenableOpen || Usable == Tstatus.OpenableOpenAutolock;
    bool soundUl = Usable == Tstatus.OpenableLocked || Usable == Tstatus.OpenableLockedAutolock;
    Usable = Tstatus.OpenableClosedAutolock;
    sr.sprite = closeImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableClosedAutolock;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
      if (soundC && door.CloseSound != null && door.Audio != null) {
        door.Audio.clip = door.CloseSound;
        door.Audio.Play();
      }
      else if (soundUl && door.UnlockSound != null && door.Audio != null) {
        door.Audio.clip = door.UnlockSound;
        door.Audio.Play();
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

  private void SetAsLocked() {
    bool soundC = Usable == Tstatus.OpenableOpen || Usable == Tstatus.OpenableOpenAutolock;
    bool sound = Usable != Tstatus.OpenableLocked && Usable != Tstatus.OpenableLockedAutolock;
    Usable = Tstatus.OpenableLocked;
    sr.sprite = lockImage == null ? closeImage : lockImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableLocked;
      if (soundC && door.CloseSound != null && door.Audio != null) {
        door.Audio.clip = door.CloseSound;
        door.Audio.Play();
      }
      else door.correspondingDoor.sr.sprite = door.correspondingDoor.lockImage == null ? door.correspondingDoor.closeImage : door.correspondingDoor.lockImage;
      if (sound && door.LockSound != null && door.Audio != null) {
        door.Audio.clip = door.LockSound;
        door.Audio.Play();
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
  private void SetAsLockedAuto() {
    bool soundC = Usable == Tstatus.OpenableOpen || Usable == Tstatus.OpenableOpenAutolock;
    bool sound = Usable != Tstatus.OpenableLocked && Usable != Tstatus.OpenableLockedAutolock;
    Usable = Tstatus.OpenableLockedAutolock;
    sr.sprite = lockImage == null ? closeImage : lockImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableLockedAutolock;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.lockImage == null ? door.correspondingDoor.closeImage : door.correspondingDoor.lockImage;
      if (soundC && door.CloseSound != null && door.Audio != null) {
        door.Audio.clip = door.CloseSound;
        door.Audio.Play();
      }
      else if (sound && door.LockSound != null && door.Audio != null) {
        door.Audio.clip = door.LockSound;
        door.Audio.Play();
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

  internal void Give(Actor giver, Actor receiver) {
    // FIXME here we should check the conditions of the actions


    PlayActions(giver, receiver, When.Give, this);
  }
}


