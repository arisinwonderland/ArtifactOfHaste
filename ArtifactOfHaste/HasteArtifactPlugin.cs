using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using R2API;
using TILER2;
using BepInEx;
using UnityEngine;

namespace Grey
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Grey.ArtifactOfHaste", "Artifact of Haste", "0.1.0")]
    public class Haste : Artifact_V2<Haste>
    {
        public override string displayName => "Artifact of Haste";
        protected override string GetNameString(string langid = null) => displayName;
        protected override string GetDescString(string langid = null) => $"All units have no skill cooldowns.";

        public Haste() {

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
