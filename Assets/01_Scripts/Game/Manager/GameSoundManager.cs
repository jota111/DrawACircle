using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OutGameCore;
using SH.Constant;
using SH.Data;
using SH.Game.DataLocal;
using UniRx;
using UnityEngine;
using Zenject;
using SH.Game.InGame.Msg;
using SH.Util;
using SH.Util.UniRx;

namespace SH.Game.Manager
{
    public enum BGM
    {
        TitleBGM, // Title배경음
        sh_lobby_ambience, // 0    로비배경음
        InGameBGM, //1      인게임배경음
        sh_flower_evt_bgm, // 꽃밭 인게임 배경
    }

    public sealed class GameSoundManager : IDisposable
    {
        private readonly UserData.UserData _userData;

        public SettingsLocal settingsLocal
        {
            get => _userData.Settings;
            set => _userData.Settings = value;
        }

        private MergeBoardIdent state;
        public static GameSoundManager Instance { get; private set; }

        public GameSoundManager(UserData.UserData userData, SceneDisposable disposable)
        {
            Instance = this;
            _userData = userData;
            SetMessages(disposable);

            UniTask.Void(async () =>
            {
                await UniTask.WaitUntil(() => AudioController.DoesInstanceExist() != null);
                settingsLocal.OptionBGM.Subscribe(Play_BGM).AddTo(disposable);
            });
        }

        #region 메시지

        private void SetMessages(SceneDisposable disposable)
        {
            MessageBroker.Default.Receive<BoardChanged>()
                .Where(_ => settingsLocal.OptionBGM.Value && AudioController.DoesInstanceExist() != null)
                .Subscribe(OnMessage)
                .AddTo(disposable);
            MessageBroker.Default.Receive<UIViewPush>()
                .Where(_ => settingsLocal.OptionSoundEffect.Value)
                .Subscribe(OnMessage)
                .AddTo(disposable);
            MessageBroker.Default.Receive<UIViewPop>()
                .Where(_ => settingsLocal.OptionSoundEffect.Value)
                .Subscribe(OnMessage)
                .AddTo(disposable);
        }

        private void OnMessage(BoardChanged msg)
        {
            state = msg.Ident;
            PlayBGM();
        }

        private void OnMessage(UIViewPush pop)
        {
            PlaySfx(SFX.sh_common_popup);
        }

        private void OnMessage(UIViewPop pop)
        {
            PlaySfx(SFX.sh_common_popup);
        }

        #endregion

        #region BGM

        public void Play_BGM(bool onOff)
        {
            if (onOff)
            {
                PlayBGM();
            }
            else
                StopAllBGM();
        }

        private void PlayBGM()
        {
            switch (state)
            {
                case MergeBoardIdent.None:
                    Play_BGM(BGM.sh_lobby_ambience);
                    break;

                case MergeBoardIdent.FlowerGarden:
                    Play_BGM(BGM.sh_flower_evt_bgm);
                    break;

                default:
                    StopAllBGM();
                    break;
            }
        }

        public void Play_BGM(BGM bgm)
        {
            // GameUtils.Log($"사운드 Play_BGM {bgm}");
            Stop_BGMExcept(bgm);
            if ((settingsLocal.OptionBGM.Value) && (AudioController.IsPlaying(bgm.ToEnumString()) == false))
            {
                AudioController.Play(bgm.ToEnumString());
            }
        }

        private static void Stop_BGMExcept(BGM exceptBGM)
        {
            foreach (var item in EnumHelper.AllValues<BGM>())
            {
                string bgm = item.ToEnumString();
                if (AudioController.IsPlaying(bgm) == true && item != exceptBGM)
                {
                    AudioController.Stop(bgm);
                }
            }
        }

        public static void StopAllBGM()
        {
            // GameUtils.Log($"사운드 StopAllBGM");
            foreach (var item in EnumHelper.AllValues<BGM>())
            {
                string bgm = item.ToEnumString();
                if (AudioController.IsPlaying(bgm) == true)
                {
                    AudioController.Stop(bgm);
                }
            }
        }

        #endregion

        #region SoundFX

        // 1 ~ 38 효과음 재생
        public void PlaySfx(string fx)
        {
            if (settingsLocal.OptionSoundEffect.Value && AudioController.GetAudioItem(fx) != null)
            {
                AudioController.Play(fx);
            }
        }

