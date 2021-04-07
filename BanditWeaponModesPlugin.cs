using System;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using R2API.Utils;
using RoR2.UI;
using RoR2;
using EntityStates.Bandit2.Weapon;

namespace BanditWeaponModes
{
    enum FireMode { Normal, Spam, DoubleDoubleTap, Count }

    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("de.userstorm.banditweaponmodes", "BanditWeaponModes", "{VERSION}")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class BanditWeaponModesPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<string> DefaultMode { get; set; }
        public static ConfigEntry<bool> EnableModeSelectionWithNumberKeys { get; set; }
        public static ConfigEntry<bool> EnableModeSelectionWithMouseWheel { get; set; }
        public static ConfigEntry<bool> EnableModeSelectionWithDPad { get; set; }
        public static ConfigEntry<int> DelayBetweenDoubleDoubleTaps { get; set; }
        public static ConfigEntry<int> DelayBetweenShots { get; set; }

        private FireMode fireMode = FireMode.Normal;
        private float fixedAge = 0;
        private bool fire = false;
        private bool reloadLock = false;
        private bool hasLeftLandingPod = false;
        private HGTextMeshProUGUI modeLabel = null;

        private void CycleFireMode (bool forward = true)
        {
            FireMode newFireMode = fireMode + (forward ? 1 : -1);

            if (newFireMode == FireMode.Count)
            {
                newFireMode = FireMode.Normal;
            }

            if ((int)newFireMode == -1)
            {
                newFireMode = FireMode.Count - 1;
            }

            fireMode = newFireMode;
        }

        private void InitConfig()
        {
            DefaultMode = Config.Bind<string>(
                "Settings",
                "DefaultMode",
                "Normal",
                "The mode that is selected on game start. Modes: Normal, Spam, DoubleDoubleTap"
            );

            EnableModeSelectionWithNumberKeys = Config.Bind<bool>(
               "Settings",
               "EnableModeSelectionWithNumberKeys",
               true,
               "When set to true modes can be selected using the number keys"
            );

            EnableModeSelectionWithMouseWheel = Config.Bind<bool>(
               "Settings",
               "EnableModeSelectionWithMouseWheel",
               true,
               "When set to true modes can be cycled through using the mouse wheel"
            );

            EnableModeSelectionWithDPad = Config.Bind<bool>(
               "Settings",
               "EnableModeSelectionWithDPad",
               true,
               "When set to true modes can be cycled through using the DPad (controller)"
            );

            DelayBetweenDoubleDoubleTaps = Config.Bind<int>(
               "Settings",
               "DelayBetweenDoubleDoubleTaps",
               400,
               "The delay (in milliseconds) between the 2nd and 3rd shot when using the DoubleDoubleTap Mode"
            );

            DelayBetweenShots = Config.Bind<int>(
               "Settings",
               "DelayBetweenShots",
               125,
               "The delay (in milliseconds) between the shots"
            );
        }

        private void HandleConfig()
        {
            try
            {
                fireMode = (FireMode)Enum.Parse(typeof(FireMode), DefaultMode.Value);
            }
            catch (Exception)
            {
                fireMode = FireMode.Normal;
            }
        }

        private HGTextMeshProUGUI CreateLabel (Transform parent, String name, String text, Vector2 position)
        {
            GameObject textContainer = new GameObject(name);

            textContainer.transform.parent = parent;

            textContainer.AddComponent<CanvasRenderer>();

            RectTransform rectTransform = textContainer.AddComponent<RectTransform>();
            HGTextMeshProUGUI hgtextMeshProUGUI = textContainer.AddComponent<HGTextMeshProUGUI>();

            hgtextMeshProUGUI.text = text;
            hgtextMeshProUGUI.fontSize = 12f;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TMPro.TextAlignmentOptions.Center;
            hgtextMeshProUGUI.enableWordWrapping = false;

            rectTransform.localPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = position;

            return hgtextMeshProUGUI;
        }

