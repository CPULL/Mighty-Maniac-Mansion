using UnityEngine;
using UnityEngine.EventSystems;

public class PortraitClickHandler : MonoBehaviour, IPointerClickHandler {
  public void OnPointerClick(PointerEventData eventData) {
    Controller.SendEventData(this);
  }
}
