﻿using System.Collections.Generic;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPUI;

public partial class KSPUIModule {
    public abstract class Transform2D(Matrix4x4 transform) : AbstractContainer2D, GLUIDrawer.IGLUIDrawable {
        protected List<GLUIDrawer.IGLUIDrawable> elements = [];
        protected Matrix4x4 transform = transform;

        public void OnDraw(GLUIDrawer.GLUIDraw draw) {
            var currentTransform = draw.CurrentTransform;
            GL.PushMatrix();
            GL.MultMatrix(currentTransform * transform);
            draw.CurrentTransform = currentTransform * transform;

            foreach (var element in elements) element.OnDraw(draw);

            GL.PopMatrix();
            draw.CurrentTransform = currentTransform;
        }


        protected override void AddElement(GLUIDrawer.IGLUIDrawable element) {
            elements.Add(element);
        }

        protected override void RemoveAllElements() {
            elements.Clear();
        }
    }

    [KSClass]
    public class Translate2D(Vector2d translate) : Transform2D(
        Matrix4x4.Translate(new Vector3((float)translate.x, (float)translate.y))) {
        private Vector2d translate = translate;

        [KSField]
        public Vector2d Translate {
            get => translate;
            set {
                translate = value;
                transform = Matrix4x4.Translate(new Vector3((float)value.x, (float)value.y));
            }
        }
    }

    [KSClass]
    public class Scale2D(Vector2d scale, Vector2d pivot = default) : Transform2D(MakeTransform(scale, pivot)) {
        private Vector2d pivot;
        private Vector2d scale = scale;

        [KSField]
        public Vector2d Scale {
            get => scale;
            set {
                scale = value;
                transform = MakeTransform(scale, pivot);
            }
        }

        [KSField]
        public Vector2d Pivot {
            get => pivot;
            set {
                pivot = value;
                transform = MakeTransform(scale, value);
            }
        }

        private static Matrix4x4 MakeTransform(Vector2d scale, Vector2d pivot) {
            return Matrix4x4.Translate(new Vector3((float)pivot.x, (float)pivot.y)) *
                   Matrix4x4.Scale(new Vector3((float)scale.x, (float)scale.y)) *
                   Matrix4x4.Translate(new Vector3(-(float)pivot.x, -(float)pivot.y));
        }
    }

    [KSClass]
    public class Rotate2D(double degrees, Vector2d pivot = default) : Transform2D(MakeTransform(degrees, pivot)) {
        private double degrees = degrees;
        private Vector2d pivot = pivot;

        [KSField]
        public double Degrees {
            get => degrees;
            set {
                degrees = value;
                transform = MakeTransform(value, pivot);
            }
        }

        [KSField]
        public Vector2d Pivot {
            get => pivot;
            set {
                pivot = value;
                transform = MakeTransform(degrees, value);
            }
        }

        private static Matrix4x4 MakeTransform(double degrees, Vector2d pivot) {
            return Matrix4x4.Translate(new Vector3((float)pivot.x, (float)pivot.y)) *
                   Matrix4x4.Rotate(Quaternion.Euler(Vector3.forward * (float)degrees)) *
                   Matrix4x4.Translate(new Vector3(-(float)pivot.x, -(float)pivot.y));
        }
    }
}
