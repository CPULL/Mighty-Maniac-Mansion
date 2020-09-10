using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler {
  public void OnPointerClick(PointerEventData eventData) {
    Controller.SendEventData(eventData);
  }
}
