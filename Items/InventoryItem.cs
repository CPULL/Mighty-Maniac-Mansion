using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  public RawImage back;
  public Image front;
  public TextMeshProUGUI text;
  public Item item;

  public void OnPointerEnter(PointerEventData eventData) {
    back.color = Color.blue;
  }

  public void OnPointerExit(PointerEventData eventData) {
    back.color = Color.gray;
  }


}
