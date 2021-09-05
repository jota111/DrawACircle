using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using I2.Loc;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace OutGameCore
{
    public class GameUtils : MonoBehaviour
    {
        private static System.Random _sysRand;

        public static System.Random SysRand
        {
            get
            {
                if (_sysRand == null)
                    _sysRand = new System.Random();
                return _sysRand;
            }
        }

        public static bool IsLog = true;

        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("__DEV")]
        public static void Log(string l)
        {
            if (GameUtils.IsLog == true)
                Debug.Log(l);
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("__DEV")]
        public static void Error(string l)
        {
           if (GameUtils.IsLog == true)
            {
                Debug.LogError(l);
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("__DEV")]
        public static void Warning(object l)
        {
            if (GameUtils.IsLog == true)
            {
                Debug.LogWarning(l);
            }
        }

        //[System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("__DEV")]
        public static void LogException(Exception ex)
        {
            #if __DEV
            if (GameUtils.IsLog == true)
            {
                Debug.LogError($"{ex.GetType().FullName}\n{ex.Message}\n{ex.Source}\n{ex.StackTrace}");
            }
            #else
            Debug.LogException(ex);
            #endif
        }

        // 선택한 오브젝트 + 자식포함 이름 검색해서 찾기
        public static void FindGameObjectNameIndex(Transform p, string name, ref Transform target)
        {
            for (int i = 0; i < p.childCount; i++)
            {
                if (target == null)
                {
                    if (p.GetChild(i).name.IndexOf(name) > -1)
                    {
                        target = p.GetChild(i);
                        break;
                    }

                    if (p.GetChild(i).childCount > 0)
                    {
                        FindGameObjectNameIndex(p.GetChild(i), name, ref target);
                    }
                }
            }
        }

        // 선택한 오브젝트 + 자식포함 이름 매칭해서 찾기
        public static void FindGameObjectNameEquals(Transform p, string name, ref Transform target)
        {
            for (int i = 0; i < p.childCount; i++)
            {
                if (target == null)
                {
                    if (p.GetChild(i).name.Equals(name) == true)
                    {
                        target = p.GetChild(i);
                        break;
                    }

                    if (p.GetChild(i).childCount > 0)
                    {
                        FindGameObjectNameEquals(p.GetChild(i), name, ref target);
                    }
                }
            }
        }

        public static Vector2 RandomPointInBox(BoxCollider2D c)
        {
            if (c.enabled == false) c.enabled = true;
            var bounds = c.bounds;
            //GameUtils.Log($"RandomPointInBox {c.name} {bounds.center} {bounds.min.x} {bounds.max.x}");
            Bounds bounds1;
            return new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y));
        }

        public static Vector3 RandomPointInBox(Collider c)
        {
            if (c.enabled == false) c.enabled = true;
            var bounds = c.bounds;
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x), 0,
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
        }

        public static Vector2 RandomPointInBox2D(Collider2D c)
        {
            if (c.enabled == false) c.enabled = true;
            var bounds = c.bounds;
            return new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y));
        }

        public static Vector2 RandomPointInBox2DFromCenter(Collider2D c)
        {
            if (c == null) return Vector2.zero;
            if (c.enabled == false) c.enabled = true;
            var bounds = c.bounds;
            return new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x) - bounds.center.x,
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y) - bounds.center.y);
        }

        public static Vector3 RandomPointInBoxFromCenter(Collider c)
        {
            if (c == null) return Vector2.zero;
            if (c.enabled == false) c.enabled = true;
            var bounds = c.bounds;
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x) - bounds.center.x,
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y) - bounds.center.y,
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z) - bounds.center.z);
        }

        //해당 콜라이더 안에 포함되는지
        public static bool IsPointWithinCollider(Collider collider, Vector3 point)
        {
            return (collider.ClosestPoint(point) - point).sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon;
        }

        public static Vector3 RandomPositionInCircle(float rad = 5.5f)
        {
            Vector3 pos = UnityEngine.Random.insideUnitCircle * rad;
            pos.z = pos.y;
            pos.y = 0;
            return pos;
        }

        public static Vector3 RandomPositionInBox(int offset)
        {
            Vector3 pos = new Vector3(Random.Range(-offset, offset + 1), 0, Random.Range(-offset, offset + 1));
            return pos;
        }

        public static Vector3 RandomPositionInBoxExceptCollision<T>(BoxCollider boxCollider, float collisionRad = 2f)
        {
            Vector3 pos = RandomPointInBox(boxCollider);
            Collider[] overlaped = Physics.OverlapSphere(pos, collisionRad);
            int c = 0;
            while (ComponentHitCheck<T>(overlaped, arg => { return arg.Equals(boxCollider) == false; }))
            {
                pos = RandomPointInBox(boxCollider);
                overlaped = Physics.OverlapSphere(pos, collisionRad);

                if (++c > 30) break;
            }

            return pos;
        }

        public static Vector2 RandomPosition2DInBoxExceptCollision<T>(BoxCollider2D boxCollider2D, float collisionRad = 2f)
        {
            Vector2 pos = RandomPointInBox(boxCollider2D);
            Collider2D[] overlaped = Physics2D.OverlapCircleAll(pos, collisionRad);
            int c = 0;
            while (ComponentHitCheck<T>(overlaped))
            {
                pos = RandomPointInBox(boxCollider2D);
                overlaped = Physics2D.OverlapCircleAll(pos, collisionRad);

                if (++c > 30) break;
            }

            return pos;
        }

        public static bool SphereOverlapedOther<T>(Vector3 pos, float radius)
        {
            Collider[] overlaped = Physics.OverlapSphere(pos, radius);
            return ComponentHitCheck<T>(overlaped);
        }

        public static bool AheadOther<T>(Vector3 pos, Vector3 dir, float distance)
        {
            RaycastHit[] hits = Physics.RaycastAll(pos, dir, distance);
            return ComponentHitCheck<T>(hits);
        }

        public static bool AheadOther2D<T>(Vector3 pos, Vector3 dir, float distance)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(pos, dir, distance);
            return ComponentHitCheck<T>(hits);
        }

        public static bool ComponentHitCheck<T>(Collider[] colls, Func<T, bool> checker = null)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].transform.GetComponentInParent<T>() != null &&
                    (checker == null || checker(colls[i].transform.GetComponentInParent<T>()))) return true;
                if (colls[i].transform.GetComponent<T>() != null &&
                    (checker == null || checker(colls[i].transform.GetComponentInParent<T>()))) return true;
            }

            return false;
        }

        public static bool ComponentHitCheck<T>(Collider2D[] colls)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].transform.GetComponentInParent<T>() != null) return true;
                if (colls[i].transform.GetComponent<T>() != null) return true;
            }

            return false;
        }

        public static bool ComponentHitCheck<T>(RaycastHit[] hits)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponentInParent<T>() != null) return true;
                if (hits[i].transform.GetComponent<T>() != null) return true;
            }

            return false;
        }

        public static bool ComponentHitCheck<T>(RaycastHit2D[] hits)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponentInParent<T>() != null) return true;
                if (hits[i].transform.GetComponent<T>() != null) return true;
            }

            return false;
        }

        public static Transform FindNearestObject<T>(T[] objects, T target, float range) where T : MonoBehaviour
        {
            T nearest = null;
            float minDist = range;

            for (int i = 0; i < objects.Length; i++)
            {
                if (target != objects[i])
                {
                    float dist = (target.transform.position - objects[i].transform.position).magnitude;
                    if (dist < minDist)
                    {
                        nearest = objects[i];
                        minDist = dist;
                    }
                }
            }

            return nearest?.transform;
        }

        public static Transform FindNearestObject(Transform[] objects, Transform target, float range)
        {
            Transform nearest = null;
            float minDist = range;

            for (int i = 0; i < objects.Length; i++)
            {
                if (target != objects[i])
                {
                    float dist = (target.position - objects[i].position).magnitude;
                    if (dist < minDist)
                    {
                        nearest = objects[i];
                        minDist = dist;
                    }
                }
            }

            return nearest;
        }

        public static Quaternion GetRotationStaringCamera(Camera camera)
        {
            var value = Quaternion.LookRotation(camera.transform.forward, camera.transform.up);
            return value;
        }

        public static void I2SetFont(Text text, string textData)
        {
            if (text == null) return;
            var localize = text.GetComponent<Localize>();
            if (localize == null)
                localize = text.gameObject.AddComponent<Localize>();
            localize.SetTerm("", "Font_Normal");
            text.text = textData;
        }

        public static void I2FormatAnimation(Text text, string key, params object[] args)
        {
            if (text == null)
                return;

            if (CheckI2Key(key) == false)
            {
#if UNITY_EDITOR
                Debug.LogError($"not found I2 Localization : {key}");
#else
                Debug.LogWarning($"not found I2 Localization : {key}");
#endif
                text.text = $"<color=red>{key}</color>";
                return;
            }

            var localize = text.GetComponent<Localize>();
            if (localize == null)
                localize = text.gameObject.AddComponent<Localize>();

            localize.SetTerm(key, "Font_Normal");
            text.text = string.Empty;
            var msg = I2Format(key, args);
            var len = msg?.Length ?? 0;

            // 한글자당 속도
            const float speed = 0.035f;
            text.text = string.Empty;
            text.DOText(msg, len * speed);
        }

        public static void I2Format(Text text, string key, params object[] args)
        {
            // text.text = LocalizationManager.GetTranslation(key);

            if (CheckI2Key(key) == false)
            {
#if UNITY_EDITOR
                Debug.LogError($"not found I2 Localization : {key}");
#else
                Debug.LogWarning($"not found I2 Localization : {key}");
#endif
                text.text = $"<color=red>{key}</color>";
                return;
            }


            var localize = text.GetComponent<Localize>();
            if (localize == null)
                localize = text.gameObject.AddComponent<Localize>();

            localize.SetTerm(key, "Font_Normal");
            text.text = I2Format(key, args);
            // if (args != null && args.Length > 0)
            //     I2SetLocalizeParam(text, args);
        }

        public static void I2FormatSetParams(Text text, params object[] args)
        {
            var localize = text.GetComponent<Localize>();
            if (localize == null)
                localize = text.gameObject.AddComponent<Localize>();

            localize.GetFinalTerms(out var key, out var second);
            text.text = I2Format(key, args);
        }


        public static string I2Format(string key, params object[] args)
        {
            string msg = LocalizationManager.GetTranslation(key);
            if (string.IsNullOrEmpty(msg))
            {
#if UNITY_EDITOR
                Debug.LogError($"not found I2 Localization : {key}");
#else
                Debug.LogWarning($"not found I2 Localization : {key}");
#endif
                return $"<color=red>{key}</color>";
            }

            try
            {
                msg = string.Format(msg, args);
            }
            catch (FormatException ex)
            {
                Error($"I2Format Exception {msg} {args}");
                LogException(ex);
            }

            return msg;
        }

        public static bool CheckI2Key(string key)
        {
            string msg = LocalizationManager.GetTranslation(key);
            return msg != null;
        }

        public static bool CheckI2Key(string key, string language)
        {
            string msg = LocalizationManager.GetTranslation(key, overrideLanguage:language);
            return msg != null;
        }

        // public static void I2SetLocalizeParam(Text text, params object[] args)
        // {
        //     var localeManager = text.GetComponent<LocalizationParamsManager>();
        //     if (localeManager == null)
        //         localeManager = text.gameObject.AddComponent<LocalizationParamsManager>();
        //
        //     for (int i = 0; i < args.Length; i++)
        //     {
        //         if (i < args.Length - 1)
        //             localeManager.SetParameterValue(i.ToString(), args[i].ToString(), false);
        //         else
        //             localeManager.SetParameterValue(i.ToString(), args[i].ToString(), true);
        //     }
        // }

        public static string GetSubscriptionLocalization(string productID, string price)
        {
            if (productID.Contains("subscription"))
                return I2Format("SubscriptionPriceInfo_Week", price);
            return price;
        }

        public static string StringConcat(params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
                sb.Append(args[i]);
            return sb.ToString();
        }

        public static int RandomWeightedList(IEnumerable<int> list)
        {
            int rand = RandomRange(0, list.Sum());
            int v = 0;
            for (int i = 0; i < list.Count(); i++)
            {
                v += list.ElementAt(i);
                if (v > rand) return i;
            }

            return list.Count() - 1;
        }

        /// <summary>
        /// ratio가 0.1이면 true일 확률이 10%
        /// </summary>
        public static bool RandomBoolFromPercent(float ratio)
        {
            var r = SysRand.NextDouble();
            return r <= ratio;
        }

        public static bool RandomBool()
        {
            return RandomBoolFromPercent(0.5f);
        }

        public static T RandomItem<T>(IEnumerable<T> list)
        {
            if (!list.Any())
                return default(T);
            int index = SysRand.Next(0, list.Count());
            return list.ElementAt(index);
        }

        public static int RandomRange(int min, int max)
        {
            double val = SysRand.NextDouble();
            int item = 0;
            val *= max - min;
            item = ((int) val + min);
            return item;
        }

        public static float RandomRange(float min, float max)
        {
            double val = SysRand.NextDouble();
            float item = 0f;
            val *= max - min;
            item = (float) (val + min);
            return item;
        }

        #region 아이콘 가져오기 (Async)

        // 아이템 등급별 BG 가져오기
        public static ResourceRequest GetItemBGAsync(int grade)
        {
            return Resources.LoadAsync<Sprite>($"BG/Img_ItemBg_Grade_{grade}");
        }

        // 아이템 아이콘 가져오기
        public static ResourceRequest GetItemIconFromTypeAsync(string type)
        {
            return Resources.LoadAsync<Sprite>($"Icons/Items/{type}");
        }

        public static async UniTask<GameObject> GetResource(string src) // where T : Object
        {
            var asset = await Resources.LoadAsync<GameObject>(src);
            if (asset == null) GameUtils.Error($"GetResource is null {src}");
            var loadItem = Instantiate(asset) as GameObject;
            return loadItem;
        }

        public static async UniTask<T> GetResource<T>(string src) where T : Object
        {
            var asset = await Resources.LoadAsync<T>(src);
            var loadItem = Instantiate(asset) as T;
            return loadItem;
        }

        #endregion

        public static float GetRoll(Transform transform)
        {
            var fwd = transform.forward;
            fwd.y = 0;
            fwd *= Mathf.Sign(transform.up.y);
            var right = Vector3.Cross(Vector3.up, fwd).normalized;
            float roll = Vector3.Angle(right, transform.right) * Mathf.Sign(transform.right.y);
            return roll;
        }

        public static void SetLayerRecursively(GameObject go, int layerNumber)
        {
            foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layerNumber;
            }
        }

        public static void SetTagRecursively(GameObject go, string tag)
        {
            foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.tag = tag;
            }
        }

        /// <summary>
        /// 해당년도 주차 가져오기. 
        /// </summary>
        public static int GetGameTimeOfWeek()
        {
            var curTime = GetGameTime();
            DateTime standardDate = new DateTime(curTime.Year, 1, 1);
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            int weekNumber = calendar.GetWeekOfYear(curTime, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) -
                calendar.GetWeekOfYear(standardDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1;

            return weekNumber;
        }

        // 게임타임 가져오기
        public static DateTime GetGameTime()
        {
            var time = DateTime.Now;
            if (OutGame.Instance != null && OutGame.Instance.test_dateTime_enabled)
            {
                time = time.AddHours(OutGame.Instance.test_deltaTimeHour).AddMinutes(OutGame.Instance.test_deltaTimeMinute);
            }
#if UNITY_EDITOR
#endif
            return time;
        }

        public static int GetServerTimeMin()
        {
            return (int) (GetGameTime().Ticks / TimeSpan.TicksPerMinute);
        }

        public static int GetServerTimeSec()
        {
            return (int) (GetGameTime().Ticks / TimeSpan.TicksPerSecond);
        }

        public static long GetServerTimeMs()
        {
            return GetGameTime().Ticks / TimeSpan.TicksPerMillisecond;
        }

        // 초를 분으로 수정
        public static int SecToMin(int sec)
        {
            return sec / 60;
        }

        //변수가 현재 게임타임보다 크면 참
        public static bool IsBiggerThanGameTime(DateTime timeValue)
        {
            return GetGameTime().Ticks < timeValue.Ticks;
        }

        //변수와 게임타임 사이의 차이를 반환 : 일/시간/분
        public static (int, int, int) GetCompareTimeTuple(long timeValueTicks)
        {
            if (timeValueTicks == 0) return (0, 0, 0);
            var ticks = timeValueTicks - GetGameTime().Ticks;
            if (ticks < DateTime.MinValue.Ticks) ticks = DateTime.MinValue.Ticks;
            else if (ticks > DateTime.MaxValue.Ticks) ticks = DateTime.MaxValue.Ticks;
            return (new DateTime(timeValueTicks).Subtract(GetGameTime()).Days, new DateTime(ticks).Hour, new DateTime(ticks).Minute);
        }

        //변수와 게임타임 사이의 차이를 반환 : 일/시간
        public static (int, int) GetCompareTime(DateTime timeValue)
        {
            return (timeValue.Subtract(GetGameTime()).Days, new DateTime(timeValue.Ticks - GetGameTime().Ticks).Hour);
        }

        //변수와 게임타임 사이의 차이를 반환 : 시간/분
        public static (int, int) GetCompareTimeMinutes(DateTime timeValue)
        {
            var ticks = timeValue.Ticks - GetGameTime().Ticks;
            if (ticks < DateTime.MinValue.Ticks) ticks = DateTime.MinValue.Ticks;
            else if (ticks > DateTime.MaxValue.Ticks) ticks = DateTime.MaxValue.Ticks;
            return ((int) timeValue.Subtract(GetGameTime()).TotalHours, new DateTime(ticks).Minute);
        }

        //변수와 게임타임 사이의 차이를 반환 : 초
        public static long GetCompareTimeSeconds(DateTime timeValue)
        {
            var ticks = timeValue.Ticks - GetGameTime().Ticks;
            if (ticks < DateTime.MinValue.Ticks) ticks = DateTime.MinValue.Ticks;
            else if (ticks > DateTime.MaxValue.Ticks) ticks = DateTime.MaxValue.Ticks;
            return new DateTime(ticks).Second;
        }

        //변수와 게임타임 사이의 차이를 반환 : 초
        public static double GetCompareTimeTotalSeconds(DateTime timeValue)
        {
            var ticks = timeValue.Ticks - GetGameTime().Ticks;
            if (ticks < DateTime.MinValue.Ticks) ticks = DateTime.MinValue.Ticks;
            else if (ticks > DateTime.MaxValue.Ticks) ticks = DateTime.MaxValue.Ticks;
            return TimeSpan.FromTicks(ticks).TotalSeconds;
            // return (timeValue - GetGameTime()).TotalSeconds;
        }

        public static (int, int) GetCompareBeforeTime(DateTime timeValue)
        {
            return (GetGameTime().Subtract(timeValue).Days, new DateTime(GetGameTime().Ticks - timeValue.Ticks).Hour);
        }

        //변수와 게임타임 사이의 차이를 반환 : 시간/분
        public static (int, int) GetCompareBeforeTimeMinutes(DateTime timeValue)
        {
            var ticks = GetGameTime().Ticks - timeValue.Ticks;
            if (ticks < DateTime.MinValue.Ticks) ticks = DateTime.MinValue.Ticks;
            else if (ticks > DateTime.MaxValue.Ticks) ticks = DateTime.MaxValue.Ticks;
            return ((int) GetGameTime().Subtract(timeValue).TotalHours, new DateTime(ticks).Minute);
        }

        //변수와 게임타임 사이의 차이를 반환 : 초
        public static long GetCompareBeforeTimeSeconds(DateTime timeValue)
        {
            var ticks = GetGameTime().Ticks - timeValue.Ticks;
            if (ticks < DateTime.MinValue.Ticks) ticks = DateTime.MinValue.Ticks;
            else if (ticks > DateTime.MaxValue.Ticks) ticks = DateTime.MaxValue.Ticks;
            return new DateTime(ticks).Second;
        }

        //변수와 게임타임 사이의 차이를 반환 : 초
        public static long GetCompareBeforeTimeTotalSeconds(DateTime timeValue)
        {
            var sec = (long) GetGameTime().Subtract(timeValue).TotalSeconds;
            return sec;
        }

        //현재 게임타임에서 초가 더해진 DateTime 반환
        public static DateTime GetGameTimeAddedSecond(float secValue)
        {
            DateTime temp = GetGameTime();
            return temp.AddSeconds(secValue);
        }

        //현재 게임타임에서 시간/분이 더해진 DateTime 반환
        public static DateTime GetGameTimeAddedHourMinute((int, int) timeValue)
        {
            DateTime temp = GetGameTime();
            temp = temp.AddHours(timeValue.Item1);
            temp = temp.AddMinutes(timeValue.Item2);
            return temp;
        }

        //현재 게임타임에서 일/시간이 더해진 DateTime 반환
        public static DateTime GetGameTimeAddedDayHour((int, int) timeValue)
        {
            DateTime temp = GetGameTime();
            temp = temp.AddDays(timeValue.Item1);
            temp = temp.AddHours(timeValue.Item2);
            return temp;
        }

        // 일 단위로 내림처리된 게임시간 반환 (n일 00시) -> 로컬시간 표시
        public static DateTime GetFlooredGameTime_Day_LocalTime()
        {
            var time = GetFlooredGameTime_Day();
            return new DateTime(time.Ticks - DateTime.UtcNow.Ticks + DateTime.Now.Ticks);
        }

        public static double GetFlooredDiffDays(DateTime targetDate)
        {
            var day = GameUtils.GetFlooredGameTime_Day().Subtract(GameUtils.GetFlooredTime_Day(targetDate));
            return day.TotalDays;
        }

        // 일 단위로 내림처리된 게임시간 반환 (n일 00시)
        public static DateTime GetFlooredGameTime_Day()
        {
            var time = GetGameTime();
            return GetFlooredTime_Day(time);
        }

        // 주 단위로 내림처리된 게임시간 (월요일 00시)
        public static DateTime GetFlooredGameTime_Week()
        {
            var time = GetGameTime();
            return GetFlooredTime_Week(time);
        }

        // 월 단위로 내림처리된 게임시간 반환 (n월 1일 00시)
        public static DateTime GetFlooredGameTime_Month()
        {
            var time = GetGameTime();
            return GetFlooredTime_Month(time);
        }

        // 일 단위로 내림처리된 DateTime 반환
        public static DateTime GetFlooredTime_Day(DateTime time)
        {
            long delta = time.Ticks % TimeSpan.FromDays(1).Ticks;
            return time.AddTicks(-delta);
        }

        // 주 단위로 내림처리된 DateTime 반환 (일요일 00시)
        public static DateTime GetFlooredTime_Week(DateTime time)
        {
            DateTime sunDay = new DateTime(time.Year, time.Month, time.Day).AddDays(-(int) time.DayOfWeek);
            return sunDay;
        }

        // 월 단위로 내림처리된 DateTime 반환 (n월 1일 00시)
        public static DateTime GetFlooredTime_Month(DateTime time)
        {
            long deltaDay = TimeSpan.FromDays(time.Day - 1).Ticks;
            long delta = time.Ticks % TimeSpan.FromDays(1).Ticks;
            return time.AddTicks(-deltaDay).AddTicks(-delta);
        }

        public static string GetDuration_Min(float _min)
        {
            if (_min >= 1440)
            {
                string day = I2Format("DurationInfo_Days", (int) (_min / 1440));
                string hour = I2Format("DurationInfo_Hours", (int) ((_min / 60) % 24));
                string min = I2Format("DurationInfo_Minutes", (int) (_min % 60));
                string item = "";
                if (string.IsNullOrEmpty(day) == false && (int) (_min / 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += day;
                    else
                        item += $" {day}";
                if (string.IsNullOrEmpty(hour) == false && (int) ((_min / 60) % 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += hour;
                    else
                        item += $" {hour}";
                if (string.IsNullOrEmpty(min) == false && (int) (_min % 60) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += min;
                    else
                        item += $" {min}";
                // item += I2Format("DurationInfo_Description");
                return item;
            }
            else if (_min >= 60)
            {
                string hour = I2Format("DurationInfo_Hours", (int) ((_min / 60) % 24));
                string min = I2Format("DurationInfo_Minutes", (int) (_min % 60));
                string item = "";
                if (string.IsNullOrEmpty(hour) == false && (int) ((_min / 60) % 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += hour;
                    else
                        item += $" {hour}";
                if (string.IsNullOrEmpty(min) == false && (int) (_min % 60) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += min;
                    else
                        item += $" {min}";
                // item += I2Format("DurationInfo_Description");
                return item;
            }
            else
            {
                string min = I2Format("DurationInfo_Minutes", (int) (_min % 60));
                // min += I2Format("DurationInfo_Description");
                return min;
            }
        }

        public static string GetTwoDuration_Min(float _min)
        {
            if (_min >= 1440)
            {
                string day = I2Format("DurationInfo_Days", (int) (_min / 1440));
                string hour = I2Format("DurationInfo_Hours", (int) ((_min / 60) % 24));
                string item = "";
                if (string.IsNullOrEmpty(day) == false && (int) (_min / 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += day;
                    else
                        item += $" {day}";
                if (string.IsNullOrEmpty(hour) == false && (int) ((_min / 60) % 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += hour;
                    else
                        item += $" {hour}";
                // item += I2Format("DurationInfo_Description");
                return item;
            }
            else if (_min >= 60)
            {
                string hour = I2Format("DurationInfo_Hours", (int) ((_min / 60) % 24));
                string min = I2Format("DurationInfo_Minutes", (int) (_min % 60));
                string item = "";
                if (string.IsNullOrEmpty(hour) == false && (int) ((_min / 60) % 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += hour;
                    else
                        item += $" {hour}";
                if (string.IsNullOrEmpty(min) == false && (int) (_min % 60) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += min;
                    else
                        item += $" {min}";
                // item += I2Format("DurationInfo_Description");
                return item;
            }
            else
            {
                string min = I2Format("DurationInfo_Minutes", (int) (_min % 60));
                // min += I2Format("DurationInfo_Description");
                return min;
            }
        }

        public static string GetOneDuration_Min(double _min)
        {
            if (_min >= 1440)
            {
                string day = I2Format("DurationInfo_Days", (int) (_min / 1440));
                string item = "";
                if (string.IsNullOrEmpty(day) == false && (int) (_min / 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += day;
                    else
                        item += $" {day}";
                return item;
            }
            else if (_min >= 60)
            {
                string hour = I2Format("DurationInfo_Hours", (int) ((_min / 60) % 24));
                string item = "";
                if (string.IsNullOrEmpty(hour) == false && (int) ((_min / 60) % 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += hour;
                    else
                        item += $" {hour}";
                return item;
            }
            else
            {
                string min = I2Format("DurationInfo_Minutes", (int) (_min % 60));
                // min += I2Format("DurationInfo_Description");
                return min;
            }
        }

        public static string GetTwoDuration_Sec(float _sec)
        {
            var _min = _sec / 60; 
            if (_min >= 1440)
            {
                string day = I2Format("DurationInfo_Days", (int) (_min / 1440));
                string hour = I2Format("DurationInfo_Hours", (int) ((_min / 60) % 24));
                string item = "";
                if (string.IsNullOrEmpty(day) == false && (int) (_min / 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += day;
                    else
                        item += $" {day}";
                if (string.IsNullOrEmpty(hour) == false && (int) ((_min / 60) % 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += hour;
                    else
                        item += $" {hour}";
                // item += I2Format("DurationInfo_Description");
                return item;
            }
            else if (_min >= 60)
            {
                string hour = I2Format("DurationInfo_Hours", (int) ((_min / 60) % 24));
                string min = I2Format("DurationInfo_Minutes", (int) (_min % 60));
                string item = "";
                if (string.IsNullOrEmpty(hour) == false && (int) ((_min / 60) % 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += hour;
                    else
                        item += $" {hour}";
                if (string.IsNullOrEmpty(min) == false && (int) (_min % 60) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += min;
                    else
                        item += $" {min}";
                // item += I2Format("DurationInfo_Description");
                return item;
            }
            else
            {
                string min = I2Format("DurationInfo_Minutes", (int) (_min % 60));
                string sec = I2Format("DurationInfo_Seconds", (int) (_sec % 60));
                string item = "";
                if (string.IsNullOrEmpty(min) == false && (int) ((_min / 60) % 24) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += min;
                    else
                        item += $" {min}";
                if (string.IsNullOrEmpty(sec) == false && (int) (_sec % 60) > 0)
                    if (string.IsNullOrEmpty(item) == true)
                        item += sec;
                    else
                        item += $" {sec}";
                // item += I2Format("DurationInfo_Description");
                return item;
                // min += I2Format("DurationInfo_Description");
                return min;
            }
        }

        public static string GetDuration_Sec(float _sec)
        {
            string sec = I2Format("DurationInfo_Seconds", (int) (_sec % 60));
            // min += I2Format("DurationInfo_Description");
            return sec;
        }

        /// <summary>
        /// 원의 중점과 반지름을 받아 랜덤 위치 반환
        /// </summary>
        /// <param name="center">원의 중점</param>
        /// <param name="radius">반지름</param>
        /// <returns>랜덤 위치</returns>
        public static Vector3 RandomCircle(Vector3 center, float radius)
        {
            float ang = UnityEngine.Random.value * 360;
            Vector3 pos;
            pos.x = radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.x = UnityEngine.Random.Range(0, pos.x);
            pos.x += center.x;
            pos.y = center.y;
            pos.z = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.z = UnityEngine.Random.Range(0, pos.z);
            pos.z += center.z;
            return pos;
        }

        /// <summary>
        /// 원의 중점과 반지름을 받아 랜덤 위치 반환
        /// </summary>
        /// <param name="center">원의 중점</param>
        /// <param name="radius">반지름</param>
        /// <returns>랜덤 위치</returns>
        public static Vector3 RandomCircle2D(Vector3 center, float radius)
        {
            float ang = UnityEngine.Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.x = UnityEngine.Random.Range(center.x, pos.x);
            pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.y = UnityEngine.Random.Range(center.y, pos.y);
            pos.z = center.z;
            return pos;
        }

        public static Vector3 RandomCircleOutline2D(Vector3 center, float radius)
        {
            float ang = UnityEngine.Random.value * 360;
            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            pos.z = center.z;
            return pos;
        }

        /// <summary>
        /// 두 원의 교점 리스트 반환
        /// </summary>
        /// <returns></returns>
        public static List<Vector3> GetIntersectionPointsCirvles(Vector3 firstCenter, float firstRadius,
            Vector3 secondCenter, float secondRadius)
        {
            var X = secondCenter.x - firstCenter.x;
            var Z = secondCenter.z - firstCenter.z;
            var D = Math.Sqrt((X * X) + (Z * Z));
            var T1 = Math.Acos((firstRadius * firstRadius - secondRadius * secondRadius + D * D) / (2 * firstRadius * D));
            var T2 = Math.Atan(Z / X);


            var T3 = (float) (firstCenter.x + firstRadius * Math.Cos(T2 + T1));
            var T4 = (float) (firstCenter.z + firstRadius * Math.Sin(T2 + T1));


            var T5 = (float) (firstCenter.x + firstRadius * Math.Cos(T2 - T1));
            var T6 = (float) (firstCenter.z + firstRadius * Math.Sin(T2 - T1));

            if (D < firstRadius + secondRadius)
            {
                if (float.IsNaN(T3))
                {
                }

                return new List<Vector3>()
                {
                    new Vector3(T3, 0, T4),
                    new Vector3(T5, 0, T6),
                };
            }
            else if (D == firstRadius + secondRadius)
            {
                return new List<Vector3>()
                {
                    new Vector3(T3, 0, T4),
                };
            }
            else if (D > firstRadius + secondRadius)
            {
                return new List<Vector3>();
            }

            return new List<Vector3>();
        }

        /// <summary>
        /// 카메라에서 비추고있는 오브젝트의 상대적 위치 반환 
        /// </summary>
        public static Vector3 CameraViewportToCameraScreenPoint(Camera from, Camera to, Transform target)
        {
            Vector3 object3dCam = from.WorldToViewportPoint(target.position);
            return to.ViewportToScreenPoint(object3dCam);
        }

        /// <summary>
        /// 해당 트랜스폼 아래에 있는 스프라이트들의 레이어 설정
        /// </summary>
        /// <param name="root"></param>
        /// <param name="index"></param>
        public static void SetSpriteOrderinLayer(Transform root, int index)
        {
            if (root.GetComponent<SpriteRenderer>() != null) root.GetComponent<SpriteRenderer>().sortingOrder = index;
            for (int i = 0; i < root.childCount; i++)
            {
                foreach (SpriteRenderer sprite in root.GetChild(i).GetComponentsInChildren<SpriteRenderer>(true))
                {
                    sprite.sortingOrder = index;
                }

                if (root.GetChild(i).childCount != 0) SetSpriteOrderinLayer(root.GetChild(i), index);
            }
        }

        /// <summary>
        /// float 리스트를 10곱해서 Int리스트로 반환
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<int> FloatToInt10(List<float> index)
        {
            List<int> temp = new List<int>();
            foreach (var item in index)
            {
                temp.Add((int) (item * 10));
            }

            return temp;
        }

        /// <summary>
        /// 겹치지 않는 랜덤 수를 불러오기
        /// </summary>
        /// <param name="Range">범위</param>
        /// <param name="NotThisIndex">현재 리스트</param>
        /// <returns>겹치지 않는 랜덤 수</returns>
        public static int GetNewIndex(int Range, List<int> NotThisIndex)
        {
            int newDir;

            newDir = UnityEngine.Random.Range(0, Range);
            for (int i = 0; i < NotThisIndex.Count; i++)
            {
                if (newDir == NotThisIndex[i])
                {
                    newDir = GetNewIndex(Range, NotThisIndex);
                }
            }

            return newDir;
        }

        public static int GetNewIndex(List<int> pools, List<int> NotThisIndex)
        {
            int newDir = 0;

            try
            {
                var list = new List<int>();
                for (int i = 0; i < pools.Count; i++)
                {
                    bool check = true;
                    foreach (var item in NotThisIndex)
                    {
                        if (pools[i] == item)
                        {
                            check = false;
                            break;
                        }
                    }

                    if (check)
                        list.Add(pools[i]);
                }

                if (list.Count == 0)
                {
                    GameUtils.Error($"GetNewIndex NotThisIndex Error {NotThisIndex.Count} >= {pools.Count}");
                    return 0;
                }

                newDir = RandomRange(0, list.Count);

                return pools.FindIndex(x => x == list[newDir]);
            }
            catch (Exception ex)
            {
                GameUtils.Error($"GetNewIndex Error {newDir} {pools.Count}");
                GameUtils.LogException(ex);
                return 0;
            }
        }

        public static int GetNewIndex(int minRange, int maxRange, List<int> NotThisIndex)
        {
            int newDir;

            newDir = RandomRange(minRange, maxRange);
            if (NotThisIndex.Count >= Math.Abs(Math.Abs(maxRange) - Math.Abs(minRange))) return minRange;
            for (int i = 0; i < NotThisIndex.Count; i++)
            {
                if (newDir == NotThisIndex[i])
                {
                    newDir = GetNewIndex(minRange, maxRange, NotThisIndex);
                }
            }

            return newDir;
        }

        public static float GetNewIndex(float minRange, float maxRange, List<float> NotThisIndex)
        {
            float newDir;

            newDir = UnityEngine.Random.Range(minRange, maxRange);
            if (NotThisIndex.Count >= Math.Abs(Math.Abs(maxRange) - Math.Abs(minRange))) return minRange;
            for (int i = 0; i < NotThisIndex.Count; i++)
            {
                if (newDir == NotThisIndex[i])
                {
                    newDir = GetNewIndex(minRange, maxRange, NotThisIndex);
                }
            }

            return newDir;
        }


        public static int GetNewT<T>(List<T> pools, List<T> NotThisIndex) where T : IComparable
        {
            int newDir = 0;

            try
            {
                var list = new List<T>();
                for (int i = 0; i < pools.Count; i++)
                {
                    bool check = true;
                    foreach (var item in NotThisIndex)
                    {
                        if (pools[i].Equals(item))
                        {
                            check = false;
                            break;
                        }
                    }

                    if (check)
                        list.Add(pools[i]);
                }

                if (list.Count == 0)
                {
                    GameUtils.Error($"GetNewIndex NotThisIndex Error {NotThisIndex.Count} >= {pools.Count}");
                    return 0;
                }

                newDir = RandomRange(0, list.Count);

                return pools.FindIndex(x => x.Equals(list[newDir]));
            }
            catch (Exception ex)
            {
                GameUtils.Error($"GetNewT Error {newDir} {pools.Count}");
                GameUtils.LogException(ex);
                return 0;
            }
        }

        /// <summary>
        /// 인덱스를 섞어서 반환
        /// </summary>
        /// <param name="Range">범위</param>
        /// <returns>섞인 인덱스 리스트</returns>
        public static List<int> GetRandomList(int Range)
        {
            List<int> randomList = new List<int>();
            for (int i = 0; i < Range; i++)
            {
                int item = GetNewIndex(Range, randomList);
                randomList.Add(item);
            }

            return randomList;
        }

        /// <summary>
        /// 두 벡터 사이의 각도 -180~180으로 반환
        /// </summary>
        /// <param name="vStart"></param>
        /// <param name="vEnd"></param>
        /// <returns></returns>
        public static float GetAngle180(Vector3 vStart, Vector3 vEnd)
        {
            Vector3 v = vEnd - vStart;
            return Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        ///     90
        /// 180     0
        ///     -90
        /// </summary>
        /// <param name="vStart"></param>
        /// <param name="vEnd"></param>
        /// <returns></returns>
        public static float GetAngle1802D(Vector3 vStart, Vector3 vEnd)
        {
            Vector3 v = vEnd - vStart;
            return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 두 벡터 사이의 각도 -90~90으로 반환
        /// </summary>
        /// <param name="vStart"></param>
        /// <param name="vEnd"></param>
        /// <returns></returns>
        public static float GetAngle90(Vector3 vStart, Vector3 vEnd)
        {
            Vector3 v = vEnd - vStart;
            float angle = Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
            if (angle > 90) angle = 180 - angle;
            if (angle < -90) angle = -180 - angle;
            return angle;
        }

        //축을 기준으로 from을 angle만큼 회전
        public static Vector3 GetAppliedDirection(float angle, Vector3 @from, Vector3 axis)
        {
            Vector3 resultAngle;
            resultAngle = Quaternion.AngleAxis(angle, axis) * @from;
            return resultAngle;
        }

        //리스트 섞기
        public static List<T> ShuffleList<T>(List<T> list)
        {
            int random1;
            int random2;

            T tmp;

            for (int index = 0; index < list.Count; ++index)
            {
                random1 = UnityEngine.Random.Range(0, list.Count);
                random2 = UnityEngine.Random.Range(0, list.Count);

                tmp = list[random1];
                list[random1] = list[random2];
                list[random2] = tmp;
            }

            return list;
        }

        public static List<T> SumList<T>(List<T> list1, List<T> list2)
        {
            List<T> list = list1;
            list.Capacity += list2.Count;
            foreach (var item in list2)
            {
                list.Add(item);
            }

            return list;
        }

        public static List<T> SumList<T>(List<List<T>> list)
        {
            List<T> tempList = new List<T>();
            foreach (var item1 in list)
            {
                foreach (var item2 in item1)
                {
                    tempList.Add(item2);
                }
            }

            return tempList;
        }

        /// <summary>
        /// 해당방향 2D로 반환
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static Quaternion LookAt2D(Vector3 rot)
        {
            float rot_z = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(new Vector3(0f, 0f, rot_z - 90));
        }

        public static Quaternion LookAt3D(Vector3 rot, float offsetX = 0)
        {
            if (rot == Vector3.zero) return Quaternion.identity;
            //float rot_z = Mathf.Atan2(rot.z, rot.x) * Mathf.Rad2Deg;
            //이 축을 기준으로 회전
            //Vector3.Cross(Vector3.right, rot);
            //Vector3 item = new Vector3(0, 0, rot_z - 90);
            return Quaternion.Euler(Vector3.left * (90 - offsetX))
                   * Quaternion.LookRotation(rot, Vector3.up);
            //GameUtils.GetRotationStaringCamera(InGame.Instance.playerCamera).eulerAngles
            //return Quaternion.Euler(new Vector3(offsetX, 0f, rot_z - 90));
        }

        public static Vector3 LookAt3DVector3(Vector3 rot, float offsetX = 0)
        {
            float rot_z = Mathf.Atan2(rot.z, rot.x) * Mathf.Rad2Deg;
            return new Vector3(offsetX, 0f, rot_z - 90);
        }


        public static List<T> ToListType<T>(List<int> list)
        {
            List<T> items = new List<T>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    items.Add((T) (object) item);
                }
            }

            return items;
        }

        public static string GetHtmlStringFromColor(Color color) => string.Format("{0:X2}{1:X2}{2:X2}",
            Mathf.RoundToInt(color.r), Mathf.RoundToInt(color.g), Mathf.RoundToInt(color.b));

        public static Color GetColorFromHtmlString(string color)
        {
            Color col = new Color();
            ColorUtility.TryParseHtmlString(color, out col);
            return col;
        }

        public static string GetTextMeshProColor(Color color) => $"<color=#{GetHtmlStringFromColor(color)}>";
        public static string GetTextMeshProColor(string color) => $"<color=#{color}>";

        //타이머
        public static IObservable<float> createCountDownObservable(int time)
        {
            if (time < 0)
            {
                time = 0;
                GameUtils.Error($"타이머에 0 이하 값이 들어감!");
            }
            return Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1)).Select(x => (float) (time - x))
                .Take(time + 1);
        }

        public static IObservable<float> createCountDownObservable_Unscaled(int time)
        {
            if (time < 0)
            {
                time = 0;
                GameUtils.Error($"타이머에 0 이하 값이 들어감!");
            }
            return Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1), Scheduler.MainThreadIgnoreTimeScale)
                .Select(x => (float) (time - x))
                .Take(time + 1);
        }

        public static IObservable<long> createCountDownObservable(float time, float duration)
        {
            if (time < 0)
            {
                time = 0;
                GameUtils.Error($"타이머에 0 이하 값이 들어감!");
            }
            int take = (int) (duration / time);
            return Observable.Timer(TimeSpan.FromSeconds(duration - take), TimeSpan.FromSeconds(time))
                .Take(take);
        }

        // 유니크 아이디 생성
        public static string GenerateUniqueID()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 지정된 기간에 포함되는지
        /// 191011 / 191102
        /// </summary>
        /// <param name="date0"></param>
        /// <param name="date1"></param>
        /// <returns></returns>
        public static bool CheckIsOnPeriod(List<int> dates)
        {
            if (dates.Count < 2) return false;
            return CheckIsOnPeriod(dates[0], dates[1]);
        }

        public static bool CheckIsOnPeriod(int date0, int date1)
        {
            return CheckIsOnPeriod(GetGameTime(), date0, date1);
        }

        public static bool CheckIsOnPeriod(DateTime date, int date0, int date1)
        {
            DateTime startTime = DateTime.ParseExact(date0.ToString(), "yyMMdd", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(date1.ToString(), "yyMMdd", CultureInfo.InvariantCulture);
            DateTime current = date;
            if (startTime > current ||
                endTime < current)
                return false;
            return true;
        }
        
        /// <summary>
        /// UI 오브젝트 위에 있는지
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverUIObject() {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        #region string 변환

        public static string Utf16ToUtf8(string utf16String)
        {
            string utf8String = String.Empty;

            //UTF-16 바이트를 배열로 얻어온다.
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);
            //UTF-16 바이트를 배열을 UTF-8로 변환한다.
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // UTF8 bytes 배열 내부에 UTF8 문자를 추가한다.
            for (int i = 0; i < utf8Bytes.Length; i++)
            {
                byte[] utf8Container = new byte[2] {utf8Bytes[i], 0};
                utf8String += BitConverter.ToChar(utf8Container, 0);
            }

            // UTF8을 리턴한다.
            return utf8String;
        }

        public static string Utf16ToUtf8Ansi(string utf16String)
        {
            //UTF-16 바이트를 배열로 얻어온다.
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);

            //UTF-16 바이트를 배열을 UTF-8로 변환한다.
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            //  UTF8 bytes를 ansi로 변환해서 리턴한다.
            return Encoding.Default.GetString(utf8Bytes);
        }

        public static string GetKoreanEncodedString(string item)
        {
            return Encoding.GetEncoding("EUC-KR").GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(item));
        }

        public static string GetKoreanEncodedUTF8(string str)
        {
            string utf8String = String.Empty;
            Encoding encKr = Encoding.GetEncoding("EUC-KR");
            EncodingInfo[] encods = Encoding.GetEncodings();
            Encoding destEnc = Encoding.UTF8;
            foreach (EncodingInfo ec in encods)
            {
                Encoding enc = ec.GetEncoding();
                byte[] sorceBytes = enc.GetBytes(str);
                byte[] encBytes = Encoding.Convert(encKr, destEnc, sorceBytes);
                GameUtils.Log(string.Format("{0}({1}) : {2}", enc.EncodingName, enc.BodyName, destEnc.GetString(encBytes)));
                utf8String += destEnc.GetString(encBytes);
            }

            return utf8String;
        }

        //랜덤 스트링 반환
        public static string GetRandomString(int _totLen)
        {
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";

            var chars = Enumerable.Range(0, _totLen)
                .Select(x => input[SysRand.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }

        //시간 문자열 반환
        public static string GetTimeString(long time)
        {
            TimeSpan ts = TimeSpan.FromTicks(time);
            if (ts.TotalDays >= 1)
            {
                return ts.ToString(@"hh\:mm\:ss");
            }
            else
            {
                return ts.ToString(@"mm\:ss");
            }
        }

        //시간 문자열 반환
        public static string GetTimeStringFromSecond(long sec)
        {
            TimeSpan ts = TimeSpan.FromSeconds(sec);
            if (ts.TotalHours >= 1)
            {
                return ts.ToString(@"hh\:mm\:ss");
            }
            else
            {
                return ts.ToString(@"mm\:ss");
            }
        }

        public static bool CheckStringEmpty(string str)
        {
            return string.IsNullOrEmpty(str) || str.Equals("0");
        }

        #endregion

        #region RectTransform 조정

        public static void SetRectSize(RectTransform rect, float width, float height)
        {
            rect.sizeDelta = new Vector2(width, height);
        }

        public static void SetRectLeft(RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRectRight(RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetRectTop(RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetRectBottom(RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        #endregion

        #region 수열

        /// <summary>
        /// 등차수열의 합
        /// </summary>
        /// <param name="a">초항</param>
        /// <param name="d">등차</param>
        /// <param name="n">개수</param>
        /// <returns></returns>
        public static int GetArithmeticalProgression(int a, int d, int n)
        {
            var value = 0;
            value = (int) ((n * (2 * a + (n - 1) * d)) / 2f);
            return value;
        }

        #endregion

        #region 기타 계산

        public bool IsFarFrom(Transform from, Transform to, float distance)
        {
            bool isFar = (from.transform.position - to.transform.position).sqrMagnitude > distance;
            return isFar;
        }

        public bool IsFarFrom(Vector3 from, Vector3 to, float distance)
        {
            bool isFar = (from - to).sqrMagnitude > distance;
            return isFar;
        }

        public bool IsFarFrom2D(Transform from, Transform to, float distance)
        {
            bool isFar = ((Vector2) @from.transform.position - (Vector2) to.transform.position).sqrMagnitude > distance;
            return isFar;
        }

        public bool IsFarFrom2D(Vector2 from, Vector2 to, float distance)
        {
            bool isFar = (from - to).sqrMagnitude > distance;
            return isFar;
        }

        #endregion
    }

    public static class CoroutineState
    {
        public static readonly YieldInstruction WaitFixedUpdate = new WaitForFixedUpdate();
        public static readonly YieldInstruction WaitForEndOfFrame = new WaitForEndOfFrame();
    }
}