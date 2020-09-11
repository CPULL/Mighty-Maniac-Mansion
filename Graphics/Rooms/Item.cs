using UnityEngine;

public class Item : MonoBehaviour {
  SpriteRenderer img;
  public ItemType type;
  public string ItemName;
  [TextArea(3, 10)]
  public string Description;

  private void Awake() {
    img = GetComponent<SpriteRenderer>();
  }


  private void OnMouseEnter() {
    img.color = Color.yellow;
    Controller.SetCurrentItem(this);
  }

  private void OnMouseExit() {
    img.color = Color.white;
    Controller.SetCurrentItem(null);
  }
}
