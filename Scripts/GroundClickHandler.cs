using UnityEngine;
using UnityEngine.EventSystems;

public class GroundClickHandler : MonoBehaviour, IPointerClickHandler {
  public void OnPointerClick(PointerEventData eventData) {
    Controller.SendEventData(this);
  }
}
