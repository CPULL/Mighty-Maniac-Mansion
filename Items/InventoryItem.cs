using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour {
  public RawImage back;
  public Image front;
  public TextMeshProUGUI text;
  public Item item;

  private void OnMouseEnter() {
    back.color = Color.blue;
  }

  private void OnMouseExit() {
    back.color = Color.gray;
  }


}
