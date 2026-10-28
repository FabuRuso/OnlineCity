using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OCUnion
{
    public static class MainHelper
    {
        /// <summary>
        /// Активирует разрабатваемые блоки и дополнительное логирования
        /// </summary>
        public static bool DebugMode = false;
        /// <summary>
        /// Отключить все логи в файл и консоль. Сервер всегда меняет значение на false
        /// </summary>
        public static bool OffAllLog = false;

        public static bool InGame = false;

        private static Assembly AssemblyAssembly = Assembly.GetAssembly(typeof(MainHelper));
        private static Version AssemblyVersion = AssemblyAssembly.GetName().Version;
        private static DateTime? AssemblyDate = string.IsNullOrEmpty(AssemblyAssembly.Location) ? (DateTime?)null 
            : new FileInfo(AssemblyAssembly.Location).LastWriteTime;

        public static string VersionInfo = $"Version {(AssemblyVersion.ToString() + "$$").Replace(".0$$", "").Replace("$$", "")}a" +
            (AssemblyDate == null ? "" : " from " + AssemblyDate.Value.ToString("yyyy.MM.dd"));
        //public static string VersionInfo = "Version 0.04.70a from 2021.10.31";

        public static string Key = "";

        /// <summary>
        /// Для автоматической проверки: версия клиента должна быть больше или равна версии сервера
        /// </summary>
        public static long VersionNum = AssemblyVersion.Major * 10000 * 10000
          + AssemblyVersion.Minor * 10000
          + AssemblyVersion.Build;
        //public static long VersionNum = 40070;

        public static string DefaultIP = DebugMode ? "localhost" : " ";

        //        public static Dictionary<string, string> ServerList = new Dictionary<string, string> 
        //        {
        //            { "Vanilla", "185.221.214.172:19024" },
        //            { "Fantasy", "185.221.214.172:19022" },
        //            { "BIOnline", "185.221.214.172:19024"},
        //#if DEBUG
        //            { "localhost", "127.0.0.1" },
        //#endif
        //        };

        // Замена списка серверов на Pastebin

        public static Dictionary<string, string> ServerList = new Dictionary<string, string>();

        public static Dictionary<string, string> ServerDescriptions = new Dictionary<string, string>();

        public static void LoadServerList()
        {
            string pastebinRawUrl = "https://pastebin.com/raw/y7e9FEDx";
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    string json = client.DownloadString(pastebinRawUrl);

                    // Парсим гибко: поддержка и старого, и нового формата
                    var servers = new Dictionary<string, string>();
                    var descriptions = new Dictionary<string, string>();

                    var token = JToken.Parse(json);
                    if (token.Type == JTokenType.Object)
                    {
                        foreach (var prop in ((JObject)token).Properties())
                        {
                            // Старый формат: "Name": "ip:port"
                            if (prop.Value.Type == JTokenType.String)
                            {
                                var addr = prop.Value.Value<string>();
                                if (!string.IsNullOrWhiteSpace(prop.Name) && !string.IsNullOrWhiteSpace(addr))
                                    servers[prop.Name] = addr;
                            }
                            // Новый формат: "Name": { "address": "...", "description": "..." }
                            else if (prop.Value.Type == JTokenType.Object)
                            {
                                var obj = (JObject)prop.Value;
                                var addr = obj["address"]?.Value<string>()
                                           ?? obj["ip"]?.Value<string>()
                                           ?? obj["addr"]?.Value<string>();
                                var desc = obj["description"]?.Value<string>() ?? obj["desc"]?.Value<string>();

                                if (!string.IsNullOrWhiteSpace(prop.Name) && !string.IsNullOrWhiteSpace(addr))
                                    servers[prop.Name] = addr;

                                if (!string.IsNullOrWhiteSpace(desc))
                                    descriptions[prop.Name] = desc.Replace("\\n", "\n"); // поддержка \n в JSON
                            }
                        }
                    }

                    // Если не распознали как объект — пробуем как старый словарь
                    if (servers.Count == 0)
                    {
                        var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        if (parsed != null) servers = parsed;
                    }

                    if (servers.Count > 0) ServerList = servers;
                    if (descriptions.Count > 0) ServerDescriptions = descriptions;
                }

                Loger.Log("Сервера (и описания, если были) обновлены из Pastebin");
            }
            catch (Exception ex)
            {
                Loger.Log("Ошибка загрузки списка серверов из Pastebin: " + ex.Message);
            }
        }

        public static string GetServerDescription(string serverName)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                return "Сначала выберите сервер в списке.";

            if (ServerDescriptions != null
                && ServerDescriptions.TryGetValue(serverName, out var text)
                && !string.IsNullOrWhiteSpace(text))
                return text;

            return $"Описание для сервера \"{serverName}\" не найдено.";
        }



        public static int MinCostForTrade = 100000;

        public static string CashlessThingDefName = "Silver";

        private static ThingDef _CashlessThingDef;
        public static ThingDef CashlessThingDef
        {
            get 
            {
                if (_CashlessThingDef == null)
                {
                    _CashlessThingDef = (ThingDef)GenDefDatabase.GetDef(typeof(ThingDef), CashlessThingDefName);
                }
                return _CashlessThingDef;
            }
        }

        private static CultureInfo CultureValue = null;
        public static string CultureFromGame = null;
        public static CultureInfo Culture
        {
            get
            {
                if (CultureValue == null)
                {
                    try
                    {
                        if (CultureFromGame != null && CultureFromGame.StartsWith("Russian"))
                            CultureValue = CultureInfo.GetCultureInfo("ru-RU");
                        else
                            CultureValue = CultureInfo.InvariantCulture;
                    }
                    catch
                    {
                        CultureValue = CultureInfo.InvariantCulture;
                    }
                }

                return CultureValue;
            }
        }

        public static int RandomSeed { get; } = new Random((int)(DateTime.UtcNow.Ticks & int.MaxValue)).Next(10000, 99999);
        public static int LockCode { get; } = int.Parse(DateTime.Now.ToString("HHmmssfff"));


        private static ConcurrentDictionary<string, string> TranslateCacheDic = new ConcurrentDictionary<string, string>();
        public static string TranslateCache(this string text)
        {
            return TranslateCacheDic.GetOrAdd(text, t => t.Translate());
        }
        public static string TranslateCache(this string text, params object[] args)
        {
            return string.Format(text.TranslateCache(), args);
        }

        public static string NeedTranslate(this string text)
        {
            return text;
        }
        public static string NeedTranslate(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static string ToGoodString(this DateTime that)
        {
            return that.ToString(Culture);
        }

        public static string ToGoodUtcString(this DateTime that)
        {
            if (that == DateTime.MinValue) return that.ToString(Culture);
            var nowUtc = DateTime.Now - DateTime.UtcNow;
            return (that + nowUtc).ToString(Culture);
        }

        public static string ToGoodUtcString(this DateTime that, string format)
        {
            if (that == DateTime.MinValue) return that.ToString(format, Culture);
            var nowUtc = DateTime.Now - DateTime.UtcNow;
            return (that + nowUtc).ToString(format, Culture);
        }

        public static string NormalizeFileNameChars(this string fileName)
        {
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            foreach (var c in invalidFileChars) if (fileName.Contains(c)) fileName = fileName.Replace(c, '_');
            if (fileName.Contains('.')) fileName = fileName.Replace('.', '_');
            return fileName;
        }
        public static string NormalizeUniqueFileNameChars(this string fileName)
        {
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            if (fileName.Contains('.')) fileName = fileName.Replace('.', '_');
            foreach (var c in invalidFileChars) if (fileName.Contains(c)) fileName = fileName.Replace(c.ToString(), ((byte)c).ToString());
            return fileName;
        }

        public static string NormalizePath(this string fileName) => fileName == null ? null : fileName.Replace("\\", "" + Path.DirectorySeparatorChar);
        public static List<string> NormalizePath(this List<string> fileNames) => fileNames == null ? null : fileNames.Select(fn => fn.Replace("\\", "" + Path.DirectorySeparatorChar)).ToList();
        public static IEnumerable<string> NormalizePath(this IEnumerable<string> fileNames) => fileNames == null ? null : fileNames.Select(fn => fn.Replace("\\", "" + Path.DirectorySeparatorChar));
    }
}

//  ⎘ ⎆ ⚐ ⚑ ⚙ ⚠ ⚡ ⚪ ⚫ ✓ ✅ ✔ ✘ ✕ ✗ ✎ ✏ ⬤  🪙 🤔 🏱 🏲 🏳 🏴 🆗 

