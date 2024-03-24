﻿using KontrolSystem.KSP.Runtime.KSPResource;
using KontrolSystem.TO2.Binding;
using KSP.Modules;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public partial class KSPVesselModule {
    [KSClass("EngineMode")]
    public class EngineModeAdapter(Data_Engine.EngineMode engineMode) {
        [KSField] public string Name => engineMode.engineID;

        [KSField] public bool AllowRestart => engineMode.allowRestart;

        [KSField] public bool AllowShutdown => engineMode.allowShutdown;

        [KSField] public bool ThrottleLocked => engineMode.throttleLocked;

        [KSField] public double MinThrust => engineMode.minThrust;

        [KSField] public double MaxThrust => engineMode.maxThrust;

        [KSField] public EngineType EngineType => engineMode.engineType;

        [KSField]
        public KSPResourceModule.ResourceDefinitionAdapter Propellant {
            get {
                var context = KSPContext.CurrentContext;
                var resourceId =
                    context.Game.ResourceDefinitionDatabase.GetResourceIDFromName(engineMode.propellant.mixtureName);
                var resourceDefinitionData =
                    context.Game.ResourceDefinitionDatabase.GetDefinitionData(resourceId);

                return new KSPResourceModule.ResourceDefinitionAdapter(resourceDefinitionData);
            }
        }
    }
}
