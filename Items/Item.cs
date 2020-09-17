using UnityEngine;

public class Item : IObj {
  public string ID;
  public string ItemName;
  public int Quantity;
  public SpriteRenderer sr;
  public Sprite mainSprite;
  public Sprite maskSprite;
  public Sprite carrySprite;
  public Chars owner = Chars.None;
  // FIXME type?
  


  public string Interaction; // FIXME use with? Like having an Interactable or another Item may produce an action
  public GameAction action;

  private void Awake() {
    sr = GetComponent<SpriteRenderer>();
  }

  private void OnMouseEnter() {
    Controller.SetOverItem(this);
    sr.sprite = maskSprite;
  }
  private void OnMouseExit() {
    Controller.SetOverItem(null);
    sr.sprite = mainSprite;
  }
  internal void PlayActions() {
    if (action == null) return;
    Controller.AddAction(action);
  }
}
