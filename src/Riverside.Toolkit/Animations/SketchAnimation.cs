using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;
using Windows.Graphics;
using Windows.UI;
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Geometry;

namespace Cube.UI.Animations
{
    public sealed class SketchAnimation : IAnimatedVisualSource2
    {
        // Animation duration: 1.720 seconds.
        internal const long c_durationTicks = 17200000;

        public Microsoft.UI.Xaml.Controls.IAnimatedVisual TryCreateAnimatedVisual(Compositor compositor)
        {
            object ignored = null;
            return TryCreateAnimatedVisual(compositor, out ignored);
        }

        public Microsoft.UI.Xaml.Controls.IAnimatedVisual TryCreateAnimatedVisual(Compositor compositor, out object diagnostics)
        {
            diagnostics = null;

            if (Data_AnimatedVisual.IsRuntimeCompatible())
            {
                var res =
                    new Data_AnimatedVisual(
                        compositor
                        );
                return res;
            }

            return null;
        }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public double FrameCount => 43d;

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
            return frameNumber / 43d;
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

        sealed class Data_AnimatedVisual : Microsoft.UI.Xaml.Controls.IAnimatedVisual
        {
            const long c_durationTicks = 17200000;
            readonly Compositor _c;
            readonly ExpressionAnimation _reusableExpressionAnimation;
            ContainerVisual _root;
            CubicBezierEasingFunction _cubicBezierEasingFunction_0;
            CubicBezierEasingFunction _cubicBezierEasingFunction_1;
            ExpressionAnimation _rootProgress;
            StepEasingFunction _holdThenStepEasingFunction;
            StepEasingFunction _stepThenHoldEasingFunction;

            static void StartProgressBoundAnimation(
                CompositionObject target,
                string animatedPropertyName,
                CompositionAnimation animation,
                ExpressionAnimation controllerProgressExpression)
            {
                target.StartAnimation(animatedPropertyName, animation);
                var controller = target.TryGetAnimationController(animatedPropertyName);
                controller.Pause();
                controller.StartAnimation("Progress", controllerProgressExpression);
            }

            ColorKeyFrameAnimation CreateColorKeyFrameAnimation(float initialProgress, Color initialValue, CompositionEasingFunction initialEasingFunction)
            {
                var result = _c.CreateColorKeyFrameAnimation();
                result.Duration = TimeSpan.FromTicks(c_durationTicks);
                result.InterpolationColorSpace = CompositionColorSpace.Rgb;
                result.InsertKeyFrame(initialProgress, initialValue, initialEasingFunction);
                return result;
            }

            ScalarKeyFrameAnimation CreateScalarKeyFrameAnimation(float initialProgress, float initialValue, CompositionEasingFunction initialEasingFunction)
            {
                var result = _c.CreateScalarKeyFrameAnimation();
                result.Duration = TimeSpan.FromTicks(c_durationTicks);
                result.InsertKeyFrame(initialProgress, initialValue, initialEasingFunction);
                return result;
            }

            Vector2KeyFrameAnimation CreateVector2KeyFrameAnimation(float initialProgress, Vector2 initialValue, CompositionEasingFunction initialEasingFunction)
            {
                var result = _c.CreateVector2KeyFrameAnimation();
                result.Duration = TimeSpan.FromTicks(c_durationTicks);
                result.InsertKeyFrame(initialProgress, initialValue, initialEasingFunction);
                return result;
            }

            CompositionSpriteShape CreateSpriteShape(CompositionGeometry geometry, Matrix3x2 transformMatrix)
            {
                var result = _c.CreateSpriteShape(geometry);
                result.TransformMatrix = transformMatrix;
                return result;
            }

            CompositionSpriteShape CreateSpriteShape(CompositionGeometry geometry, Matrix3x2 transformMatrix, CompositionBrush fillBrush)
            {
                var result = _c.CreateSpriteShape(geometry);
                result.TransformMatrix = transformMatrix;
                result.FillBrush = fillBrush;
                return result;
            }

