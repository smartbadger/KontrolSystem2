﻿using KontrolSystem.TO2.Binding;
using KSP.Game.Science;
using KSP.Sim.impl;

namespace KontrolSystem.KSP.Runtime.KSPScience {
    public partial class KSPScienceModule {
        [KSClass("ResearchReport",
            Description = "Represents the stored report of a science experiment")]
        public class ResearchReportAdapter {
            private readonly ResearchReport researchReport;
            private readonly ScienceStorageComponent scienceStorageComponnt;
            
            public ResearchReportAdapter(ScienceStorageComponent scienceStorageComponnt, ResearchReport researchReport) {
                this.scienceStorageComponnt = scienceStorageComponnt;
                this.researchReport = researchReport;
            }

            [KSField(Description = "Get the research location the experiment was performed at.")]
            public ResearchLocationAdapter ResearchLocation => new ResearchLocationAdapter(researchReport.Location);

            [KSField(Description = "Get the definition of the experiment.")]
            public ExperimentDefinitionAdapter Definition => new ExperimentDefinitionAdapter(
                KSPContext.CurrentContext.Game.ScienceManager.ScienceExperimentsDataStore.GetExperimentDefinition(
                    researchReport.ExperimentID)
            );

            [KSField]
            public bool TransmissionStatus => researchReport.TransmissionStatus;

            [KSField]
            public double TransmissionPercentage => researchReport.TransmissionPercentage;

            [KSField]
            public double EcRequired => researchReport.EcRequired;

            [KSField]
            public double TimeRequired => researchReport.TimeRequired;

            [KSField]
            public double TransmissionSize => researchReport.TransmissionSize;

            [KSMethod]
            public bool StartTransmit() =>
                scienceStorageComponnt.StartReportTransmission(researchReport.ResearchReportKey);
        }
    }
}