using System;
using UnityEngine;

namespace FalseExit.Screens
{
    /// <summary>
    /// Connect your existing fuse, cable and calibration puzzles to the displays.
    /// This component displays state; it does not decide when a puzzle is solved.
    /// Uses per-material property blocks, so shared material assets are never edited.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("False Exit/Screens/AI Screen Controller")]
    public sealed class AIScreenController : MonoBehaviour
    {
        public enum LinkNode { PWR, NET, CPU, CORE }

        [Serializable]
        public sealed class ScreenTarget
        {
            public Renderer renderer;
            [Min(0)] public int materialIndex;
            [NonSerialized] private MaterialPropertyBlock block;

            internal bool Read(int requiredProperty, out MaterialPropertyBlock properties)
            {
                properties = null;
                if (renderer == null || materialIndex < 0) return false;
                Material[] materials = renderer.sharedMaterials;
                if (materialIndex >= materials.Length) return false;
                Material material = materials[materialIndex];
                if (material == null || !material.HasProperty(requiredProperty)) return false;
                if (block == null) block = new MaterialPropertyBlock();
                // Preserve other per-material overrides on this slot.
                renderer.GetPropertyBlock(block, materialIndex);
                properties = block;
                return true;
            }

            internal void Write(MaterialPropertyBlock properties)
            {
                renderer.SetPropertyBlock(properties, materialIndex);
            }
        }

        [Header("Assign the three display surfaces")]
        public ScreenTarget fuseScreen = new ScreenTarget();
        public ScreenTarget linkScreen = new ScreenTarget();
        public ScreenTarget stabilityScreen = new ScreenTarget();

        [Header("Display power")]
        [Range(0f, 1f)] public float screenPower = 1f;

        [Header("Fuse puzzle")]
        public bool fuse1Present = true;
        public bool fuse2Present;
        public bool fuse3Present;
        [Range(0f, 1f)] public float fuse1Charge = 1f;
        [Range(0f, 1f)] public float fuse2Charge = 1f;
        [Range(0f, 1f)] public float fuse3Charge = 1f;
        [Range(0.01f, 1f)] public float onlineThreshold = 0.95f;

        [Header("Cable links")]
        public bool pwrConnected = true;
        public bool netConnected = true;
        public bool cpuConnected = true;
        public bool coreConnected;

        [Header("Calibration and stability")]
        [Range(0f, 1f)] public float stability = 0.42f;
        [Range(0f, 1f)] public float signal = 0.42f;
        [Range(0f, 1f)] public float systemPower = 0.8f;
        [Range(0f, 1f)] public float sync = 0.17f;
        [Range(0.01f, 1f)] public float stableThreshold = 0.95f;

        private bool dirty = true;
        private static readonly int PowerId = Shader.PropertyToID("_Power");
        private static readonly int F1Id = Shader.PropertyToID("_Fuse1Present");
        private static readonly int F2Id = Shader.PropertyToID("_Fuse2Present");
        private static readonly int F3Id = Shader.PropertyToID("_Fuse3Present");
        private static readonly int C1Id = Shader.PropertyToID("_Fuse1Charge");
        private static readonly int C2Id = Shader.PropertyToID("_Fuse2Charge");
        private static readonly int C3Id = Shader.PropertyToID("_Fuse3Charge");
        private static readonly int OnlineId = Shader.PropertyToID("_OnlineThreshold");
        private static readonly int PwrId = Shader.PropertyToID("_LinkPWR");
        private static readonly int NetId = Shader.PropertyToID("_LinkNET");
        private static readonly int CpuId = Shader.PropertyToID("_LinkCPU");
        private static readonly int CoreId = Shader.PropertyToID("_LinkCORE");
        private static readonly int StabilityId = Shader.PropertyToID("_Stability");
        private static readonly int SignalId = Shader.PropertyToID("_Signal");
        private static readonly int SystemPowerId = Shader.PropertyToID("_SystemPower");
        private static readonly int SyncId = Shader.PropertyToID("_Sync");
        private static readonly int StableId = Shader.PropertyToID("_StableThreshold");

        public int OnlineFuseCount
        {
            get
            {
                float threshold = Mathf.Clamp(onlineThreshold, 0.01f, 1f);
                return (fuse1Present && fuse1Charge >= threshold ? 1 : 0)
                     + (fuse2Present && fuse2Charge >= threshold ? 1 : 0)
                     + (fuse3Present && fuse3Charge >= threshold ? 1 : 0);
            }
        }

        public int ConnectedLinkCount
        {
            get
            {
                return (pwrConnected ? 1 : 0) + (netConnected ? 1 : 0)
                     + (cpuConnected ? 1 : 0) + (coreConnected ? 1 : 0);
            }
        }

        private void OnEnable() { dirty = true; }

        private void OnValidate()
        {
            ClampState();
            // OnValidate may run while Unity is loading. Defer renderer API calls.
            dirty = true;
        }

        private void Update()
        {
            if (dirty) ApplyState();
        }

