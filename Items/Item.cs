using System;
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
    if (!VerifyMainCondition(actor, null, When.Use)) return condition.Result;

    if (Usable == Tstatus.OpenableLocked || Usable == Tstatus.OpenableLockedAutolock) return "a big message just to have a big balloon.\n\n -- It is locked -- \n\nWith a last line for debugging.";

    if (Usable == Tstatus.Usable) {
      if (!PlayActions(actor, null, When.Use)) return "It does not work";
    }
    else if (Usable == Tstatus.OpenableOpen) {
      SetAsClosedUnlocked();
      PlayActions(actor, null, When.Use);
    }
    else if (Usable == Tstatus.OpenableOpenAutolock) {
      SetAsLockedAuto();
      PlayActions(actor, null, When.Use);
    }
    else if (Usable == Tstatus.OpenableClosed) {
      SetAsOpen();
      PlayActions(actor, null, When.Use);
    }
    else if (Usable == Tstatus.OpenableClosedAutolock) {
      SetAsOpenAuto();
      PlayActions(actor, null, When.Use);
    }
    else
      return "Seems it does nothing";
    return null;
  }

  public string Open(ChangeWay val) {
    switch (val) {
      case ChangeWay.Ignore: return null;

      case ChangeWay.EnOpenLock: // Open
        if (Usable == Tstatus.OpenableClosed) SetAsOpen();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsOpen();
        else if (Usable == Tstatus.OpenableOpen) return null;
        else if (Usable == Tstatus.OpenableOpenAutolock) return null;
        else if (Usable == Tstatus.OpenableLocked) return "It is locked.";
        else if (Usable == Tstatus.OpenableLockedAutolock) return "It is locked.";
        else return "Cannot open it";
        return null;

      case ChangeWay.DisCloseUnlock: // Close
        if (Usable == Tstatus.OpenableClosed) return null;
        else if (Usable == Tstatus.OpenableClosedAutolock) return null;
        else if (Usable == Tstatus.OpenableOpen) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsClosedUnlockedAuto();
        else if (Usable == Tstatus.OpenableLocked) return null;
        else if (Usable == Tstatus.OpenableLockedAutolock) return null;
        else return "Does not work";
        return this.Name + " locked";

      case ChangeWay.SwapSwitch:
        if (Usable == Tstatus.OpenableClosed) SetAsOpen();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsOpenAuto();
        else if (Usable == Tstatus.OpenableOpen) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsClosedUnlockedAuto();
        else if (Usable == Tstatus.OpenableLocked) return "It is locked.";
        else if (Usable == Tstatus.OpenableLockedAutolock) return "It is locked.";
        else return "Does not work";
        return null;

      default:
        Debug.LogError("Not handled Open case! " + val.ToString());
        return "Not handled Open case! " + val.ToString();
    }
  }

  public void ForceOpen(ChangeWay val) {
    switch (val) {
      case ChangeWay.EnOpenLock: // Open
        if (Usable == Tstatus.OpenableClosed) SetAsOpen();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsOpen();
        else if (Usable == Tstatus.OpenableOpen) SetAsOpen();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsOpenAuto();
        else if (Usable == Tstatus.OpenableLocked) SetAsOpen();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsOpenAuto();
        return;

      case ChangeWay.DisCloseUnlock: // Close
        if (Usable == Tstatus.OpenableClosed) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsClosedUnlockedAuto();
        else if (Usable == Tstatus.OpenableOpen) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsClosedUnlockedAuto();
        else if (Usable == Tstatus.OpenableLocked) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsClosedUnlockedAuto();
        return;
    }
  }

  public string Lock(ChangeWay val) {
    switch(val) {
      case ChangeWay.Ignore: return null;

      case ChangeWay.EnOpenLock: // Lock
        if (Usable == Tstatus.OpenableClosed) SetAsLocked();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsLockedAuto();
        else if (Usable == Tstatus.OpenableOpen) SetAsLocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsLockedAuto();
        else if (Usable == Tstatus.OpenableLocked) return "Already locked";
        else if (Usable == Tstatus.OpenableLockedAutolock) return "Already locked";
        else return "Does not work";
        return this.Name + " locked.";

      case ChangeWay.DisCloseUnlock: // Unlock
        if (Usable == Tstatus.OpenableClosed) return "Already unlocked";
        else if (Usable == Tstatus.OpenableClosedAutolock) return "Already unlocked";
        else if (Usable == Tstatus.OpenableOpen) return "Already unlocked";
        else if (Usable == Tstatus.OpenableOpenAutolock) return "Already unlocked";
        else if (Usable == Tstatus.OpenableLocked) SetAsClosedUnlocked();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsClosedUnlockedAuto();
        else return "Does not work";
        return this.Name + " unlocked!";

      case ChangeWay.SwapSwitch:
        if (Usable == Tstatus.OpenableClosed) { SetAsLocked(); return Name + " locked."; }
        else if (Usable == Tstatus.OpenableClosedAutolock) { SetAsLockedAuto(); return Name + " locked."; }
        else if (Usable == Tstatus.OpenableOpen) { SetAsLocked(); return Name + " locked."; }
        else if (Usable == Tstatus.OpenableOpenAutolock) { SetAsLockedAuto(); return Name + " locked."; }
        else if (Usable == Tstatus.OpenableLocked) { SetAsClosedUnlocked(); return Name + " unlocked!"; }
        else if (Usable == Tstatus.OpenableLockedAutolock) { SetAsClosedUnlockedAuto(); return Name + " unlocked!"; }
        else return "Does not work";

      default:
        Debug.LogError("Not handled Lock case! " + val.ToString());
        return "Not handled Lock case! " + val.ToString();
    }
  }

  public void ForceLock(ChangeWay val) {
    switch (val) {
      case ChangeWay.EnOpenLock: // Lock
        if (Usable == Tstatus.OpenableClosed) SetAsLocked();
        else if (Usable == Tstatus.OpenableClosedAutolock) SetAsLockedAuto();
        else if (Usable == Tstatus.OpenableOpen) SetAsLocked();
        else if (Usable == Tstatus.OpenableOpenAutolock) SetAsLockedAuto();
        else if (Usable == Tstatus.OpenableLocked) SetAsLocked();
        else if (Usable == Tstatus.OpenableLockedAutolock) SetAsLockedAuto();
        break;

      case ChangeWay.DisCloseUnlock: // Unlock
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
      if (ac.Condition.condition != Condition.None && ac.Condition.when == when) return true;
    }
    return false;
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
        Debug.Log("Open");
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
        Debug.Log("Open autolock");
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
        Debug.Log("Close");
      }
      else if (soundUl && door.UnlockSound != null && door.Audio != null) {
        door.Audio.clip = door.UnlockSound;
        door.Audio.Play();
        Debug.Log("Unlock");
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
        Debug.Log("Close");
      }
      else if (soundUl && door.UnlockSound != null && door.Audio != null) {
        door.Audio.clip = door.UnlockSound;
        door.Audio.Play();
        Debug.Log("Unlock");
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
        Debug.Log("Close");
      }
      else door.correspondingDoor.sr.sprite = door.correspondingDoor.lockImage == null ? door.correspondingDoor.closeImage : door.correspondingDoor.lockImage;
      if (sound && door.LockSound != null && door.Audio != null) {
        door.Audio.clip = door.LockSound;
        door.Audio.Play();
        Debug.Log("Locked");
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
        Debug.Log("Close");
      }
      else if (sound && door.LockSound != null && door.Audio != null) {
        door.Audio.clip = door.LockSound;
        door.Audio.Play();
        Debug.Log("Locked auto");
      }
    }
  }

  internal void Give(Actor giver, Actor receiver) {
    // Check give conditions
    if (!VerifyMainCondition(giver, receiver, When.Give)) {
      receiver.Say(condition.Result);
      return;
    }
    PlayActions(giver, receiver, When.Give, this);
  }
}


