﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace KontrolSystem.TO2.Generator;

public interface ILocalRef {
    int LocalIndex { get; }

    Type LocalType { get; }
}

public interface ITempLocalRef : ILocalRef, IDisposable {
}

public readonly struct LabelRef(Label label, bool isShort) {
    public readonly Label label = label;
    public readonly bool isShort = isShort;
}

public interface IILEmitter {
    int LastLocalIndex { get; }

    int ILSize { get; }

    int StackCount { get; }
    void Emit(OpCode opCode);

    void Emit(OpCode opCode, byte arg);

    void Emit(OpCode opCode, short arg);

    void Emit(OpCode opCode, int arg);

    void Emit(OpCode opCode, long arg);

    void Emit(OpCode opCode, double arg);

    void Emit(OpCode opCode, string arg);

    void Emit(OpCode opCode, LabelRef label);

    void Emit(OpCode opCode, IEnumerable<LabelRef> labels);

    void Emit(OpCode opCode, ILocalRef localBuilder);

    void Emit(OpCode opCode, FieldInfo field);

    void Emit(OpCode opCode, Type type, int? argumentCount = null, int? resultCount = null);

    void EmitPtr(OpCode opCode, MethodInfo methodInfo);

    void EmitNew(OpCode opCode, ConstructorInfo constructor, int? argumentCount = null, int? resultCount = null);

    void EmitCall(OpCode opCode, MethodInfo methodInfo, int argumentCount);

    ILocalRef DeclareLocal(Type type);

    ITempLocalRef TempLocal(Type type);

    LabelRef DefineLabel(bool isShort);

    void MarkLabel(LabelRef label);

    void BeginScope();

    void EndScope();

    void EmitReturn(Type returnType);

    void AdjustStack(int diff);
}

public class CodeGenerationException(string message) : Exception(message) {
}
