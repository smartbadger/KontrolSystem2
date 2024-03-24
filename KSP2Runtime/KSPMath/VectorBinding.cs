﻿using System.Collections.Generic;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using KSP.Sim;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public class VectorBinding {
    public static readonly BoundType VectorType = Direct.BindType("ksp::math", "GlobalVector",
        "Abstract vector in space that can be projected to a concrete 3-dimensional vector in a specific coordinate system",
        typeof(Vector),
        new OperatorCollection {
            {
                Operator.Neg,
                new StaticMethodOperatorEmitter(() => BuiltinType.Unit, LazyVectorType,
                    typeof(Vector).GetMethod("negate", [typeof(Vector)]))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(() => BuiltinType.Float, LazyVectorType,
                    typeof(VectorBinding).GetMethod("Multiply", [typeof(double), typeof(Vector)]))
            }
        },
        new OperatorCollection {
            {
                Operator.Add,
                new StaticMethodOperatorEmitter(() => VectorType!, LazyVectorType,
                    typeof(Vector).GetMethod("op_Addition", [typeof(Vector), typeof(Vector)]))
            }, {
                Operator.AddAssign,
                new StaticMethodOperatorEmitter(() => VectorType!, LazyVectorType,
                    typeof(Vector).GetMethod("op_Addition", [typeof(Vector), typeof(Vector)]))
            }, {
                Operator.Sub,
                new StaticMethodOperatorEmitter(() => VectorType!, LazyVectorType,
                    typeof(Vector).GetMethod("op_Subtraction", [typeof(Vector), typeof(Vector)]))
            }, {
                Operator.SubAssign,
                new StaticMethodOperatorEmitter(() => VectorType!, LazyVectorType,
                    typeof(Vector).GetMethod("op_Subtraction", [typeof(Vector), typeof(Vector)]))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(() => VectorType!, () => BuiltinType.Float,
                    typeof(Vector).GetMethod("dot"))
            }, {
                Operator.Mul,
                new StaticMethodOperatorEmitter(() => BuiltinType.Float, LazyVectorType,
                    typeof(Vector).GetMethod("op_Multiply", [typeof(Vector), typeof(double)]))
            }, {
                Operator.MulAssign,
                new StaticMethodOperatorEmitter(() => BuiltinType.Float, LazyVectorType,
                    typeof(Vector).GetMethod("op_Multiply", [typeof(Vector), typeof(double)]))
            }
        },
        new Dictionary<string, IMethodInvokeFactory> {
            {
                "to_local",
                new BoundMethodInvokeFactory("Get local vector in a coordinate system", true,
                    () => Vector3Binding.Vector3Type,
                    () => [new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference")], false,
                    typeof(VectorBinding), typeof(VectorBinding).GetMethod("ToLocal"))
            }, {
                "to_string",
                new BoundMethodInvokeFactory("Convert vector to string in a given coordinate system.", true,
                    () => BuiltinType.String,
                    () => [new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference")], false, typeof(VectorBinding),
                    typeof(VectorBinding).GetMethod("ToString", [typeof(Vector), typeof(ITransformFrame)]))
            }, {
                "to_fixed",
                new BoundMethodInvokeFactory(
                    "Convert the vector to string with fixed number of `decimals` in a given coordinate system.",
                    true,
                    () => BuiltinType.String,
                    () => [
                        new("frame", TransformFrameBinding.TransformFrameType, "Frame of reference"),
                        new("decimals", BuiltinType.Int, "Number of decimals")
                    ],
                    false, typeof(VectorBinding), typeof(VectorBinding).GetMethod("ToFixed"))
            }, {
                "to_direction",
                new BoundMethodInvokeFactory("Convert the vector to a rotation/direction in space.",
                    true,
                    () => RotationBinding.RotationType,
                    () => [],
                    false, typeof(VectorBinding), typeof(VectorBinding).GetMethod("ToDirection"))
            }, {
                "cross",
                new BoundMethodInvokeFactory("Calculate the cross/other product with `other` vector.", true,
                    LazyVectorType,
                    () => [new("other", VectorType!, "Other vector")], false,
                    typeof(Vector), typeof(Vector).GetMethod("cross"))
            }, {
                "dot",
                new BoundMethodInvokeFactory("Calculate the dot/inner product with `other` vector.", true,
                    () => BuiltinType.Float,
                    () => [new("other", VectorType!, "Other vector")], false,
                    typeof(Vector), typeof(Vector).GetMethod("dot"))
            }, {
                "lerp_to",
                new BoundMethodInvokeFactory(
                    "Linear interpolate position between this and `other` vector, where `t = 0.0` is this and `t = 1.0` is `other`.",
                    true,
                    LazyVectorType,
                    () => [
                        new("other", VectorType!, "Other vector"),
                        new("t", BuiltinType.Float, "Relative position of mid-point (0.0 - 1.0)")
                    ], false, typeof(Vector), typeof(Vector).GetMethod("Lerp"))
            }, {
                "exclude_from",
                new BoundMethodInvokeFactory("Exclude this from `other` vector.", true, LazyVectorType,
                    () => [new("other", VectorType!, "Other vector")], false,
                    typeof(VectorBinding), typeof(VectorBinding).GetMethod("ExcludeFrom"))
            }
        },
        new Dictionary<string, IFieldAccessFactory> {
            {
                "magnitude",
                new BoundPropertyLikeFieldAccessFactory("Magnitude/length of the vector", () => BuiltinType.Float,
                    typeof(Vector), typeof(Vector).GetProperty("magnitude"))
            }, {
                "sqr_magnitude",
                new BoundPropertyLikeFieldAccessFactory("Squared magnitude of the vector", () => BuiltinType.Float,
                    typeof(Vector), typeof(Vector).GetProperty("sqrMagnitude"))
            }, {
                "normalized",
                new BoundPropertyLikeFieldAccessFactory("Normalized vector (i.e. scaled to length 1)",
                    LazyVectorType, typeof(Vector), typeof(Vector).GetMethod("normalize"), null)
            }
        });

    private static BoundType LazyVectorType() => VectorType;

    public static Vector3d ToLocal(Vector vector, ITransformFrame frame) {
        return frame.ToLocalVector(vector);
    }

    public static string ToString(Vector v, ITransformFrame frame) {
        return Vector3Binding.ToString(frame.ToLocalVector(v));
    }

    public static string ToFixed(Vector v, ITransformFrame frame, long decimals) {
        return Vector3Binding.ToFixed(frame.ToLocalVector(v), decimals);
    }

    public static RotationWrapper ToDirection(Vector v) {
        return new RotationWrapper(v);
    }

    public static Vector ExcludeFrom(Vector v, Vector other) {
        var otherLocal = v.coordinateSystem.ToLocalVector(other);
        return new Vector(v.coordinateSystem, Vector3d.Exclude(v.vector, otherLocal));
    }

    public static Vector Multiply(double scale, Vector v) {
        return v * scale;
    }
}