            // - - - - Layer aggregator
            // - - ShapeGroup: 组 1 Offset:<270.253, 450.25>
            CanvasGeometry Geometry_0()
            {
                CanvasGeometry result;
                using (var builder = new CanvasPathBuilder(null))
                {
                    builder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Winding);
                    builder.BeginFigure(new Vector2(-202.5F, -416.25F));
                    builder.AddCubicBezier(new Vector2(-202.5F, -425.200989F), new Vector2(-206.056F, -433.785004F), new Vector2(-212.384995F, -440.11499F));
                    builder.AddCubicBezier(new Vector2(-218.714996F, -446.444F), new Vector2(-227.298996F, -450F), new Vector2(-236.25F, -450F));
                    builder.AddCubicBezier(new Vector2(-245.201004F, -450F), new Vector2(-253.785995F, -446.444F), new Vector2(-260.11499F, -440.11499F));
                    builder.AddCubicBezier(new Vector2(-266.444F, -433.785004F), new Vector2(-270F, -425.200989F), new Vector2(-270F, -416.25F));
                    builder.AddLine(new Vector2(-270F, -281.25F));
                    builder.AddCubicBezier(new Vector2(-270.002991F, -261.975006F), new Vector2(-262.937012F, -243.367996F), new Vector2(-250.139999F, -228.953995F));
                    builder.AddCubicBezier(new Vector2(-237.343002F, -214.539993F), new Vector2(-219.705002F, -205.320007F), new Vector2(-200.565002F, -203.039993F));
                    builder.AddLine(new Vector2(-246.285004F, -90.3150024F));
                    builder.AddCubicBezier(new Vector2(-255.348007F, -67.6800003F), new Vector2(-259.554993F, -43.3930016F), new Vector2(-258.631989F, -19.0289993F));
                    builder.AddCubicBezier(new Vector2(-257.709991F, 5.33599997F), new Vector2(-251.679001F, 29.2350006F), new Vector2(-240.929993F, 51.1199989F));
                    builder.AddLine(new Vector2(-61.3800011F, 408.869995F));
                    builder.AddCubicBezier(new Vector2(-48.6450005F, 434.339996F), new Vector2(-25.2450008F, 450F), new Vector2(0F, 450F));
                    builder.AddCubicBezier(new Vector2(25.2450008F, 450F), new Vector2(48.5999985F, 434.339996F), new Vector2(61.4249992F, 408.915009F));
                    builder.AddLine(new Vector2(240.975006F, 51.1199989F));
                    builder.AddCubicBezier(new Vector2(262.619995F, 7.92000008F), new Vector2(264.644989F, -45.0449982F), new Vector2(246.285004F, -90.3150024F));
                    builder.AddLine(new Vector2(200.565002F, -203.039993F));
                    builder.AddCubicBezier(new Vector2(219.705002F, -205.320007F), new Vector2(237.343002F, -214.539993F), new Vector2(250.139999F, -228.953995F));
                    builder.AddCubicBezier(new Vector2(262.937012F, -243.367996F), new Vector2(270.002991F, -261.975006F), new Vector2(270F, -281.25F));
                    builder.AddLine(new Vector2(270F, -416.25F));
                    builder.AddCubicBezier(new Vector2(270F, -425.200989F), new Vector2(266.444F, -433.785004F), new Vector2(260.11499F, -440.11499F));
                    builder.AddCubicBezier(new Vector2(253.785995F, -446.444F), new Vector2(245.201004F, -450F), new Vector2(236.25F, -450F));
                    builder.AddCubicBezier(new Vector2(227.298996F, -450F), new Vector2(218.714005F, -446.444F), new Vector2(212.384995F, -440.11499F));
                    builder.AddCubicBezier(new Vector2(206.056F, -433.785004F), new Vector2(202.5F, -425.200989F), new Vector2(202.5F, -416.25F));
                    builder.AddLine(new Vector2(202.5F, -281.25F));
                    builder.AddCubicBezier(new Vector2(202.5F, -278.265991F), new Vector2(201.315002F, -275.404999F), new Vector2(199.205002F, -273.295013F));
                    builder.AddCubicBezier(new Vector2(197.095001F, -271.184998F), new Vector2(194.233994F, -270F), new Vector2(191.25F, -270F));
                    builder.AddLine(new Vector2(-191.25F, -270F));
                    builder.AddCubicBezier(new Vector2(-194.233994F, -270F), new Vector2(-197.095001F, -271.184998F), new Vector2(-199.205002F, -273.295013F));
                    builder.AddCubicBezier(new Vector2(-201.315002F, -275.404999F), new Vector2(-202.5F, -278.265991F), new Vector2(-202.5F, -281.25F));
                    builder.AddLine(new Vector2(-202.5F, -416.25F));
                    builder.EndFigure(CanvasFigureLoop.Closed);
                    builder.BeginFigure(new Vector2(127.934998F, -202.5F));
                    builder.AddLine(new Vector2(183.735001F, -64.9349976F));
                    builder.AddCubicBezier(new Vector2(194.850006F, -37.4850006F), new Vector2(193.5F, -4.81500006F), new Vector2(180.585007F, 20.8349991F));
                    builder.AddLine(new Vector2(33.75F, 313.515015F));
                    builder.AddLine(new Vector2(33.75F, 13.4549999F));
                    builder.AddCubicBezier(new Vector2(46.618F, 6.0250001F), new Vector2(56.6759987F, -5.44299984F), new Vector2(62.3619995F, -19.1709995F));
                    builder.AddCubicBezier(new Vector2(68.0479965F, -32.8989983F), new Vector2(69.0459976F, -48.1189995F), new Vector2(65.1999969F, -62.4720001F));
                    builder.AddCubicBezier(new Vector2(61.3540001F, -76.8249969F), new Vector2(52.8800011F, -89.5070038F), new Vector2(41.0909996F, -98.5530014F));
                    builder.AddCubicBezier(new Vector2(29.3029995F, -107.598999F), new Vector2(14.8590002F, -112.501999F), new Vector2(0F, -112.501999F));
                    builder.AddCubicBezier(new Vector2(-14.8590002F, -112.501999F), new Vector2(-29.3029995F, -107.598999F), new Vector2(-41.0909996F, -98.5530014F));
                    builder.AddCubicBezier(new Vector2(-52.8800011F, -89.5070038F), new Vector2(-61.3540001F, -76.8249969F), new Vector2(-65.1999969F, -62.4720001F));
                    builder.AddCubicBezier(new Vector2(-69.0459976F, -48.1189995F), new Vector2(-68.0479965F, -32.8989983F), new Vector2(-62.3619995F, -19.1709995F));
                    builder.AddCubicBezier(new Vector2(-56.6759987F, -5.44299984F), new Vector2(-46.618F, 6.0250001F), new Vector2(-33.75F, 13.4549999F));
                    builder.AddLine(new Vector2(-33.75F, 313.515015F));
                    builder.AddLine(new Vector2(-180.630005F, 20.8349991F));
                    builder.AddCubicBezier(new Vector2(-187.065994F, 7.53499985F), new Vector2(-190.664993F, -6.95800018F), new Vector2(-191.199005F, -21.7240009F));
                    builder.AddCubicBezier(new Vector2(-191.733994F, -36.4889984F), new Vector2(-189.192001F, -51.2039986F), new Vector2(-183.735001F, -64.9349976F));
                    builder.AddLine(new Vector2(-127.934998F, -202.5F));
                    builder.AddLine(new Vector2(127.934998F, -202.5F));
                    builder.EndFigure(CanvasFigureLoop.Closed);
                    result = CanvasGeometry.CreatePath(builder);
                }
                return result;
            }

