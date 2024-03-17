﻿using System;
using System.Collections;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime;

public class CorouttineAdapter : IEnumerator {
    private readonly IKSPContext context;
    private readonly Action<string?, CoreError.StackEntry[]?> onDone;

    private object? current = new WaitForFixedUpdate();
    private IAnyFuture? process;

    public CorouttineAdapter(IAnyFuture process, IKSPContext context, Action<string?, CoreError.StackEntry[]?> onDone) {
        this.process = process;
        this.context = context;
        this.onDone = onDone;
    }

    object? IEnumerator.Current => current;

    public bool MoveNext() {
        if (process == null) return false;

        try {
            ContextHolder.CurrentContext.Value = context;
            context.ResetTimeout();
            var result = process.Poll();
            if (result.IsReady) {
                process = null;
                onDone(ExtractMessage(result.ValueObject), ExtractStackTrace(result.ValueObject));
                return false;
            } else {
                current = context.NextYield;
                return true;
            }
        } catch (Exception e) {
            context.Logger.Error($"Exception in process poll: {e.Message}");
            context.Logger.LogException(e);

            process = null;
            onDone(ExtractMessage(e), ExtractStackTrace(e));
            return false;
        } finally {
            ContextHolder.CurrentContext.Value = null;
        }
    }

    public void Reset() {
    }

    public void Dispose() {
    }

    private string? ExtractMessage(object? resultValue) {
        if (resultValue == null) return null;

        switch (resultValue) {
        case IAnyResult anyResult: return anyResult.ErrorString;
        case Exception exception: return exception.Message;
        default: return null;
        }
    }

    private CoreError.StackEntry[]? ExtractStackTrace(object? resultValue) {
        if (resultValue == null) return null;

        switch (resultValue) {
        case IAnyResult anyResult: return anyResult.ErrorObject?.StackTrace;
        default: return null;
        }
    }
}
