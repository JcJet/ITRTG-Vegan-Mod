using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace ITRTGTextureOverrideMod
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "itrtg.vegan-texture-overrides";
        public const string PluginName = "ITRTG Vegan Texture Overrides";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;

            LogOptions options = new LogOptions();
            options.LogMisses = Config.Bind("Debug", "LogMisses", true,
                "Log Resources.Load calls that were not overridden but match LogKeywords.").Value;
            options.LogAllLoads = Config.Bind("Debug", "LogAllLoads", false,
                "Very noisy: log every Resources.Load(path, type) call.").Value;
            options.LogKeywordsCsv = Config.Bind("Debug", "LogKeywords", "food,bacon,fsm,pet,rebirth",
                "Comma-separated path fragments used when LogMisses is enabled.").Value;

            OverrideRegistry.Initialize(Paths.PluginPath, Logger, options);

            Harmony harmony = new Harmony(PluginGuid);
            harmony.PatchAll();

            Logger.LogInfo(PluginName + " loaded. Overrides directory: " + OverrideRegistry.ModDirectory);
        }
    }

    internal sealed class LogOptions
    {
        public bool LogMisses;
        public bool LogAllLoads;
        public string LogKeywordsCsv;
    }

    internal sealed class OverrideEntry
    {
        public string ResourceKey;
        public string TexturePath;
        public float PivotX;
        public float PivotY;
        public float PixelsPerUnit;
    }

    internal static class OverrideRegistry
    {
        private static readonly Dictionary<string, OverrideEntry> Exact = new Dictionary<string, OverrideEntry>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, OverrideEntry> ByName = new Dictionary<string, OverrideEntry>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> LoggedMisses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static ManualLogSource _log;
        private static LogOptions _options;
        private static string[] _keywords = new string[0];

        public static string ModDirectory;
        public static string TextureDirectory;

        public static void Initialize(string pluginPath, ManualLogSource log, LogOptions options)
        {
            _log = log;
            _options = options;
            ModDirectory = Path.Combine(pluginPath, "ITRTGTextureOverrides");
            TextureDirectory = Path.Combine(ModDirectory, "Textures");

            if (!Directory.Exists(TextureDirectory))
            {
                Directory.CreateDirectory(TextureDirectory);
            }

            ParseKeywords(options.LogKeywordsCsv);
            LoadOverrideTable(Path.Combine(ModDirectory, "texture_overrides.tsv"));
            AutoRegisterLoosePngs();

            _log.LogInfo("Texture override entries loaded: exact=" + Exact.Count + ", by-name=" + ByName.Count);
        }

        private static void ParseKeywords(string csv)
        {
            if (String.IsNullOrEmpty(csv))
            {
                _keywords = new string[0];
                return;
            }

            string[] parts = csv.Split(',');
            List<string> items = new List<string>();
            for (int i = 0; i < parts.Length; i++)
            {
                string s = parts[i].Trim();
                if (s.Length > 0)
                {
                    items.Add(s.ToLowerInvariant());
                }
            }
            _keywords = items.ToArray();
        }

        private static void LoadOverrideTable(string tablePath)
        {
            if (!File.Exists(tablePath))
            {
                _log.LogWarning("No texture_overrides.tsv found at " + tablePath + ". Falling back to filename matching only.");
                return;
            }

            string[] lines = File.ReadAllLines(tablePath);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length == 0 || line.StartsWith("#"))
                {
                    continue;
                }

                string[] parts = line.Split('\t');
                if (parts.Length < 2)
                {
                    _log.LogWarning("Bad override row " + (i + 1) + ": " + line);
                    continue;
                }

                OverrideEntry entry = new OverrideEntry();
                entry.ResourceKey = NormalizeResourceKey(parts[0]);
                entry.TexturePath = Path.Combine(TextureDirectory, parts[1].Replace('/', Path.DirectorySeparatorChar));
                entry.PivotX = ParseFloat(parts, 2, 0.5f);
                entry.PivotY = ParseFloat(parts, 3, 0.5f);
                entry.PixelsPerUnit = ParseFloat(parts, 4, 100f);

                Register(entry);
            }
        }

        private static float ParseFloat(string[] parts, int index, float fallback)
        {
            if (index >= parts.Length)
            {
                return fallback;
            }

            float result;
            if (Single.TryParse(parts[index], NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return fallback;
        }

        private static void Register(OverrideEntry entry)
        {
            if (String.IsNullOrEmpty(entry.ResourceKey))
            {
                return;
            }

            Exact[entry.ResourceKey] = entry;
            string name = LastPathSegment(entry.ResourceKey);
            if (!String.IsNullOrEmpty(name))
            {
                ByName[name] = entry;
            }
        }

        private static void AutoRegisterLoosePngs()
        {
            string[] files = Directory.GetFiles(TextureDirectory, "*.png", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                string name = Path.GetFileNameWithoutExtension(files[i]);
                if (!ByName.ContainsKey(name))
                {
                    OverrideEntry entry = new OverrideEntry();
                    entry.ResourceKey = name;
                    entry.TexturePath = files[i];
                    entry.PivotX = 0.5f;
                    entry.PivotY = 0.5f;
                    entry.PixelsPerUnit = 100f;
                    ByName[name] = entry;
                }
            }
        }

        public static bool TryGetOverride(string path, Type requestedType, out UnityEngine.Object result)
        {
            result = null;

            if (String.IsNullOrEmpty(path))
            {
                return false;
            }

            string key = NormalizeResourceKey(path);
            string typeName = requestedType == null ? "<null>" : requestedType.FullName;

            if (_options != null && _options.LogAllLoads)
            {
                _log.LogInfo("Resources.Load path='" + path + "', type='" + typeName + "'");
            }

            OverrideEntry entry;
            if (!Exact.TryGetValue(key, out entry))
            {
                string name = LastPathSegment(key);
                ByName.TryGetValue(name, out entry);
            }

            if (entry == null)
            {
                LogMissIfInteresting(path, typeName);
                return false;
            }

            if (requestedType == typeof(Sprite))
            {
                result = GetSprite(entry);
                return result != null;
            }

            if (requestedType == typeof(Texture2D) || requestedType == typeof(UnityEngine.Object) || requestedType == null)
            {
                result = GetTexture(entry);
                return result != null;
            }

            // Unknown requested type: keep the original asset to avoid invalid casts.
            return false;
        }

        private static Texture2D GetTexture(OverrideEntry entry)
        {
            Texture2D cached;
            if (TextureCache.TryGetValue(entry.TexturePath, out cached))
            {
                return cached;
            }

            if (!File.Exists(entry.TexturePath))
            {
                _log.LogWarning("Override image does not exist: " + entry.TexturePath);
                return null;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(entry.TexturePath);
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                if (!texture.LoadImage(bytes))
                {
                    _log.LogWarning("Unity failed to load PNG: " + entry.TexturePath);
                    return null;
                }

                texture.name = Path.GetFileNameWithoutExtension(entry.TexturePath);
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Bilinear;
                TextureCache[entry.TexturePath] = texture;
                return texture;
            }
            catch (Exception ex)
            {
                _log.LogError("Failed to load override image " + entry.TexturePath + ": " + ex);
                return null;
            }
        }

        private static Sprite GetSprite(OverrideEntry entry)
        {
            Sprite cached;
            if (SpriteCache.TryGetValue(entry.TexturePath, out cached))
            {
                return cached;
            }

            Texture2D texture = GetTexture(entry);
            if (texture == null)
            {
                return null;
            }

            try
            {
                Rect rect = new Rect(0f, 0f, texture.width, texture.height);
                Vector2 pivot = new Vector2(entry.PivotX, entry.PivotY);
                Sprite sprite = Sprite.Create(texture, rect, pivot, entry.PixelsPerUnit);
                sprite.name = texture.name;
                SpriteCache[entry.TexturePath] = sprite;
                return sprite;
            }
            catch (Exception ex)
            {
                _log.LogError("Failed to create sprite for " + entry.TexturePath + ": " + ex);
                return null;
            }
        }

        private static void LogMissIfInteresting(string path, string typeName)
        {
            if (_options == null || !_options.LogMisses)
            {
                return;
            }

            string lower = path.ToLowerInvariant();
            bool interesting = false;
            for (int i = 0; i < _keywords.Length; i++)
            {
                if (lower.IndexOf(_keywords[i]) >= 0)
                {
                    interesting = true;
                    break;
                }
            }

            if (!interesting)
            {
                return;
            }

            string signature = path + "|" + typeName;
            if (LoggedMisses.Contains(signature))
            {
                return;
            }

            LoggedMisses.Add(signature);
            _log.LogInfo("No override for Resources.Load path='" + path + "', type='" + typeName + "'");
        }

        private static string NormalizeResourceKey(string path)
        {
            if (path == null)
            {
                return String.Empty;
            }

            string key = path.Trim().Replace('\\', '/');
            if (key.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                key = key.Substring(0, key.Length - 4);
            }
            return key.Trim('/');
        }

        private static string LastPathSegment(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                return String.Empty;
            }

            int idx = key.LastIndexOf('/');
            if (idx < 0 || idx >= key.Length - 1)
            {
                return key;
            }
            return key.Substring(idx + 1);
        }
    }

    [HarmonyPatch(typeof(Resources), "Load", new Type[] { typeof(string), typeof(Type) })]
    internal static class ResourcesLoadPathTypePatch
    {
        private static bool Prefix(string path, Type systemTypeInstance, ref UnityEngine.Object __result)
        {
            UnityEngine.Object replacement;
            if (OverrideRegistry.TryGetOverride(path, systemTypeInstance, out replacement))
            {
                __result = replacement;
                return false;
            }
            return true;
        }
    }
}
