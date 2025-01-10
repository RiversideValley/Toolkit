using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Graphics;
using Windows.UI;
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI.Xaml.Controls;

namespace Cube.UI.Animations
{
    public sealed class RefreshAnimation : IAnimatedVisualSource
    {
        // Animation duration: 1.000 seconds.
        internal const long c_durationTicks = 10000000;

        public Microsoft.UI.Xaml.Controls.IAnimatedVisual TryCreateAnimatedVisual(Compositor compositor)
        {
            object ignored = null;
            return TryCreateAnimatedVisual(compositor, out ignored);
        }

        public Microsoft.UI.Xaml.Controls.IAnimatedVisual TryCreateAnimatedVisual(Compositor compositor, out object diagnostics)
        {
            diagnostics = null;

            if (Message_AnimatedVisual.IsRuntimeCompatible())
            {
                var res =
                    new Message_AnimatedVisual(
                        compositor
                        );
                return res;
            }

            return null;
        }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public double FrameCount => 25d;

        /// <summary>
        /// Gets the frame rate of the animation.
        /// </summary>
        public double Framerate => 25d;

        /// <summary>
        /// Gets the duration of the animation.
        /// </summary>
        public TimeSpan Duration => TimeSpan.FromTicks(c_durationTicks);

        /// <summary>
        /// Converts a zero-based frame number to the corresponding progress value denoting the
        /// start of the frame.
        /// </summary>
        public double FrameToProgress(double frameNumber)
        {
            return frameNumber / 25d;
        }

        /// <summary>
        /// Returns a map from marker names to corresponding progress values.
        /// </summary>
        public IReadOnlyDictionary<string, double> Markers =>
            new Dictionary<string, double>
            {
            };

        /// <summary>
        /// Sets the color property with the given name, or does nothing if no such property
        /// exists.
        /// </summary>
        public void SetColorProperty(string propertyName, Color value)
        {
        }

        /// <summary>
        /// Sets the scalar property with the given name, or does nothing if no such property
        /// exists.
        /// </summary>
        public void SetScalarProperty(string propertyName, double value)
        {
        }

        sealed class Message_AnimatedVisual : Microsoft.UI.Xaml.Controls.IAnimatedVisual
        {
            const long c_durationTicks = 10000000;
            readonly Compositor _c;
            readonly ExpressionAnimation _reusableExpressionAnimation;
            ContainerVisual _root;
            CubicBezierEasingFunction _cubicBezierEasingFunction_0;

            void BindProperty(
                CompositionObject target,
                string animatedPropertyName,
                string expression,
                string referenceParameterName,
                CompositionObject referencedObject)
            {
                _reusableExpressionAnimation.ClearAllParameters();
                _reusableExpressionAnimation.Expression = expression;
                _reusableExpressionAnimation.SetReferenceParameter(referenceParameterName, referencedObject);
                target.StartAnimation(animatedPropertyName, _reusableExpressionAnimation);
            }

            ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(float initialProgress, float initialValue, CompositionEasingFunction initialEasingFunction)
            {
                var result = _c.CreateScalarKeyFrameAnimation();
                result.Duration = TimeSpan.FromTicks(c_durationTicks);
                result.InsertKeyFrame(initialProgress, initialValue, initialEasingFunction);
                return result;
            }

            CompositionSpriteShape CreateSpriteShape(CompositionGeometry geometry, Matrix3x2 transformMatrix, CompositionBrush fillBrush)
            {
                var result = _c.CreateSpriteShape(geometry);
                result.TransformMatrix = transformMatrix;
                result.FillBrush = fillBrush;
                return result;
            }

            // - - - - Shape tree root for layer: Refresh
            // - - ShapeGroup: 组 1 Offset:<439.375, 431.447>
            CanvasGeometry Geometry()
            {
                CanvasGeometry result;
                using (var builder = new CanvasPathBuilder(null))
                {
                    builder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Winding);
                    builder.BeginFigure(new Vector2(-0.0520000011F, -349.096985F));
                    builder.AddCubicBezier(new Vector2(-188.442993F, -349.063995F), new Vector2(-341.138F, -196.317001F), new Vector2(-341.105988F, -7.92600012F));
                    builder.AddCubicBezier(new Vector2(-341.072998F, 180.464996F), new Vector2(-188.326004F, 333.158997F), new Vector2(0.0649999976F, 333.127014F));
                    builder.AddCubicBezier(new Vector2(188.455994F, 333.093994F), new Vector2(341.149994F, 180.347F), new Vector2(341.118011F, -8.04399967F));
                    builder.AddCubicBezier(new Vector2(341.11499F, -24.8920002F), new Vector2(339.864014F, -41.7159996F), new Vector2(337.375F, -58.3790016F));
                    builder.AddCubicBezier(new Vector2(334.328003F, -79.0270004F), new Vector2(349.428009F, -98.947998F), new Vector2(370.304993F, -98.947998F));
                    builder.AddCubicBezier(new Vector2(387.132996F, -98.947998F), new Vector2(401.959015F, -87.3050003F), new Vector2(404.506012F, -70.612999F));
                    builder.AddCubicBezier(new Vector2(439.125F, 152.789001F), new Vector2(286.087006F, 361.958008F), new Vector2(62.6839981F, 396.576996F));
                    builder.AddCubicBezier(new Vector2(-160.718002F, 431.196991F), new Vector2(-369.885986F, 278.15799F), new Vector2(-404.505005F, 54.7560005F));
                    builder.AddCubicBezier(new Vector2(-439.125F, -168.647003F), new Vector2(-286.087006F, -377.815002F), new Vector2(-62.6850014F, -412.434998F));
                    builder.AddCubicBezier(new Vector2(58.3889999F, -431.196991F), new Vector2(181.501999F, -394.739014F), new Vector2(272.835999F, -313.075989F));
                    builder.AddLine(new Vector2(272.835999F, -360.46701F));
                    builder.AddCubicBezier(new Vector2(272.835999F, -379.306F), new Vector2(288.109009F, -394.57901F), new Vector2(306.947998F, -394.57901F));
                    builder.AddCubicBezier(new Vector2(325.786987F, -394.57901F), new Vector2(341.059998F, -379.306F), new Vector2(341.059998F, -360.46701F));
                    builder.AddLine(new Vector2(341.059998F, -224.022995F));
                    builder.AddCubicBezier(new Vector2(341.059998F, -205.184006F), new Vector2(325.786987F, -189.910995F), new Vector2(306.947998F, -189.910995F));
                    builder.AddLine(new Vector2(170.503006F, -189.910995F));
                    builder.AddCubicBezier(new Vector2(151.664001F, -189.910995F), new Vector2(136.393005F, -205.184006F), new Vector2(136.393005F, -224.022995F));
                    builder.AddCubicBezier(new Vector2(136.393005F, -242.862F), new Vector2(151.664001F, -258.134003F), new Vector2(170.503006F, -258.134003F));
                    builder.AddLine(new Vector2(231.904007F, -258.134003F));
                    builder.AddCubicBezier(new Vector2(168.884995F, -316.718994F), new Vector2(85.9929962F, -349.226013F), new Vector2(-0.0520000011F, -349.096985F));
                    builder.EndFigure(CanvasFigureLoop.Closed);
                    result = CanvasGeometry.CreatePath(builder);
                }
                return result;
            }

