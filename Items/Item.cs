using UnityEngine;

public class Item : GameItem {
  public SpriteRenderer sr;
  public Vector2 HotSpot;
  public Dir preferredDirection;
  [TextArea(3, 10)] public string Description;
  public Color32 overColor = new Color32(255, 255, 255, 255);
  public Color32 normalColor = new Color32(255, 255, 255, 255);

  public GameCondition Condition;

  private void Awake() {
    sr = GetComponent<SpriteRenderer>();
  }

  private void OnMouseEnter() {
    Controller.SetItem(this);
    sr.sprite = maskImage;
    sr.color = overColor;
  }
  private void OnMouseExit() {
    Controller.SetItem(null);
    sr.sprite = (Openable == Tstatus.No) ? noImage : yesImage;
    sr.color = normalColor;
  }

  public string Use() {
    // Check conditions to use it
    string res = Condition.Verify();
    if (res != null) return res;

    if (Lockable == Tstatus.Yes) return "It is locked";

    if (Usable == Tstatus.Yes) {
      return PlayActions();
    }
    else if (Openable == Tstatus.Yes) {
      Openable = Tstatus.No;
      sr.sprite = noImage;
      return PlayActions();
    }
    else if (Openable == Tstatus.No) {
      Openable = Tstatus.Yes;
      sr.sprite = yesImage;
      return PlayActions();
    }

    return "Seems it does nothing";
  }

}
