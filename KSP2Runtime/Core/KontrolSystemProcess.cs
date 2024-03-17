﻿using System;
using KontrolSystem.KSP.Runtime.KSPGame;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Runtime;
using KSP.Sim.impl;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.Core;

public enum KontrolSystemProcessState {
    Available,
    Running,
    Outdated,
    Error
}

public class KontrolSystemProcess(ITO2Logger logger, IKontrolModule module) {
    public readonly Guid id = Guid.NewGuid();
    internal readonly ITO2Logger logger = logger;
    private readonly IKontrolModule module = module;
    internal KSPCoreContext? context;

    public string Name => module.Name;

    public KontrolSystemProcessState State { get; private set; } = KontrolSystemProcessState.Available;

    public void MarkRunning(KSPCoreContext newContext) {
        State = KontrolSystemProcessState.Running;
        context?.Cleanup();
        context = newContext;
    }

    public KontrolSystemProcess MarkOutdated() {
        State = KontrolSystemProcessState.Outdated;
        return this;
    }

    public void MarkDone(string? message, CoreError.StackEntry[]? stackTrace) {
        if (!string.IsNullOrEmpty(message)) {
            logger.Info($"Process {id} for module {module.Name} terminated with: {message}");
            context?.ConsoleBuffer.Print(
                $"\n\n>>>>> ERROR <<<<<<<<<\n\nModule {module.Name} terminated with:\n{message}");
            if (stackTrace != null) {
                context?.ConsoleBuffer.Print("\n\nStackTrace:\n");
                foreach (var stackEntry in stackTrace.Take(10)) {
                    context?.ConsoleBuffer.Print($"{stackEntry}\n");
                }
            }
        }

        State = KontrolSystemProcessState.Available;
        context?.Cleanup();
        context = null;
    }

    public Entrypoint? EntrypointFor(KSPGameMode gameMode, IKSPContext newContext) {
        switch (gameMode) {
        case KSPGameMode.KSC: return module.GetKSCEntrypoint(newContext);
        case KSPGameMode.VAB:
            return module.GetEditorEntrypoint(newContext);
        case KSPGameMode.Tracking: return module.GetTrackingEntrypoint(newContext);
        case KSPGameMode.Flight:
            return module.GetFlightEntrypoint(newContext);
        default:
            return null;
        }
    }

    public int EntrypointArgumentCount(KSPGameMode gameMode) {
        return module.GetEntrypointArgumentCount(gameMode);
    }

    public EntrypointArgumentDescriptor[] EntrypointArgumentDescriptors(KSPGameMode gameMode) {
        return module.GetEntrypointParameterDescriptors(gameMode);
    }

    public bool AvailableFor(KSPGameMode gameMode, VesselComponent vessel) {
        switch (gameMode) {
        case KSPGameMode.KSC: return module.HasKSCEntrypoint();
        case KSPGameMode.VAB: return module.HasEditorEntrypoint();
        case KSPGameMode.Tracking: return module.HasTrackingEntrypoint();
        case KSPGameMode.Flight:
            return (!module.Name.StartsWith("boot::") && module.HasFlightEntrypoint()) ||
                   module.IsBootFlightEntrypointFor(vessel);
        default:
            return false;
        }
    }

    public bool IsBootFor(KSPGameMode gameMode, VesselComponent vessel) {
        switch (gameMode) {
        case KSPGameMode.Flight: return module.IsBootFlightEntrypointFor(vessel);
        default: return false;
        }
    }
}