            // - - Shape tree root for layer: Refresh
            // ShapeGroup: 组 1 Offset:<439.375, 431.447>
            CompositionColorBrush ColorBrush_White()
            {
                return _c.CreateColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            }

            // Shape tree root for layer: Refresh
            CompositionContainerShape ContainerShape()
            {
                var result = _c.CreateContainerShape();
                result.CenterPoint = new Vector2(439.375F, 431.446991F);
                result.Offset = new Vector2(96.9790039F, 116.08902F);
                // ShapeGroup: 组 1 Offset:<439.375, 431.447>
                result.Shapes.Add(SpriteShape());
                result.StartAnimation("RotationAngleInDegrees", RotationAngleInDegreesScalarAnimation_0_to_360());
                var controller = result.TryGetAnimationController("RotationAngleInDegrees");
                controller.Pause();
                BindProperty(controller, "Progress", "_.Progress", "_", _root);
                return result;
            }

            // - - Shape tree root for layer: Refresh
            // ShapeGroup: 组 1 Offset:<439.375, 431.447>
            CompositionPathGeometry PathGeometry()
            {
                return _c.CreatePathGeometry(new CompositionPath(Geometry()));
            }

            // - Shape tree root for layer: Refresh
            // 路径 1
            CompositionSpriteShape SpriteShape()
            {
                // Offset:<439.375, 431.447>
                var geometry = PathGeometry();
                var result = CreateSpriteShape(geometry, new Matrix3x2(1F, 0F, 0F, 1F, 439.375F, 431.446991F), ColorBrush_White());
                return result;
            }

            // The root of the composition.
            ContainerVisual Root()
            {
                var result = _root = _c.CreateContainerVisual();
                var propertySet = result.Properties;
                propertySet.InsertScalar("Progress", 0F);
                // Shape tree root for layer: Refresh
                result.Children.InsertAtTop(ShapeVisual_0());
                return result;
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_0()
            {
                return _cubicBezierEasingFunction_0 = _c.CreateCubicBezierEasingFunction(new Vector2(0.166999996F, 0.166999996F), new Vector2(0.833000004F, 0.833000004F));
            }

            // - Shape tree root for layer: Refresh
            // Rotation
            ScalarKeyFrameAnimation RotationAngleInDegreesScalarAnimation_0_to_360()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, HoldThenStepEasingFunction());
                // Frame 5.
                result.InsertKeyFrame(0.200000003F, 28F, CubicBezierEasingFunction_0());
                // Frame 10.
                result.InsertKeyFrame(0.400000006F, 189.332993F, _cubicBezierEasingFunction_0);
                // Frame 15.
                result.InsertKeyFrame(0.600000024F, 275.666992F, _cubicBezierEasingFunction_0);
                // Frame 20.
                result.InsertKeyFrame(0.800000012F, 360F, _cubicBezierEasingFunction_0);
                return result;
            }

            // Shape tree root for layer: Refresh
            ShapeVisual ShapeVisual_0()
            {
                var result = _c.CreateShapeVisual();
                result.Size = new Vector2(1080F, 1080F);
                result.Shapes.Add(ContainerShape());
                return result;
            }

            // - - Shape tree root for layer: Refresh
            // RotationAngleInDegrees
            StepEasingFunction HoldThenStepEasingFunction()
            {
                var result = _c.CreateStepEasingFunction();
                result.IsFinalStepSingleFrame = true;
                return result;
            }

            internal Message_AnimatedVisual(
                Compositor compositor
                )
            {
                _c = compositor;
                _reusableExpressionAnimation = compositor.CreateExpressionAnimation();
                Root();
            }

            public Visual RootVisual => _root;
            public TimeSpan Duration => TimeSpan.FromTicks(c_durationTicks);
            public Vector2 Size => new Vector2(1080F, 1080F);
            void IDisposable.Dispose() => _root?.Dispose();

            internal static bool IsRuntimeCompatible()
            {
                return Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7);
            }
        }
    }
}
