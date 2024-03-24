﻿using System.Linq;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using KSP.Modules;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPScience;

public partial class KSPScienceModule {
    [KSClass("Experiment",
        Description = "Represents an in-game science experiment.")]
    public class ExperimentAdapter(SimulationObjectModel simulationObject, ExperimentStanding experimentStanding,
        ExperimentConfiguration experimentConfiguration) : BaseExperimentAdapter(experimentConfiguration) {
        private readonly ExperimentStanding experimentStanding = experimentStanding;
        private readonly SimulationObjectModel simulationObject = simulationObject;

        [KSField] public bool HasEnoughResources => experimentStanding.HasEnoughResources;

        [KSField] public bool CurrentSituationIsValid => experimentStanding.CurrentSituationIsValid;

        [KSField] public double CurrentRunningTime => experimentStanding.CurrentRunningTime;

        [KSField] public ExperimentState CurrentExperimentState => experimentStanding.CurrentExperimentState;

        [KSField] public ExperimentState PreviousExperimentState => experimentStanding.PreviousExperimentState;

        [KSField]
        public ResearchLocationAdapter[] ValidLocations =>
            experimentStanding.ExperimentDefinition.ValidLocations
                .Select(location => new ResearchLocationAdapter(location)).ToArray();

        [KSField] public bool RegionRequired => experimentStanding.RegionRequired;

        [KSField(Description = "Get the research location the experiment was last performed.")]
        public Option<ResearchLocationAdapter> ExperimentLocation =>
            experimentStanding.ExperimentLocation != null
                ? new Option<ResearchLocationAdapter>(
                    new ResearchLocationAdapter(experimentStanding.ExperimentLocation))
                : new Option<ResearchLocationAdapter>();

        [KSMethod]
        public bool PauseExperiment() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(simulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_ScienceExperiment>(out var moduleScienceExperiment)) return false;

            moduleScienceExperiment.OnPauseExperiment(experimentStanding.ExperimentID);
            return true;
        }

        [KSMethod]
        public bool CancelExperiment() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(simulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_ScienceExperiment>(out var moduleScienceExperiment)) return false;

            moduleScienceExperiment.OnCancelExperiment(experimentStanding.ExperimentID);
            return true;
        }

        [KSMethod]
        public bool RunExperiment() {
            if (!KSPContext.CurrentContext.Game.SpaceSimulation.TryGetViewObject(simulationObject,
                    out var viewObject)) return false;

            if (!viewObject.TryGetComponent<Module_ScienceExperiment>(out var moduleScienceExperiment)) return false;

            moduleScienceExperiment.OnAttemptToRunExperiment(experimentStanding.ExperimentID);
            return true;
        }
    }
}
