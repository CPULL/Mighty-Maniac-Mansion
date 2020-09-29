using UnityEngine;
using UnityEngine.EventSystems;

public class PortraitClickHandler : MonoBehaviour, IPointerClickHandler {
  public UnityEngine.UI.Image portrait;

  public void OnPointerClick(PointerEventData eventData) {
    Controller.HandleToolbarClicks(this);
  }
}
