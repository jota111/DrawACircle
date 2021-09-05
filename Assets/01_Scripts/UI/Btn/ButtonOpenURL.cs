
using System.Text;
using SH.AppEvent;
using SH.Constant;
using SH.Game.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SH.UI.Btn
{
    [RequireComponent(typeof(Button))]
    public class ButtonOpenURL : MonoBehaviour
    {
        [SerializeField] private string url;
        
        private void Start()
        {
            GetComponent<Button>()?.OnClickAsObservable()
                .Subscribe(_ => Click()).AddTo(this);
        }

        private void Click()
        {
            GameSoundManager.Instance.PlaySfx(SFX.sh_common_click);
            Application.OpenURL(url);
        }
    }
}
