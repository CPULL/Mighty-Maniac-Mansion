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

  public string Use() {
    // Check conditions to use it
    string res = VerifyConditions();
    if (res != null) return res;

    if (Lockable == Tstatus.CanAndDone) return "It is locked";

    if (Usable == Tstatus.CanAndDone) {
      return PlayActions();
    }
    else if (Openable == Tstatus.CanAndDone) {
      Openable = Tstatus.CanAndNotDone;
      sr.sprite = noImage;
      return PlayActions();
    }
    else if (Openable == Tstatus.CanAndNotDone) {
      Openable = Tstatus.CanAndDone;
      sr.sprite = yesImage;
      return PlayActions();
    }

    return "Seems it does nothing";
  }


}


