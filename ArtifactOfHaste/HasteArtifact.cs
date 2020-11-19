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
using BepInEx;
using UnityEngine;

namespace Grey.HasteArtifact
{
    [BepInPlugin("com.Grey.ArtifactOfHaste", "Artifact of Haste", "0.1.0")]
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.ThinkInvisible.TILER2")] 
    [R2APISubmoduleDependency(nameof(ResourcesAPI))]
    public class HasteArtifactPlugin : BaseUnityPlugin
    {
        //internal static BepInEx.Logging.ManualLogSource _logger;
        public void Awake()
        {
            //_logger = Logger;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArtifactOfHaste.artifactofhaste"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@ArtifactOfHaste", bundle);
                ResourcesAPI.AddProvider(provider);
            }
            Chat.AddMessage("Loaded Haste mod!");
            //Logger.LogDebug("Loaded Haste!");
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
            if (IsActiveAndEnabled() && body) {
                body.gameObject.AddComponent<HasteController>().Init(body);
                Debug.Log("A Haste controller was added to a unit!");
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
