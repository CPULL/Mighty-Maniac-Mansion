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
    // Check conditions to use it
    if (!VerifyMainCondition(actor, null, null, When.Use)) return condition.BadResult; // Give the bad result of the condition, if any

    if (Usable == Tstatus.OpenableLocked || Usable == Tstatus.OpenableLockedAutolock) return "It is locked";

    if (Usable == Tstatus.Usable) {
      string res = PlayActions(actor, null, When.Use, this, out bool goodByDefault);
      if (string.IsNullOrEmpty(res) && !goodByDefault) 
        return "It does not work";
      return res;
    }
    else if (Usable == Tstatus.OpenableOpen) {
      SetAsClosedUnlocked();
      return PlayActions(actor, null, When.Use, null, out _);
    }
    else if (Usable == Tstatus.OpenableOpenAutolock) {
      SetAsLockedAuto();
      return PlayActions(actor, null, When.Use, null, out _);
    }
    else if (Usable == Tstatus.OpenableClosed) {
      SetAsOpen();
      return PlayActions(actor, null, When.Use, null, out _);
    }
    else if (Usable == Tstatus.OpenableClosedAutolock) {
      SetAsOpenAuto();
      return PlayActions(actor, null, When.Use, null, out _);
    }

    return "Seems it does nothing";
  }

  public string UseTogether(Actor actor, Item other) {
    // Can we use the two items together?
    if (!VerifyMainCondition(actor, null, other, When.Use)) return condition.BadResult;
    if (!other.VerifyMainCondition(actor, null, this, When.Use)) return other.condition.BadResult;

    // Case of two items
    string res = null;
    foreach (ActionAndCondition ac in actions) {
      if (ac.Condition.IsValid(actor, null, this.Item, other == null ? ItemEnum.Undefined : other.Item, When.Use, 0)) {
        return PlayActions(actor, null, When.Use, other, out bool goodByDefault);
      }
      if (res == null) res = "FIXME ac.Condition.BadResult";
    }
    if (res != null) return res;
    foreach (ActionAndCondition ac in other.actions) {
      if (ac.Condition.IsValid(actor, null, this.Item, other == null ? ItemEnum.Undefined : other.Item, When.Use, 0)) {
        return other.PlayActions(actor, null, When.Use, this, out bool goodByDefault);
      }
      if (res == null) res = "FIXME ac.Condition.BadResult";
    }
    if (res != null) return res;

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

  public string Open(FlagValue val) {
    switch (val) {
      case FlagValue.NA: return null;

      case FlagValue.Yes: // Open
        if (Usable == Tstatus.OpenableClosed) SetAsOpen();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsOpen();
        else if (Usable == Tstatus.OpenableOpen) return null;
        else if (Usable == Tstatus.OpenableOpenAutolock) return null;
        else if (Usable == Tstatus.OpenableLocked) return "It is locked.";
        else if (Usable == Tstatus.OpenableLockedAutolock) return "It is locked.";
        else return "Cannot open it";
        return null;

      case FlagValue.No: // Close
        if (Usable == Tstatus.OpenableClosed) return null;
        else if (Usable == Tstatus.OpenableClosedAutolock) return null;
        else if (Usable == Tstatus.OpenableOpen) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsClosedUnlockedAuto();
        else if (Usable == Tstatus.OpenableLocked) return null;
        else if (Usable == Tstatus.OpenableLockedAutolock) return null;
        else return "Does not work";
        return this.Name + " locked";

      default:
        Debug.LogError("Not handled Open case! " + val.ToString());
        return "Not handled Open case! " + val.ToString();
    }
  }

  public void ForceOpen(FlagValue val) {
    switch (val) {
      case FlagValue.Yes: // Open
        if (Usable == Tstatus.OpenableClosed) SetAsOpen();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsOpen();
        else if (Usable == Tstatus.OpenableOpen) SetAsOpen();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsOpenAuto();
        else if (Usable == Tstatus.OpenableLocked) SetAsOpen();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsOpenAuto();
        return;

      case FlagValue.No: // Close
        if (Usable == Tstatus.OpenableClosed) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsClosedUnlockedAuto();
        else if (Usable == Tstatus.OpenableOpen) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsClosedUnlockedAuto();
        else if (Usable == Tstatus.OpenableLocked) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsClosedUnlockedAuto();
        return;
    }
  }

  public string Lock(FlagValue val) {
    switch(val) {
      case FlagValue.NA: return null;

      case FlagValue.Yes: // Lock
        if (Usable == Tstatus.OpenableClosed) SetAsLocked();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsLockedAuto();
        else if (Usable == Tstatus.OpenableOpen) SetAsLocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsLockedAuto();
        else if (Usable == Tstatus.OpenableLocked) return "Already locked";
        else if (Usable == Tstatus.OpenableLockedAutolock) return "Already locked";
        else return "Does not work";
        return this.Name + " locked.";

      case FlagValue.No: // Unlock
        if (Usable == Tstatus.OpenableClosed) return "Already unlocked";
        else if (Usable == Tstatus.OpenableClosedAutolock) return "Already unlocked";
        else if (Usable == Tstatus.OpenableOpen) return "Already unlocked";
        else if (Usable == Tstatus.OpenableOpenAutolock) return "Already unlocked";
        else if (Usable == Tstatus.OpenableLocked) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsClosedUnlockedAuto();
        else return "Does not work";
        return this.Name + " unlocked!";

      default:
        Debug.LogError("Not handled Lock case! " + val.ToString());
        return "Not handled Lock case! " + val.ToString();
    }
  }

  public void ForceLock(FlagValue val) {
    switch (val) {
      case FlagValue.Yes: // Lock
        if (Usable == Tstatus.OpenableClosed) SetAsLocked();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsLockedAuto();
        else if (Usable == Tstatus.OpenableOpen) SetAsLocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsLockedAuto();
        else if (Usable == Tstatus.OpenableLocked) SetAsLocked();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsLockedAuto();
        break;

      case FlagValue.No: // Unlock
        if (Usable == Tstatus.OpenableClosed) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsClosedUnlockedAuto();
        else if (Usable == Tstatus.OpenableOpen) SetAsOpen();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsOpenAuto();
        else if (Usable == Tstatus.OpenableLocked) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsClosedUnlockedAuto();
        break;
    }
  }

  internal bool HasActions(When when) {
    foreach(ActionAndCondition ac in actions) {
      if (ac.Condition.type != ConditionType.None && (When)ac.Condition.id1 == when) return true;
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
    // Check give conditions
    if (!VerifyMainCondition(giver, receiver, null, When.Give)) {
      receiver.Say(condition.BadResult);
      return;
    }
    PlayActions(giver, receiver, When.Give, this, out bool goodByDefault);
  }
}


