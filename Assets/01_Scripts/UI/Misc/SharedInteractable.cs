/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Misc
{
    /// <summary>
    /// 공유되는 Interactable
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class SharedInteractable : MonoBehaviour
    {
        [Inject]
        private void Construct(InteractableManager interactableManager)
        {
            interactableManager.InteractableUI.Subscribe(SetInteractable)
                .AddTo(this);
        }

        private void SetInteractable(bool interactable)
        {
            if (this == null)
                return;
            
            var selectable = GetComponent<Selectable>();
            if (selectable == null)
                return;

            selectable.interactable = interactable;
        }
    }
}