/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.IO;
using System.Text;
using BestHTTP.Decompression.Zlib;

namespace SH.Util
{
    public static class CompressUtil
    {
        public static byte[] CompressGZip(byte[] data)
        {
            using var compressedStream = new MemoryStream();
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Compress);
            zipStream.Write(data, 0, data.Length);
            zipStream.Close();
            var result = compressedStream.ToArray();
            return result;
        }

        public static byte[] CompressGZipFromString(string str)
        {
            var data = Encoding.UTF8.GetBytes(str);
            return CompressGZip(data);
        }
        
        public static byte[] DecompressGZip(byte[] data)
        {
            using var compressedStream = new MemoryStream(data);
            using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();
            zipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }

        public static string DecompressGZipToJson(byte[] data)
        {
            var decompress = DecompressGZip(data);
            var json = Encoding.UTF8.GetString(decompress);
            return json;
        }
    }
}