using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using SH.Game.UserData;
using SH.Game.UserManager;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

namespace SH.UI.Data
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(ZenjectBinding))]
    public class TextUserId : MonoBehaviour
    {
        [Inject]
        private void Construct()
        {
            var text = GetComponent<Text>();
            var id = UserIdManager.Instance.Id;
            gameObject.SetActive(id != 0);
            text.text = $"UID.{id}";
            
            UserIdManager.UserIdUpdate.Receive().Subscribe(_ =>
            {
                var id = UserIdManager.Instance.Id;
                gameObject.SetActive(id != 0);
                // GameUtils.I2Format(text, "UISettings_PlayerID", id);
                text.text = $"UID.{id}";
            }).AddTo(gameObject);
        }
    }
}
