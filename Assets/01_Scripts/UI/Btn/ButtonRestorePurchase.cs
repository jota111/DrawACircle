using System.Collections;
using System.Collections.Generic;
using SH.Game.Manager;
using SH.Iap;
using SH.UI.View.Dialog;
using SH.Util.UniRx;
using UniRx;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.UI.Btn
{
    [RequireComponent(typeof(Button))]
    public class ButtonRestorePurchase : MonoBehaviour
    {
        private IapManager _iapManager;

        [Inject]
        private void Construct(IapManager iapManager, SceneDisposable disposable)
        {
            _iapManager = iapManager;
            GetComponent<Button>()?.OnClickAsObservable()
                .Subscribe(_ => Click()).AddTo(this);

            MessageBroker.Default.Receive<MsgIapRestore>()
                .Subscribe(OnMessage)
                .AddTo(disposable);
        }

        private void OnMessage(MsgIapRestore restore)
        {
            if (restore is {result: true})
                UIViewDialog.Instance.DialogForget("UIShop_RestorePurchaseSuccess", "Text_RestorePurchase", "Dialog_OK", true, false);
            else
                UIViewDialog.Instance.DialogForget("UIShop_RestorePurchaseFailed", "Text_RestorePurchase", "Dialog_OK", true, false);
        }

        private void Click()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            _iapManager.Restore();
        }
    }
}