            // - - - Layer aggregator
            // - -  RotationDegrees:183, Offset:<4.463, 1018.215>
            CanvasGeometry Geometry_1()
            {
                CanvasGeometry result;
                using (var builder = new CanvasPathBuilder(null))
                {
                    builder.BeginFigure(new Vector2(-156.199005F, 301.23999F));
                    builder.AddCubicBezier(new Vector2(-93.7190018F, 176.281006F), new Vector2(-63.8409996F, 81.6439972F), new Vector2(-189.669006F, 182.975006F));
                    builder.AddCubicBezier(new Vector2(-693.96698F, 589.091003F), new Vector2(113.801003F, -435.123993F), new Vector2(-553.388977F, 287.85199F));
                    builder.EndFigure(CanvasFigureLoop.Open);
                    result = CanvasGeometry.CreatePath(builder);
                }
                return result;
            }

            // - - Layer aggregator
            // -  RotationDegrees:183, Offset:<4.463, 1018.215>
            // Color
            ColorKeyFrameAnimation ColorAnimation_White_to_TransparentWhite()
            {
                // Frame 0.
                var result = CreateColorKeyFrameAnimation(0F, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), _stepThenHoldEasingFunction);
                // Frame 34.
                // White
                result.InsertKeyFrame(0.790697694F, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), _holdThenStepEasingFunction);
                // Frame 38.
                // TransparentWhite
                result.InsertKeyFrame(0.883720934F, Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), _c.CreateCubicBezierEasingFunction(new Vector2(0.166999996F, 0.166999996F), new Vector2(0.833000004F, 0.833000004F)));
                return result;
            }

            // - Layer aggregator
            // RotationDegrees:183, Offset:<4.463, 1018.215>
            CompositionColorBrush AnimatedColorBrush_White_to_TransparentWhite()
            {
                var result = _c.CreateColorBrush();
                StartProgressBoundAnimation(result, "Color", ColorAnimation_White_to_TransparentWhite(), _rootProgress);
                return result;
            }

            // - - Layer aggregator
            // ShapeGroup: 组 1 Offset:<270.253, 450.25>
            CompositionColorBrush ColorBrush_AlmostWhite_FFFFFEFF()
            {
                return _c.CreateColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFE, 0xFF));
            }

            // Layer aggregator
            CompositionContainerShape ContainerShape()
            {
                var result = _c.CreateContainerShape();
                result.CenterPoint = new Vector2(270.252991F, 450.25F);
                // ShapeGroup: 组 1 Offset:<270.253, 450.25>
                result.Shapes.Add(SpriteShape_0());
                StartProgressBoundAnimation(result, "RotationAngleInDegrees", RotationAngleInDegreesScalarAnimation_0_to_0(), RootProgress());
                StartProgressBoundAnimation(result, "Scale.X", ScaleXScalarAnimation_1_to_1p052(), _rootProgress);
                StartProgressBoundAnimation(result, "Scale.Y", ScaleYScalarAnimation_1_to_1(), _rootProgress);
                StartProgressBoundAnimation(result, "Offset", OffsetVector2Animation(), _rootProgress);
                return result;
            }

            // - - Layer aggregator
            // ShapeGroup: 组 1 Offset:<270.253, 450.25>
            CompositionPathGeometry PathGeometry_0()
            {
                return _c.CreatePathGeometry(new CompositionPath(Geometry_0()));
            }

            // - Layer aggregator
            // RotationDegrees:183, Offset:<4.463, 1018.215>
            CompositionPathGeometry PathGeometry_1()
            {
                var result = _c.CreatePathGeometry(new CompositionPath(Geometry_1()));
                StartProgressBoundAnimation(result, "TrimEnd", TrimEndScalarAnimation_0_to_1(), _rootProgress);
                return result;
            }

            // - Layer aggregator
            // ShapeGroup: 组 1 Offset:<270.253, 450.25>
            CompositionSpriteShape SpriteShape_0()
            {
                // Offset:<270.253, 450.25>
                var geometry = PathGeometry_0();
                var result = CreateSpriteShape(geometry, new Matrix3x2(1F, 0F, 0F, 1F, 270.252991F, 450.25F), ColorBrush_AlmostWhite_FFFFFEFF());
                return result;
            }

            // Layer aggregator
            // 路径 1
            CompositionSpriteShape SpriteShape_1()
            {
                // Offset:<4.463, 1018.215>, Rotation:-176.9999734298508 degrees, Scale:<1, 1>
                var result = CreateSpriteShape(PathGeometry_1(), new Matrix3x2(-0.99862957F, -0.0523358099F, 0.0523358099F, -0.99862957F, 4.46299982F, 1018.21503F));
                result.StrokeBrush = AnimatedColorBrush_White_to_TransparentWhite();
                result.StrokeDashCap = CompositionStrokeCap.Round;
                result.StrokeStartCap = CompositionStrokeCap.Round;
                result.StrokeEndCap = CompositionStrokeCap.Round;
                result.StrokeMiterLimit = 2F;
                result.StrokeThickness = 40F;
                return result;
            }

            // The root of the composition.
            ContainerVisual Root()
            {
                var result = _root = _c.CreateContainerVisual();
                var propertySet = result.Properties;
                propertySet.InsertScalar("Progress", 0F);
                propertySet.InsertScalar("t0", 0F);
                // Layer aggregator
                result.Children.InsertAtTop(ShapeVisual_0());
                StartProgressBoundAnimation(result.Properties, "t0", t0ScalarAnimation_0_to_1(), _rootProgress);
                return result;
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_0()
            {
                return _cubicBezierEasingFunction_0 = _c.CreateCubicBezierEasingFunction(new Vector2(0.333000004F, 0F), new Vector2(0F, 1F));
            }

            CubicBezierEasingFunction CubicBezierEasingFunction_1()
            {
                return _cubicBezierEasingFunction_1 = _c.CreateCubicBezierEasingFunction(new Vector2(0.421000004F, 0.00600000005F), new Vector2(0.833000004F, 1F));
            }

            ExpressionAnimation RootProgress()
            {
                var result = _rootProgress = _c.CreateExpressionAnimation("_.Progress");
                result.SetReferenceParameter("_", _root);
                return result;
            }

            // - Layer aggregator
            // Rotation
            ScalarKeyFrameAnimation RotationAngleInDegreesScalarAnimation_0_to_0()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, HoldThenStepEasingFunction());
                // Frame 10.
                result.InsertKeyFrame(0.232558146F, 41F, CubicBezierEasingFunction_0());
                // Frame 32.
                result.InsertKeyFrame(0.744186044F, 31F, _c.CreateCubicBezierEasingFunction(new Vector2(0.333000004F, 0F), new Vector2(0.833000004F, 1F)));
                // Frame 42.
                result.InsertKeyFrame(0.976744175F, 0F, _c.CreateCubicBezierEasingFunction(new Vector2(0.166999996F, 0F), new Vector2(0.833000004F, 1F)));
                return result;
            }

            // - Layer aggregator
            // Scale
            ScalarKeyFrameAnimation ScaleXScalarAnimation_1_to_1p052()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 1F, _holdThenStepEasingFunction);
                // Frame 10.
                result.InsertKeyFrame(0.232558146F, 0.63132F, _cubicBezierEasingFunction_0);
                // Frame 32.
                result.InsertKeyFrame(0.744186044F, 0.63132F, _c.CreateCubicBezierEasingFunction(new Vector2(0.305000007F, 0F), new Vector2(0.72299999F, 0.637000024F)));
                // Frame 42.
                result.InsertKeyFrame(0.976744175F, 1.05219996F, CubicBezierEasingFunction_1());
                return result;
            }

            // - Layer aggregator
            // Scale
            ScalarKeyFrameAnimation ScaleYScalarAnimation_1_to_1()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 1F, _holdThenStepEasingFunction);
                // Frame 10.
                result.InsertKeyFrame(0.232558146F, 0.600000024F, _cubicBezierEasingFunction_0);
                // Frame 32.
                result.InsertKeyFrame(0.744186044F, 0.600000024F, _c.CreateCubicBezierEasingFunction(new Vector2(0.305000007F, 0F), new Vector2(0.72299999F, 0.654999971F)));
                // Frame 42.
                result.InsertKeyFrame(0.976744175F, 1F, _cubicBezierEasingFunction_1);
                return result;
            }

            ScalarKeyFrameAnimation t0ScalarAnimation_0_to_1()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, _stepThenHoldEasingFunction);
                result.SetReferenceParameter("_", _root);
                // Frame 10.
                result.InsertKeyFrame(0.232558131F, 1F, _cubicBezierEasingFunction_0);
                return result;
            }

            // - - Layer aggregator
            // -  RotationDegrees:183, Offset:<4.463, 1018.215>
            // TrimEnd
            ScalarKeyFrameAnimation TrimEndScalarAnimation_0_to_1()
            {
                // Frame 0.
                var result = CreateScalarKeyFrameAnimation(0F, 0F, _stepThenHoldEasingFunction);
                // Frame 10.
                result.InsertKeyFrame(0.232558146F, 0F, _holdThenStepEasingFunction);
                // Frame 25.
                result.InsertKeyFrame(0.581395328F, 1F, _c.CreateCubicBezierEasingFunction(new Vector2(0.333000004F, 0F), new Vector2(0.666999996F, 1F)));
                return result;
            }

            // Layer aggregator
            ShapeVisual ShapeVisual_0()
            {
                var result = _c.CreateShapeVisual();
                result.Size = new Vector2(1080F, 1080F);
                var shapes = result.Shapes;
                shapes.Add(ContainerShape());
                // RotationDegrees:183, Offset:<4.463, 1018.215>
                shapes.Add(SpriteShape_1());
                return result;
            }

            StepEasingFunction HoldThenStepEasingFunction()
            {
                var result = _holdThenStepEasingFunction = _c.CreateStepEasingFunction();
                result.IsFinalStepSingleFrame = true;
                return result;
            }

            StepEasingFunction StepThenHoldEasingFunction()
            {
                var result = _stepThenHoldEasingFunction = _c.CreateStepEasingFunction();
                result.IsInitialStepSingleFrame = true;
                return result;
            }

            // - Layer aggregator
            // Offset
            Vector2KeyFrameAnimation OffsetVector2Animation()
            {
                // Frame 0.
                var result = CreateVector2KeyFrameAnimation(0F, new Vector2(269.747009F, 89.75F), _holdThenStepEasingFunction);
                result.SetReferenceParameter("_", _root);
                // Frame 10.
                result.InsertExpressionKeyFrame(0.232558131F, "Pow(1-_.t0,3)*Vector2(269.747,89.75)+(3*Square(1-_.t0)*_.t0*Vector2(236.58,90.417))+(3*(1-_.t0)*Square(_.t0)*Vector2(70.747,93.75))+(Pow(_.t0,3)*Vector2(70.747,93.75))", StepThenHoldEasingFunction());
                // Frame 10.
                result.InsertKeyFrame(0.232558146F, new Vector2(70.7470016F, 93.75F), _stepThenHoldEasingFunction);
                // Frame 14.
                result.InsertKeyFrame(0.325581402F, new Vector2(8.19499969F, 241.964005F), _c.CreateCubicBezierEasingFunction(new Vector2(0.335999995F, 0F), new Vector2(0.66900003F, 0.977999985F)));
                // Frame 16.
                result.InsertKeyFrame(0.372093022F, new Vector2(193.729004F, 147.602005F), _c.CreateCubicBezierEasingFunction(new Vector2(0.333999991F, 0.00899999961F), new Vector2(0.667999983F, 0.614000022F)));
                // Frame 17.
                result.InsertKeyFrame(0.395348847F, new Vector2(269.29599F, 166.404007F), _c.CreateCubicBezierEasingFunction(new Vector2(0.333999991F, 0.51700002F), new Vector2(0.666999996F, 0.977999985F)));
                // Frame 19.
                result.InsertKeyFrame(0.441860467F, new Vector2(176.843994F, 360.847992F), _c.CreateCubicBezierEasingFunction(new Vector2(0.333999991F, 0.0160000008F), new Vector2(0.666999996F, 0.984000027F)));
                // Frame 25.
                result.InsertKeyFrame(0.581395328F, new Vector2(433.471008F, 117.853996F), _c.CreateCubicBezierEasingFunction(new Vector2(0.335000008F, 0.0289999992F), new Vector2(0.667999983F, 0.953000009F)));
                // Frame 32.
                result.InsertKeyFrame(0.744186044F, new Vector2(412.747009F, 99.75F), _c.CreateCubicBezierEasingFunction(new Vector2(0.319999993F, 0.672999978F), new Vector2(0.635999978F, 1F)));
                // Frame 42.
                result.InsertKeyFrame(0.976744175F, new Vector2(269.747009F, 89.75F), _c.CreateCubicBezierEasingFunction(new Vector2(0.333000004F, 0F), new Vector2(0.633000016F, 1F)));
                return result;
            }

            internal Data_AnimatedVisual(
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
