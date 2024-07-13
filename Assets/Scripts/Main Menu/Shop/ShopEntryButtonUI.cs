using UnityEngine;
using UnityEngine.EventSystems;

public class ShopEntryButtonUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject dimmerPanel;
    public void OnSelect(BaseEventData eventData) {
        dimmerPanel.SetActive(true);
    }
    public void OnDeselect(BaseEventData eventData) {
        dimmerPanel.SetActive(false);
    }
}
