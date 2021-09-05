/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using SH.Constant;
using SH.Game.Manager;
using SH.Game.UserData;
using SH.UI.View;
using SH.Util.UniRx;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SH.Game.Settings
{
    public class UIViewTransferThanks : UIViewBase
    {
        [SerializeField, Required, ChildGameObjectsOnly]
        private Button _buttonOK;

        private FundManager _fundManager;
        private UserData.UserData _userData;
        private GetEffectManager _getEffectManager;

        //[Inject]
        private void Construct(FundManager fundManager, UserData.UserData userData, GetEffectManager getEffectManager)
        {
            _getEffectManager = getEffectManager;
            _userData = userData;
            _fundManager = fundManager;

            // if (userData.TransferReward)
            // {
            //     Observable.NextFrame()
            //         .Subscribe(_ => Show())
            //         .AddTo(this);
            // }
        }

        private void Start()
        {
            _buttonOK.OnClickSoundAsObservable()
                .First()
                .Subscribe(_ => OnClick())
                .AddTo(this);
        }

        private void OnClick()
        {
            Hide();
            // if (_userData.TransferReward)
            // {
            //     GameSoundManager.Instance.PlaySfx(SFX.sh_tap_jewel);
            //     _fundManager.AddJewel(200);
            //     _getEffectManager?.Get(ItemType.Jewel_04, _buttonOK.transform.position);
            //     _userData.TransferReward = false;
            //     UserDataSaveManager.Instance.Save();
            // }
        }
    }
}