        private void ClampState()
        {
            screenPower = Mathf.Clamp01(screenPower);
            fuse1Charge = Mathf.Clamp01(fuse1Charge);
            fuse2Charge = Mathf.Clamp01(fuse2Charge);
            fuse3Charge = Mathf.Clamp01(fuse3Charge);
            onlineThreshold = Mathf.Clamp(onlineThreshold, 0.01f, 1f);
            stability = Mathf.Clamp01(stability);
            signal = Mathf.Clamp01(signal);
            systemPower = Mathf.Clamp01(systemPower);
            sync = Mathf.Clamp01(sync);
            stableThreshold = Mathf.Clamp(stableThreshold, 0.01f, 1f);
        }

        /// <summary>Also available through the component's context menu for edit-mode preview.</summary>
        [ContextMenu("Apply State To Screens")]
        public void ApplyState()
        {
            ClampState();
            MaterialPropertyBlock block;
            if (fuseScreen != null && fuseScreen.Read(F1Id, out block))
            {
                block.SetFloat(PowerId, screenPower);
                block.SetFloat(F1Id, fuse1Present ? 1f : 0f);
                block.SetFloat(F2Id, fuse2Present ? 1f : 0f);
                block.SetFloat(F3Id, fuse3Present ? 1f : 0f);
                block.SetFloat(C1Id, fuse1Charge);
                block.SetFloat(C2Id, fuse2Charge);
                block.SetFloat(C3Id, fuse3Charge);
                block.SetFloat(OnlineId, onlineThreshold);
                fuseScreen.Write(block);
            }
            if (linkScreen != null && linkScreen.Read(PwrId, out block))
            {
                block.SetFloat(PowerId, screenPower);
                block.SetFloat(PwrId, pwrConnected ? 1f : 0f);
                block.SetFloat(NetId, netConnected ? 1f : 0f);
                block.SetFloat(CpuId, cpuConnected ? 1f : 0f);
                block.SetFloat(CoreId, coreConnected ? 1f : 0f);
                linkScreen.Write(block);
            }
            if (stabilityScreen != null && stabilityScreen.Read(StabilityId, out block))
            {
                block.SetFloat(PowerId, screenPower);
                block.SetFloat(StabilityId, stability);
                block.SetFloat(SignalId, signal);
                block.SetFloat(SystemPowerId, systemPower);
                block.SetFloat(SyncId, sync);
                block.SetFloat(StableId, stableThreshold);
                stabilityScreen.Write(block);
            }
            dirty = false;
        }

        /// <summary>Fuse slots are numbered 1, 2, 3. Charge uses 0..1.</summary>
        public void SetFuse(int slot, bool installed, float charge = 1f)
        {
            switch (slot)
            {
                case 1: fuse1Present = installed; fuse1Charge = charge; break;
                case 2: fuse2Present = installed; fuse2Charge = charge; break;
                case 3: fuse3Present = installed; fuse3Charge = charge; break;
                default: throw new ArgumentOutOfRangeException(nameof(slot), "Fuse slot must be 1, 2 or 3.");
            }
            ApplyState();
        }

        public void SetLink(LinkNode node, bool connected)
        {
            switch (node)
            {
                case LinkNode.PWR: pwrConnected = connected; break;
                case LinkNode.NET: netConnected = connected; break;
                case LinkNode.CPU: cpuConnected = connected; break;
                case LinkNode.CORE: coreConnected = connected; break;
                default: throw new ArgumentOutOfRangeException(nameof(node));
            }
            ApplyState();
        }

        /// <summary>Normalized value: 0.42 displays 42%, 1 displays 100%.</summary>
        public void SetStability(float value) { stability = value; ApplyState(); }

        public void SetDiagnostics(float signalLevel, float powerLevel, float syncLevel)
        {
            signal = signalLevel; systemPower = powerLevel; sync = syncLevel;
            ApplyState();
        }

        public void SetDisplayPower(bool on) { screenPower = on ? 1f : 0f; ApplyState(); }

        [ContextMenu("Preview/Repair State")]
        public void PreviewRepair()
        {
            screenPower = 1f;
            fuse1Present = true; fuse2Present = false; fuse3Present = false;
            fuse1Charge = fuse2Charge = fuse3Charge = 1f;
            pwrConnected = netConnected = cpuConnected = true; coreConnected = false;
            stability = signal = 0.42f; systemPower = 0.8f; sync = 0.17f;
            ApplyState();
        }

        [ContextMenu("Preview/All Online")]
        public void PreviewOnline()
        {
            screenPower = 1f;
            fuse1Present = fuse2Present = fuse3Present = true;
            fuse1Charge = fuse2Charge = fuse3Charge = 1f;
            pwrConnected = netConnected = cpuConnected = coreConnected = true;
            stability = signal = systemPower = sync = 1f;
            ApplyState();
        }

        [ContextMenu("Preview/Fault State")]
        public void PreviewFault()
        {
            screenPower = 1f;
            fuse1Present = true; fuse2Present = fuse3Present = false;
            fuse1Charge = 0.35f; fuse2Charge = fuse3Charge = 1f;
            pwrConnected = netConnected = cpuConnected = coreConnected = false;
            stability = 0.07f; signal = 0.08f; systemPower = 0.16f; sync = 0.04f;
            ApplyState();
        }
    }
}
