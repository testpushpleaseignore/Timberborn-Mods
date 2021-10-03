using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using System;

using Timberborn.CameraSystem;
using Timberborn.InputSystem;

using UnityEngine;

namespace CameraRotateDelayFix {

	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	[BepInProcess("Timberborn.exe")]
	public class CameraRotateDelayFix : BaseUnityPlugin {
		public const string pluginGuid = "testpostpleaseignore.timberborn.camera_rotate_delay_fix";
		public const string pluginName = "CameraRotateDelayFix";
		public const string pluginVersion = "0.0.1";

		private Harmony harmony;

		public static CameraRotateDelayFix instance;

		internal static ManualLogSource logger;
		new internal static BepInEx.Configuration.ConfigFile Config;

		private void Awake() {
			logger = base.Logger;
			Config = base.Config;

			instance = this;

			harmony = new Harmony(pluginGuid);

			try { harmony.PatchAll(typeof(CameraRotateDelayFix)); }
			catch (Exception e) { Logger.LogError($"Harmony patching failed: {e.Message}"); }

			logger.LogInfo("Load Complete");
		}

		[HarmonyPrefix, HarmonyPatch(typeof(MouseCameraController), "RotationUpdate")]
		public static bool MouseCameraController_RotationUpdate_Patch(MouseCameraController __instance) {
			if (__instance._inputService._mouse.IsButtonHeld(__instance._inputService.MouseRotateCameraButton) && !__instance._inputService.MouseMoveCamera) {
				if (!__instance._rotating) {
					__instance.StartRotatingCamera();
				}
				__instance.RotateCamera();
				return false;
			}
			if (__instance._rotating) {
				__instance.StopRotatingCamera();
			}

			return false;
		}

		private void OnDestroy() {
			harmony.UnpatchSelf();
			instance = null;
		}
	}
}
