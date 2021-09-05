/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SH.Net;
using UnityEngine;

namespace SH.AppEvent
{
    public static class StatisticsBuffer
    {
        private static readonly string Path = Application.persistentDataPath + "/Statistics.dat";
        private const long BufferSize = 1024 * 2; // 버퍼사이즈 2k
        private const string Api = "tracker/statistics_bundle";
        private static readonly Dictionary<string, object> _temp = new Dictionary<string, object>();

        public static void Add(string type, object value)
        {
            _temp.Clear();
            _temp.Add(type, value);
            var json = JsonConvert.SerializeObject(_temp, Formatting.None);
            long length = 0;
            try
            {
                using var fs = new FileStream(Path, FileMode.Append, FileAccess.Write);
                length = fs.Length;
                using var writer = new StreamWriter(fs, Encoding.UTF8);
                writer.WriteLine(json);
            }
            catch (Exception e)
            {
                Delete();
                Debug.LogError(e);
                return;
            }

            if (BufferSize <= length)
            {
                SendToServer();
                Delete();
            }
        }

        private static void Delete()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
        }

        private static void SendToServer()
        {
            string json = string.Empty;
            using (var stream = new StreamReader(Path, Encoding.UTF8))
            {
                var jObject = new JObject { { "device_id", SystemInfo.deviceUniqueIdentifier } };
                var array = new JArray();
                jObject.Add("data", array);
                
                while (stream.Peek() >= 0)
                {
                    var line = stream.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        try
                        {
                            var obj = JObject.Parse(line);
                            array.Add(obj);
                        }
                        catch (Exception e)
                        {
                            // ignored
                        }
                    }
                }
                if (array.Count > 0)
                {
                    json = JsonConvert.SerializeObject(jObject);
                }
            }

            if (!string.IsNullOrEmpty(json))
            {
                NetManager.PostForget(Api, json);
            }
        }
    }
}