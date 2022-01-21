using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
  [HideInInspector] public SpriteRenderer sr;
  [HideInInspector] public SpriteRenderer[] srs;

  public ItemEnum ID;
  public string Name;

  public Tstatus Usable;
  public OpenStatus openStatus;
  public LockStatus lockStatus;

  public WhatItDoes whatItDoesL = WhatItDoes.Walk;
  public WhatItDoes whatItDoesR = WhatItDoes.Use;

  [TextArea(3, 10)] public string Description;
  public Chars owner;

  public Vector2 HotSpot;
  public Dir dir;
  public Color32 normalColor = new Color32(255, 255, 255, 255);
  public Color32 overColor = new Color32(255, 255, 0, 255);

  public Sprite openImage;
  public Sprite closeImage;
  public Sprite lockImage;
  public Sprite iconImage;

  public List<ActionAndCondition> actions;


  private string animToPlay = null;
  private System.DateTime animStartTime;
  private float timeForAnim;
  [HideInInspector] public bool isEnabled = true;

  public bool PlayAnim(string animName, float timer, out float length) {
    Animator anim = GetComponent<Animator>();
    if (anim == null) {
      Debug.LogError("Missing animator for animated item: " + gameObject.name);
      length = 0;
      return true;
    }
    if (gameObject.activeInHierarchy) {
      animToPlay = null;
      anim.enabled = true;
      anim.Play(animName);
      length = anim.GetCurrentAnimatorStateInfo(0).length;
    }
    else {
      animToPlay = animName;
      animStartTime = System.DateTime.Now;
      timeForAnim = timer;
      length = timer;
      foreach(AnimationClip clip in anim.runtimeAnimatorController.animationClips) {
        if (clip.name.Equals(animName, System.StringComparison.InvariantCultureIgnoreCase)) {
          length = clip.length;
          break;
        }
      }
    }
    return false;
  }

  private void Awake() {
    sr = GetComponent<SpriteRenderer>();
    if (sr == null) { // Item is composed of sub-items
      int num = 0;
      foreach(Transform t in transform) {
        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
        if (sr != null && sr.material.name != "SnapPoint") num++;
      }
      srs = new SpriteRenderer[num];
      num = 0;
      foreach (Transform t in transform) {
        SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
        if (sr != null && sr.material.name != "SnapPoint") {
          srs[num] = sr;
          num++;
        }
      }
    }
    isEnabled = gameObject.activeSelf;
  }

  private void OnMouseEnter() {
    if (Options.IsActive() || Controller.InventoryActive()) return;
    Controller.SetItem(this);
    if (sr != null)
      sr.color = overColor;
    else if (srs!=null) {
      foreach(SpriteRenderer sr in srs)
        sr.color = overColor;
    }
  }
  private void OnMouseExit() {
    Controller.SetItem(null);
    if (sr != null)
      sr.color = normalColor;
    else if (srs != null) {
      foreach (SpriteRenderer sr in srs)
        sr.color = normalColor;
    }
  }

  private void OnEnable() {
    if (animToPlay == null) return;
    System.TimeSpan elapsed = System.DateTime.Now - animStartTime;
    if (elapsed.TotalSeconds > timeForAnim) {
      animToPlay = null;
      return;
    }
    Animator anim = GetComponent<Animator>();
    anim.Play(animToPlay, 0, (float)elapsed.TotalSeconds / timeForAnim);
  }

  public string Use(Actor actor) {
    if (Usable == Tstatus.Openable && !IsOpen() && IsLocked()) return "It is locked";

    if (Usable == Tstatus.Usable) {
      ActionRes res = PlayActions(actor, null, When.Use, this);
      if (res == null || !res.actionDone) {
        if (string.IsNullOrWhiteSpace(res.res)) return "It does not work";
        return res.res;
      }
      return null;
    }
    else if (Usable == Tstatus.Openable || Usable == Tstatus.Swithchable) {
      if (openStatus == OpenStatus.Open) {
        SetAsClosed();
        ActionRes res = PlayActions(actor, null, When.Use, null);
        if (res != null && !res.actionDone) return res.res;
        return null;
      }
      else if (openStatus == OpenStatus.Closed) {
        SetAsOpen();
        ActionRes res = PlayActions(actor, null, When.Use, null);
        if (res != null && !res.actionDone) return res.res;
        return null;
      }
    }

    return "Seems it does nothing";
  }

  internal ActionRes PlayActions(Actor actor, Actor secondary, When when, Item item) {
    if (actions == null || actions.Count == 0) return null;

    ActionRes res = null;
    Item item1 = this;
    Item item2 = (this == item ? null : item);

    foreach (ActionAndCondition ac in actions) {
      if (ac.IsValid(actor, secondary, item1, item2, when)) {
        res = new ActionRes { 
          actionDone = true, 
          res = ac.RunAsSequence(actor, secondary, item1, Name)
        };
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
      if (ac.IsValid(actor, null, this, other, When.UseTogether)) {
        ac.RunAsSequence(actor, null, this, Name);
        done = true;
      }
      else res = ac.GetConditionMsg(actor, null, When.UseTogether, this, other);
    }
    if (res != null) return res;
    foreach (ActionAndCondition ac in other.actions) {
      if (ac.IsValid(actor, null, this, other, When.UseTogether)) {
        ac.RunAsSequence(actor, null, this, Name);
        done = true;
      }
      else res = ac.GetConditionMsg(actor, null, When.UseTogether, this, other);
    }
    if (res != null) return res;
    if (done) return null;

    if ((other as Door) != null) return "Not a valid key";

    // Case of an item and a container
    Container c = other as Container;
    if (c != null) {
      if (c.openStatus != OpenStatus.Open) return "It is closed";
      if (c.ValidFor(this)) {
        // Put item back
        actor.inventory.Remove(this);
        transform.parent = c.transform;
        owner = Chars.None;
        c.Place(this, actor);
        return null;
      }
      else
        return "It does not fit";
    }


    if (!done) {
      if (res != null) return res;
      return "It does not work...";
    }
    return null;
  }


  public void ForceOpen(bool val) {
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

  internal bool HasActionsForWhen(When when) {
    foreach(ActionAndCondition ac in actions) {
      if (ac.Condition.type != ConditionType.None && (ac.Condition.when == when || ac.Condition.when == When.Always)) return true;
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

  internal void SetZPos(float z) {
    if (sr != null) {
      sr.sortingOrder = (int)z;
    }
    else {
      foreach(SpriteRenderer sr in srs)
        sr.sortingOrder = (int)z;
    }
  }

  internal bool IsOpen() {
    return openStatus != OpenStatus.Closed;
  }

  internal bool IsLocked() {
    if (Usable != Tstatus.Openable) return false;
    return lockStatus == LockStatus.Locked || lockStatus == LockStatus.Autolock;
  }

  private void SetAsOpen() {
    bool sound = openStatus == OpenStatus.Closed;
    openStatus = OpenStatus.Open;
    if (sr != null && openImage != null)
      sr.sprite = openImage;
    Door door = this as Door;
    if (door != null) {
      if (door.correspondingDoor != null) {
        door.correspondingDoor.openStatus = OpenStatus.Open;
        door.correspondingDoor.sr.sprite = door.correspondingDoor.openImage;
      }
      if (sound && door.OpenSound != null && door.Audio != null) {
        door.Audio.clip = door.OpenSound;
        if (door.gameObject.activeSelf) door.Audio.Play();
      }
    }
    else {
      Container c = this as Container;
      if (c != null) {
        c.ShowItems();
        if (sound && c.OpenSound != null && c.Audio != null) {
          c.Audio.clip = c.OpenSound;
          if (c.gameObject.activeSelf) c.Audio.Play();
        }
      }
    }
  }
  private void SetAsClosed() {
    bool sound = openStatus == OpenStatus.Open;
    if (openStatus == OpenStatus.Open) {
      openStatus = OpenStatus.Closed;
      if (sr != null && closeImage != null)
        sr.sprite = closeImage;
      if (lockStatus == LockStatus.UnlockedAutolock) lockStatus = LockStatus.Autolock;
    }
    Door door = this as Door;
    if (door != null) {
      if (door.correspondingDoor != null && door.correspondingDoor.openStatus == OpenStatus.Open) {
        door.correspondingDoor.openStatus = OpenStatus.Closed;
        door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
        if (door.correspondingDoor.lockStatus == LockStatus.UnlockedAutolock) door.correspondingDoor.lockStatus = LockStatus.Autolock;
      }
      if (sound && door.CloseSound != null && door.Audio != null) {
        door.Audio.clip = door.CloseSound;
        if (door.gameObject.activeSelf) door.Audio.Play();
      }
    }
    else {
      Container c = this as Container;
      if (c != null) {
        c.HideItems();
        if (sound && c.CloseSound != null && c.Audio != null) {
          c.Audio.clip = c.CloseSound;
          if (c.gameObject.activeSelf) c.Audio.Play();
        }
      }
    }
  }

  private void SetAsUnlocked() {
    bool soundC = openStatus == OpenStatus.Open;
    bool soundUl = lockStatus == LockStatus.Locked || lockStatus == LockStatus.Autolock;
    if (lockStatus == LockStatus.Locked) lockStatus = LockStatus.Unlocked;
    if (lockStatus == LockStatus.Autolock) lockStatus = LockStatus.UnlockedAutolock;
    if (sr != null && closeImage != null)
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
          if (door.gameObject.activeSelf) door.Audio.Play();
        }
        else if (soundUl && door.UnlockSound != null && door.Audio != null) {
          door.Audio.clip = door.UnlockSound;
          if (door.gameObject.activeSelf) door.Audio.Play();
        }
      }
    }
    else {
      Container c = this as Container;
      if (c != null) {
        if (soundUl && c.UnlockSound != null && c.Audio != null) {
          c.Audio.clip = c.UnlockSound;
          if (c.gameObject.activeSelf) c.Audio.Play();
        }
      }
    }
  }

  private void SetAsLocked() {
    bool soundC = openStatus == OpenStatus.Open;
    bool soundUl = lockStatus == LockStatus.Unlocked || lockStatus == LockStatus.UnlockedAutolock;
    if (lockStatus == LockStatus.Unlocked) lockStatus = LockStatus.Locked;
    if (lockStatus == LockStatus.UnlockedAutolock) lockStatus = LockStatus.Autolock;
    if (sr != null && lockImage != null) sr.sprite = lockImage; else sr.sprite = closeImage;
    Door door = this as Door;
    if (door != null) {
      if (door.correspondingDoor != null) {
        if (door.correspondingDoor.openStatus == OpenStatus.Open) door.correspondingDoor.openStatus = OpenStatus.Closed;
        if (door.correspondingDoor.lockStatus == LockStatus.Unlocked) {
          door.correspondingDoor.lockStatus = LockStatus.Locked;
          if (door.correspondingDoor.sr != null && door.correspondingDoor.lockImage != null) door.correspondingDoor.sr.sprite = door.correspondingDoor.lockImage; else door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
        }
        else if (door.correspondingDoor.lockStatus == LockStatus.UnlockedAutolock) {
          door.correspondingDoor.lockStatus = LockStatus.Autolock;
          if (door.correspondingDoor.sr != null && door.correspondingDoor.lockImage != null) door.correspondingDoor.sr.sprite = door.correspondingDoor.lockImage; else door.correspondingDoor.sr.sprite = door.correspondingDoor.closeImage;
        }
        if (soundC && door.CloseSound != null && door.Audio != null) {
          door.Audio.clip = door.CloseSound;
          if (door.gameObject.activeSelf) door.Audio.Play();
        }
        else if (soundUl && door.LockSound != null && door.Audio != null) {
          door.Audio.clip = door.LockSound;
          if (door.gameObject.activeSelf) door.Audio.Play();
        }
      }
    }
    else {
      Container c = this as Container;
      if (c != null) {
        c.HideItems();
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
    ActionRes res = PlayActions(giver, receiver, When.Give, this);
    if (res == null || !res.actionDone) { // Give it by default if the receiver is not an NPC
      if (receiver.IAmNPC) {
        if (receiver as Dog)
          receiver.Say("Grrrrrr!");
        else
          receiver.Say("I don't want that!");
      }
      else {
        if (ID == ItemEnum.Coat) giver.Wear(ItemEnum.Coat, true);
        giver.inventory.Remove(this);
        receiver.inventory.Add(this);
        owner = Controller.GetCharFromActor(receiver);
        Controller.UpdateInventory();
      }
    }
  }
}


