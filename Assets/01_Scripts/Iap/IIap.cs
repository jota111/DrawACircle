using UniRx;

namespace SH.Iap
{
    public interface IIap
    {
        /// <summary>
        /// 초기화 되었는가?
        /// </summary>
        /// <returns></returns>
        bool IsInit();

        /// <summary>
        /// iap 정보 설정
        /// </summary>
        /// <param name="info"></param>
        void SetInfo(IIapInfo info);

        /// <summary>
        /// 초기화
        /// </summary>
        void Init();

        /// <summary>
        /// 로컬라이징 가격
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string LocalizedPrice(string id);

        /// <summary>
        /// rx 현지화 가격 얻기
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IReadOnlyReactiveProperty<string> ObsLocalizedPrice(string id);

        /// <summary>
        /// 결제 요청
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        void Purchase(string id, IapResult callback = null);

        /// <summary>
        /// 복구
        /// </summary>
        void Restore();

        /// <summary>
        /// 타입
        /// </summary>
        /// <returns></returns>
        IapType IapType();
    }
}