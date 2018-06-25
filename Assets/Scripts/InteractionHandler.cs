using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Events;

public class InteractionHandler : MonoBehaviour, IInputClickHandler
{
    public UnityEvent OnSelect = new UnityEvent();

    //AirTapのイベントハンドラ
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if(!eventData.used)
            OnSelect.Invoke();
    }
   
}
