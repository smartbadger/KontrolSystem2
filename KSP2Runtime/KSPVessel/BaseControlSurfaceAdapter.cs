﻿using KontrolSystem.TO2.Binding;
using KSP.Modules;
using KSP.Sim.DeltaV;

namespace KontrolSystem.KSP.Runtime.KSPVessel;

public class BaseControlSurfaceAdapter<P, T> : BaseModuleAdapter<P, T> where P : BasePartAdapter<T> where T : IDeltaVPart {
    protected readonly Data_ControlSurface dataControlSurface;

    protected BaseControlSurfaceAdapter(P part, Data_ControlSurface dataControlSurface) : base(part) {
        this.dataControlSurface = dataControlSurface;
    }

    [KSField]
    public bool InvertControl {
        get => dataControlSurface.InvertControl.GetValue();
        set => dataControlSurface.InvertControl.SetValue(value);
    }

    [KSField]
    public bool EnableRoll {
        get => dataControlSurface.EnableRoll.GetValue();
        set => dataControlSurface.EnableRoll.SetValue(value);
    }

    [KSField]
    public bool EnableYaw {
        get => dataControlSurface.EnableYaw.GetValue();
        set => dataControlSurface.EnableYaw.SetValue(value);
    }

    [KSField]
    public bool EnablePitch {
        get => dataControlSurface.EnablePitch.GetValue();
        set => dataControlSurface.EnablePitch.SetValue(value);
    }

    [KSField]
    public double AuthorityLimiter {
        get => dataControlSurface.AuthorityLimiter.GetValue();
        set => dataControlSurface.AuthorityLimiter.SetValue((float)value);
    }


    [KSField]
    public double LiftDragRatio => dataControlSurface.LiftDragRatioParent.GetValue();

    [KSField] public double Lift => dataControlSurface.LiftScalar.GetValue();

    [KSField] public double Drag => dataControlSurface.DragScalar.GetValue();

    [KSField] public double AngleOfAttack => dataControlSurface.AoA.GetValue();
}
