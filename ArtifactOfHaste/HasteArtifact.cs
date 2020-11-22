using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using R2API;
using R2API.Utils;
using TILER2;
using static TILER2.MiscUtil;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Path = System.IO.Path;

namespace Grey.HasteArtifact
{
    [BepInPlugin(modName, modGuid, "0.1.0")]
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.ThinkInvisible.TILER2")] 
    [R2APISubmoduleDependency(nameof(ResourcesAPI))]
    public class HasteArtifactPlugin : BaseUnityPlugin
    {
        public const string modName = "ArtifactOfHaste";
        public const string modGuid = "com.Grey.ArtifactOfHaste";

        internal static FilingDictionary<CatalogBoilerplate> itemList = new FilingDictionary<CatalogBoilerplate>();
        private static ConfigFile config;
        private void Awake()
        {
            Debug.Log("Loading assets.");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArtifactOfHaste.artifactofhaste"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@ArtifactOfHaste", bundle);
                ResourcesAPI.AddProvider(provider);
            }

            config = new ConfigFile(Path.Combine(Paths.ConfigPath, modGuid + ".cfg"), true);

            Debug.Log("Creating mod info.");
            itemList = T2Module.InitAll<CatalogBoilerplate>(new T2Module.ModInfo
            {
                displayName = "Artifact of Haste",
                longIdentifier = "ArtifactOfHaste",
                shortIdentifier = "AOH",
                mainConfigFile = config
            });

            Debug.Log("Initializing mod.");
            T2Module.SetupAll_PluginAwake(itemList);
            T2Module.SetupAll_PluginStart(itemList);
        }
    }

    public class Haste : Artifact_V2<Haste>
    {
        public override string displayName => "Artifact of Haste";
        protected override string GetNameString(string langid = null) => displayName;
        protected override string GetDescString(string langid = null) => $"All units have no skill cooldowns.";

        public Haste() {
            iconResourcePath = "@ArtifactOfHaste:Assets/haste_enabled.png";
            iconResourcePathDisabled = "@ArtifactOfHaste:Assets/haste_disabled.png";
        }

        public void Awake()
        {
            Chat.AddMessage("Loaded mod!");
            Debug.Log("Awakening mod.");
        }

        public override void Install() {
            base.Install();
            On.RoR2.CharacterMaster.SpawnBody += CharacterMaster_SpawnBody;
        }

        public override void Uninstall() {
            base.Uninstall();
            On.RoR2.CharacterMaster.SpawnBody -= CharacterMaster_SpawnBody;
        }

        private CharacterBody CharacterMaster_SpawnBody(On.RoR2.CharacterMaster.orig_SpawnBody orig, CharacterMaster self, GameObject bodyPrefab, Vector3 position, Quaternion rotation) {
            CharacterBody body = orig(self, bodyPrefab, position, rotation);
            if (body) { 
                Chat.AddMessage("Spawned body!");
                if (IsActiveAndEnabled()) {
                    body.gameObject.AddComponent<HasteController>().Init(body);
                    Chat.AddMessage("A Haste controller was added to a unit!");
                }
            }
            return body;
        }
    }
 
    public class HasteController : MonoBehaviour {
        private SkillLocator skills;
        public void Init(CharacterBody body) {
            this.skills = body.GetComponent<SkillLocator>();
        }
        public void Update() {
            skills.ApplyAmmoPack();
        }
    }
}
