﻿using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath;

public static class Matrix2x2Binding {
    public static readonly RecordStructType Matrix2x2Type = new("ksp::math", "Matrix2x2",
        "A 2-dimensional matrix.", typeof(Matrix2x2),
        [
            new RecordStructField("a", "a", BuiltinType.Float, typeof(Matrix2x2).GetField("a")),
            new RecordStructField("b", "b", BuiltinType.Float, typeof(Matrix2x2).GetField("b")),
            new RecordStructField("c", "c", BuiltinType.Float, typeof(Matrix2x2).GetField("c")),
            new RecordStructField("d", "d", BuiltinType.Float, typeof(Matrix2x2).GetField("d"))
        ],
        new OperatorCollection {
            {
                Operator.Neg,
                new StaticMethodOperatorEmitter(() => BuiltinType.Unit, LazyMatrix2x2Type,
                    typeof(Matrix2x2).GetMethod("op_UnaryNegation", [typeof(Matrix2x2)]))
            }
        },
        new OperatorCollection {
            {
                Operator.Mul,
                new StaticMethodOperatorEmitter(() => Vector2Binding.Vector2Type, () => Vector2Binding.Vector2Type,
                    typeof(Matrix2x2).GetMethod("op_Multiply", [typeof(Matrix2x2), typeof(Vector2d)]))
            }
        },
        [],
        new Dictionary<string, IFieldAccessFactory> {
            {
                "determinant",
                new BoundPropertyLikeFieldAccessFactory("Get determinant of matrix", () => BuiltinType.Float,
                    typeof(Matrix2x2), typeof(Matrix2x2).GetProperty("Determinant"))
            }, {
                "inverse",
                new BoundPropertyLikeFieldAccessFactory("Invert matrix", LazyMatrix2x2Type,
                    typeof(Matrix2x2), typeof(Matrix2x2).GetProperty("Inverse"))
            }
        });

    private static RecordStructType LazyMatrix2x2Type() => Matrix2x2Type;

    public static Matrix2x2 Matrix2x2(double a, double b, double c, double d) {
        return new Matrix2x2(a, b, c, d);
    }
}
