using UnityEngine;
using UnityEngine.EventSystems;

public class ActorClickHandler : MonoBehaviour, IPointerClickHandler {
  public void OnPointerClick(PointerEventData eventData) {
    Controller.SendEventData(eventData, this);
  }
}