        public void PlaySfx(SFX fx)
        {
            if (settingsLocal.OptionSoundEffect.Value && AudioController.GetAudioItem(fx.ToEnumString()) != null)
            {
                AudioController.Play(fx.ToEnumString());
            }
        }

        public void PlaySfx(LobbySFX fx)
        {
            if (settingsLocal.OptionSoundEffect.Value && AudioController.GetAudioItem(fx.ToEnumString()) != null)
            {
                AudioController.Play(fx.ToEnumString());
            }
        }

        public void StopSfx(string fx)
        {
            if (settingsLocal.OptionSoundEffect.Value && AudioController.GetAudioItem(fx) != null && AudioController.IsPlaying(fx) == true)
            {
                AudioController.Stop(fx);
            }
        }

        public void StopSfx(SFX fx)
        {
            if (settingsLocal.OptionSoundEffect.Value && AudioController.GetAudioItem(fx.ToEnumString()) != null &&
                AudioController.IsPlaying(fx.ToEnumString()) == true)
            {
                AudioController.Stop(fx.ToEnumString());
            }
        }

        public void StopSfx(LobbySFX fx)
        {
            if (settingsLocal.OptionSoundEffect.Value && AudioController.GetAudioItem(fx.ToEnumString()) != null &&
                AudioController.IsPlaying(fx.ToEnumString()) == true)
            {
                AudioController.Stop(fx.ToEnumString());
            }
        }

        #endregion

        public void Dispose()
        {
            Instance = null;
        }
    }

    public enum SFX
    {
        None = 0, //없음

        sh_transition, // 로비,인게임 화면 전환 사운드
        sh_common_click, // 공용 버튼 클릭 사운드
        sh_common_popup, // 팝업 열기 / 닫기
        sh_bubble_break, //일정 시간 지나 버블 터질 때
        sh_bubble_gem_remove, // 젬을 사용해서 버브 터트릴때
        sh_unpack_box, // 거미줄 해제시 주변 상자 벗겨질때
        sh_level_up, // 왼쪽 상단 레벨업 마크 숫자 올라갈 때
        sh_level_up_btn_click, // 레벨업 클릭시
        sh_tap, // 탭했을 때
        sh_tap_energy, // 에너지 탭했을 때
        sh_tap_coin, // 코인 탭했을 때
        sh_tap_jewel, // 쥬얼 탭했을 때
        sh_tap_xp, // 경험치 탭했을 때 (별)
        sh_tap_timeskip, // 타임스킵
        sh_merge_1, // 머지 효과음 1단계
        sh_merge_2, // 머지 효과음 2단계
        sh_merge_3, // 머지 효과음 3단계
        sh_merge_4, // 머지 효과음 4단계
        sh_merge_5, // 머지 효과음 5단계
        sh_merge_6, // 머지 효과음 6단계
        sh_merge_7, // 머지 효과음 7단계
        sh_merge_8, // 머지 효과음 8단계 이상
        
        sh_merge_1_b, // 머지 효과음 1단계
        sh_merge_2_b, // 머지 효과음 2단계
        sh_merge_3_b, // 머지 효과음 3단계
        sh_merge_4_b, // 머지 효과음 4단계
        sh_merge_5_b, // 머지 효과음 5단계
        sh_merge_6_b, // 머지 효과음 6단계
        sh_merge_7_b, // 머지 효과음 7단계
        sh_merge_8_b, // 머지 효과음 8단계 이상
        
        sh_xp_merge_1, // XP 머지 효과음 1단계
        sh_xp_merge_2, // XP 머지 효과음 2단계
        sh_xp_merge_3, // XP 머지 효과음 3단계
        sh_xp_merge_4, // XP 머지 효과음 4단계
        sh_xp_merge_5, // XP 머지 효과음 5단계
        sh_xp_spawn, // 특정 단계 이후 경험치 (별) 획득시
        
        sh_mission_complete, // 미션 컴플릿 사운드

        ProduceItem, //아이템 생산
        QuestSuccess, //퀘스트 성공
        
        sh_inven_in,    // 인벤 넣기
        sh_inven_out,   // 인벤 빼기
        
        sh_send_gift,   // 친구 선물 보내기
        
        sh_evt_web_remove, // 꽃밭 공장 거미줄 제거될 때
        sh_evt_factory_start, // 꽃밭 공장 가동될때
    }

    public enum LobbySFX
    {
        None = 0, //없음

        sh_leaves_clean, //낙엽 쓸기
        sh_plant,   //꽃심기
        CuttingTree, //나무 베기
        Making, //뚝딱뚝딱
    }
}