        private void SelectFireModeWithNumberKeys()
        {
            if (!EnableModeSelectionWithNumberKeys.Value)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                fireMode = FireMode.Normal;

                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                fireMode = FireMode.Spam;

                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                fireMode = FireMode.DoubleDoubleTap;
            }
        }

        private void SelectFireModeWithMouseWheel()
        {
            if (!EnableModeSelectionWithMouseWheel.Value)
            {
                return;
            }

            float wheel = Input.GetAxis("Mouse ScrollWheel");

            if (wheel == 0)
            {
                return;
            }

            // scroll down => forward; scroll up => backward
            CycleFireMode(wheel < 0f);
        }

        private void SelectFireModeWithDPad()
        {
            if (!EnableModeSelectionWithDPad.Value)
            {
                return;
            }

            if (DPad.GetInputDown(DPadInput.Right) || DPad.GetInputDown(DPadInput.Down))
            {
                CycleFireMode();

                return;
            }

            if (DPad.GetInputDown(DPadInput.Left) || DPad.GetInputDown(DPadInput.Up))
            {
                CycleFireMode(false);
            }
         }

        private void SelectFireMode()
        {
            SelectFireModeWithNumberKeys();
            SelectFireModeWithMouseWheel();
            SelectFireModeWithDPad();
        }

        private float getDelay(int charges)
        {
            if (fireMode == FireMode.DoubleDoubleTap && charges % 2 == 0)
            {
                return (float) DelayBetweenDoubleDoubleTaps.Value / 1000f;
            }

            return (float) DelayBetweenShots.Value / 1000f;
        }

        private void FireLogic(EntityStateMachine self)
        {
            var skill = self.commonComponents.skillLocator.primary;
            var inputBank = self.commonComponents.inputBank;
            bool fire = inputBank && inputBank.skill1.down;
            bool justPressed = inputBank && inputBank.skill1.justPressed;
            bool isReloading = self.state.ToString() == "EntityStates.Bandit2.Weapon.Reload";
            int charges = skill.stock;
            float delay = getDelay(charges);

            if (charges == 0 || isReloading)
            {
                reloadLock = true;
            }

            if ((charges == skill.maxStock && !isReloading) || justPressed)
            {
                reloadLock = false;
            }

            if (!reloadLock && charges > 0 && fire && fixedAge > delay)
            {
                fixedAge = 0;

                // ignore first shot since it's handled by the game
                if (justPressed)
                {
                    return;
                }

                if (skill.skillNameToken == "BANDIT2_PRIMARY_NAME")
                {
                    self.SetNextState(new FireShotgun2());
                }
                else
                {
                    self.SetNextState(new Bandit2FireRifle());
                }

                skill.DeductStock(1);
            }
        }

        public void FixedUpdateHook(On.RoR2.EntityStateMachine.orig_FixedUpdate orig, EntityStateMachine self)
        {
            orig.Invoke(self);

            var body = self.commonComponents.characterBody;
            bool isBandit = body && body.baseNameToken == "BANDIT2_BODY_NAME";

            if (!isBandit)
            {
                return;
            }

            // if no longer EntityStates.SpawnTeleporterState or EntityStates.GenericCharacterPod
            if (
                !hasLeftLandingPod &&
                self.customName == "Body" &&
                self.state.ToString() == "EntityStates.GenericCharacterMain"
            )
            {
                hasLeftLandingPod = true;

                return;
            }

            if (hasLeftLandingPod && self.customName == "Weapon" && fireMode != FireMode.Normal)
            {
                fixedAge += Time.fixedDeltaTime;

                FireLogic(self);
            }
        }

        public void UpdateHook(On.RoR2.UI.SkillIcon.orig_Update orig, SkillIcon self)
        {
            orig.Invoke(self);

            if (self.targetSkill && self.targetSkillSlot == SkillSlot.Primary)
            {
                if (self.targetSkill.characterBody.baseNameToken == "BANDIT2_BODY_NAME")
                {
                    if (!modeLabel) {
                        var pos = self.stockText.transform.position;

                        modeLabel = CreateLabel(
                            self.stockText.transform.parent,
                            "ModeLabel",
                            fireMode.ToString(),
                            new Vector2(pos.x - 7.5f, pos.y + 20f)
                        );

                        modeLabel.transform.rotation = self.stockText.transform.rotation;
                        modeLabel.color = self.stockText.color;
                        modeLabel.alignment = self.stockText.alignment;
                    }

                    modeLabel.SetText(fireMode.ToString());
                }
            }
        }

        public void Awake()
        {
            InitConfig();
            HandleConfig();

            On.RoR2.EntityStateMachine.FixedUpdate += FixedUpdateHook;
            On.RoR2.UI.SkillIcon.Update += UpdateHook;
        }

        public void Update()
        {
            DPad.Update();

            SelectFireMode();
        }

        public void OnDestroy()
        {
            On.RoR2.EntityStateMachine.FixedUpdate -= FixedUpdateHook;
            On.RoR2.UI.SkillIcon.Update -= UpdateHook;
        }
    }
}
