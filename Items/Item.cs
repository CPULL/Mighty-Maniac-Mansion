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
    string res = VerifyConditions(actor);
    if (res != null) return res;

    if (Usable == Tstatus.OpenableLocked) return "It is locked";

    if (Usable == Tstatus.Usable) {
      return PlayActions(actor);
    }
    else if (Usable == Tstatus.OpenableOpen) {
      Usable = Tstatus.OpenableClosed;
      sr.sprite = yesImage;
      return PlayActions(actor);
    }
    else if (Usable == Tstatus.OpenableClosed) {
      Usable = Tstatus.OpenableOpen;
      sr.sprite = noImage;
      return PlayActions(actor);
    }

    return "Seems it does nothing";
  }

  public void Open(bool val) {
    if (Usable != Tstatus.OpenableOpen && Usable != Tstatus.OpenableClosed) return;
    Usable = val ? Tstatus.OpenableOpen : Tstatus.OpenableClosed;
  }

  public void Lock(bool val) {
    // FIXME we may want to check if we have the key
    if (Usable != Tstatus.OpenableLocked) return;
    Usable = val ? Tstatus.OpenableLocked : Tstatus.OpenableOpen;
  }

}


