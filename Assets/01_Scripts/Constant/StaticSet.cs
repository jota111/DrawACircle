using System;
using UnityEngine;

namespace SH.Constant
{
    public static class StaticSet
    {
        public static readonly string Version = Application.version;
        public static readonly int VersionNumber = Convert.ToInt32(Version.Replace(".", string.Empty));
    }
}
