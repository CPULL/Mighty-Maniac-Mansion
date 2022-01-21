using UnityEngine;
using UnityEngine.EventSystems;

public class OverActor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  public CharSelection cs;
  public Chars num;

  public void OnPointerEnter(PointerEventData eventData) {
    cs.Over(num);
  }

  public void OnPointerExit(PointerEventData eventData) {
    cs.Over(Chars.None);
  }
}
