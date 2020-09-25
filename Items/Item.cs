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
    if (!VerifyConditions(actor, null, When.Use)) return condition.Result;

    if (Usable == Tstatus.OpenableLocked || Usable == Tstatus.OpenableLockedAutolock) return "It is locked";

    if (Usable == Tstatus.Usable) {
      return PlayActions(actor, null, When.Use);
    }
    else if (Usable == Tstatus.OpenableOpen) {
      SetAsClosedUnlocked();
      return PlayActions(actor, null, When.Use);
    }
    else if (Usable == Tstatus.OpenableOpenAutolock) {
      SetAsClosedUnlockedAuto();
      return PlayActions(actor, null, When.Use);
    }
    else if (Usable == Tstatus.OpenableClosed) {
      SetAsOpen();
      return PlayActions(actor, null, When.Use);
    }
    else if (Usable == Tstatus.OpenableClosedAutolock) {
      SetAsOpenAuto();
      return PlayActions(actor, null, When.Use);
    }

    return "Seems it does nothing";
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

  internal bool HasActions(When when) {
    foreach(ActionAndCondition ac in actions) {
      if (ac.Condition.condition != Condition.None && ac.Condition.when == when) return true;
    }
    return false;
  }

  private void SetAsOpen() {
    Usable = Tstatus.OpenableOpen;
    sr.sprite = openImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableOpen;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.openImage;
    }
  }
  private void SetAsOpenAuto() {
    Usable = Tstatus.OpenableOpenAutolock;
    sr.sprite = openImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableOpenAutolock;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.openImage;
    }
  }
  private void SetAsClosedUnlocked() {
    Usable = Tstatus.OpenableClosed;
    sr.sprite = closeImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableClosed;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
    }
  }
  private void SetAsClosedUnlockedAuto() {
    Usable = Tstatus.OpenableClosedAutolock;
    sr.sprite = closeImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableClosedAutolock;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
    }
  }
  private void SetAsLocked() {
    Usable = Tstatus.OpenableLocked;
    sr.sprite = lockImage == null ? closeImage : lockImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableLocked;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.lockImage == null ? door.correspondingDoor.closeImage : door.correspondingDoor.lockImage;
    }
  }
  private void SetAsLockedAuto() {
    Usable = Tstatus.OpenableLockedAutolock;
    sr.sprite = lockImage == null ? closeImage : lockImage;
    Door door = this as Door;
    if (door != null) {
      door.correspondingDoor.Usable = Tstatus.OpenableLockedAutolock;
      door.correspondingDoor.sr.sprite = door.correspondingDoor.lockImage == null ? door.correspondingDoor.closeImage : door.correspondingDoor.lockImage;
    }
  }

  internal void Give(Actor giver, Actor receiver) {
    // Check give conditions
    if (!VerifyConditions(giver, receiver, When.Give)) {
      receiver.Say(condition.Result);
      return;
    }
    string res = PlayActions(giver, receiver, When.Give, this);
    if (res != null) {
      if (Controller.IsEnemy(receiver)) {
        receiver.Say("No thanks");
        return;
      }
    }

    giver.inventory.Remove(this);
    receiver.inventory.Add(this);
    Controller.UpdateInventory();
  }
}


