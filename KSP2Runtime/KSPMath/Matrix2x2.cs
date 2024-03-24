﻿using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath;

//  [a    b]
//  [c    d]
public readonly struct Matrix2x2(double a, double b, double c, double d) {
    public readonly double a = a, b = b, c = c, d = d;

    public readonly double Determinant => a * d - b * c;

    public readonly Matrix2x2 Inverse {
        get {
            var det = a * d - b * c;
            return new Matrix2x2(d / det, -b / det, -c / det, a / det);
        }
    }

    public static Matrix2x2 operator -(Matrix2x2 m) {
        return new Matrix2x2(-m.a, -m.b, -m.c, -m.d);
    }

    public static Vector2d operator *(Matrix2x2 m, Vector2d vec) {
        return new Vector2d(m.a * vec.x + m.b * vec.y, m.c * vec.x + m.d * vec.y);
    }
}
