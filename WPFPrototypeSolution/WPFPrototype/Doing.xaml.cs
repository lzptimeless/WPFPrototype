using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFPrototype
{
    /// <summary>
    /// Interaction logic for Doing.xaml
    /// </summary>
    public partial class Doing : UserControl
    {
        public Doing()
        {
            InitializeComponent();

            if (!WPFPrototype.Toolkit.Commons.View.IsDesignMode)
            {
                ((ScaleTransform)((TransformGroup)this.Circle.RenderTransform).Children[0]).ScaleX = 0;
                ((ScaleTransform)((TransformGroup)this.Circle.RenderTransform).Children[0]).ScaleY = 0;
                ((ScaleTransform)((TransformGroup)this.Arc0.RenderTransform).Children[1]).ScaleX = 0;
                ((ScaleTransform)((TransformGroup)this.Arc0.RenderTransform).Children[1]).ScaleY = 0;
                ((ScaleTransform)((TransformGroup)this.Arc1.RenderTransform).Children[1]).ScaleX = 0;
                ((ScaleTransform)((TransformGroup)this.Arc1.RenderTransform).Children[1]).ScaleY = 0;
                ((ScaleTransform)((TransformGroup)this.Arc2.RenderTransform).Children[1]).ScaleX = 0;
                ((ScaleTransform)((TransformGroup)this.Arc2.RenderTransform).Children[1]).ScaleY = 0;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation circleScaleX = new DoubleAnimation();
            Storyboard.SetTarget(circleScaleX, this.Circle);
            Storyboard.SetTargetProperty(circleScaleX, new PropertyPath("(0).(1)[0].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleXProperty));
            circleScaleX.To = 1;
            circleScaleX.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            circleScaleX.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            DoubleAnimation circleScaleY = new DoubleAnimation();
            Storyboard.SetTarget(circleScaleY, this.Circle);
            Storyboard.SetTargetProperty(circleScaleY, new PropertyPath("(0).(1)[0].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleYProperty));
            circleScaleY.To = 1;
            circleScaleY.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            circleScaleY.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(circleScaleX);
            sb.Children.Add(circleScaleY);

            DoubleAnimation arc0ScaleX = new DoubleAnimation();
            Storyboard.SetTarget(arc0ScaleX, this.Arc0);
            Storyboard.SetTargetProperty(arc0ScaleX, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleXProperty));
            arc0ScaleX.To = 1;
            arc0ScaleX.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc0ScaleX.BeginTime = TimeSpan.FromSeconds(0.15);
            arc0ScaleX.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            DoubleAnimation arc0ScaleY = new DoubleAnimation();
            Storyboard.SetTarget(arc0ScaleY, this.Arc0);
            Storyboard.SetTargetProperty(arc0ScaleY, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleYProperty));
            arc0ScaleY.To = 1;
            arc0ScaleY.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc0ScaleY.BeginTime = TimeSpan.FromSeconds(0.15);
            arc0ScaleY.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(arc0ScaleX);
            sb.Children.Add(arc0ScaleY);

            DoubleAnimation arc1ScaleX = new DoubleAnimation();
            Storyboard.SetTarget(arc1ScaleX, this.Arc1);
            Storyboard.SetTargetProperty(arc1ScaleX, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleXProperty));
            arc1ScaleX.To = 1;
            arc1ScaleX.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc1ScaleX.BeginTime = TimeSpan.FromSeconds(0.25);
            arc1ScaleX.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            DoubleAnimation arc1ScaleY = new DoubleAnimation();
            Storyboard.SetTarget(arc1ScaleY, this.Arc1);
            Storyboard.SetTargetProperty(arc1ScaleY, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleYProperty));
            arc1ScaleY.To = 1;
            arc1ScaleY.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc1ScaleY.BeginTime = TimeSpan.FromSeconds(0.25);
            arc1ScaleY.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(arc1ScaleX);
            sb.Children.Add(arc1ScaleY);

            DoubleAnimation arc2ScaleX = new DoubleAnimation();
            Storyboard.SetTarget(arc2ScaleX, this.Arc2);
            Storyboard.SetTargetProperty(arc2ScaleX, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleXProperty));
            arc2ScaleX.To = 1;
            arc2ScaleX.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc2ScaleX.BeginTime = TimeSpan.FromSeconds(0.35);
            arc2ScaleX.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            DoubleAnimation arc2ScaleY = new DoubleAnimation();
            Storyboard.SetTarget(arc2ScaleY, this.Arc2);
            Storyboard.SetTargetProperty(arc2ScaleY, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleYProperty));
            arc2ScaleY.To = 1;
            arc2ScaleY.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc2ScaleY.BeginTime = TimeSpan.FromSeconds(0.35);
            arc2ScaleY.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(arc2ScaleX);
            sb.Children.Add(arc2ScaleY);

            DoubleAnimation angleDa = new DoubleAnimation();
            Storyboard.SetTarget(sb, this.Root);
            Storyboard.SetTargetProperty(sb, new PropertyPath("(0).(1)[0].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                RotateTransform.AngleProperty));
            angleDa.To = ((RotateTransform)((TransformGroup)this.Root.RenderTransform).Children[0]).Angle + 360;
            angleDa.Duration = new Duration(TimeSpan.FromSeconds(2));
            angleDa.RepeatBehavior = RepeatBehavior.Forever;

            sb.Children.Add(angleDa);
            sb.FillBehavior = FillBehavior.HoldEnd;

            this.Sprite().Start(sb);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation circleScaleX = new DoubleAnimation();
            Storyboard.SetTarget(circleScaleX, this.Circle);
            Storyboard.SetTargetProperty(circleScaleX, new PropertyPath("(0).(1)[0].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleXProperty));
            circleScaleX.To = 0;
            circleScaleX.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            circleScaleX.BeginTime = TimeSpan.FromSeconds(0.35);
            circleScaleX.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            DoubleAnimation circleScaleY = new DoubleAnimation();
            Storyboard.SetTarget(circleScaleY, this.Circle);
            Storyboard.SetTargetProperty(circleScaleY, new PropertyPath("(0).(1)[0].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleYProperty));
            circleScaleY.To = 0;
            circleScaleY.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            circleScaleY.BeginTime = TimeSpan.FromSeconds(0.35);
            circleScaleY.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(circleScaleX);
            sb.Children.Add(circleScaleY);

            DoubleAnimation arc0ScaleX = new DoubleAnimation();
            Storyboard.SetTarget(arc0ScaleX, this.Arc0);
            Storyboard.SetTargetProperty(arc0ScaleX, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleXProperty));
            arc0ScaleX.To = 0;
            arc0ScaleX.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc0ScaleX.BeginTime = TimeSpan.FromSeconds(0.2);
            arc0ScaleX.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            DoubleAnimation arc0ScaleY = new DoubleAnimation();
            Storyboard.SetTarget(arc0ScaleY, this.Arc0);
            Storyboard.SetTargetProperty(arc0ScaleY, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleYProperty));
            arc0ScaleY.To = 0;
            arc0ScaleY.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc0ScaleY.BeginTime = TimeSpan.FromSeconds(0.2);
            arc0ScaleY.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(arc0ScaleX);
            sb.Children.Add(arc0ScaleY);

            DoubleAnimation arc1ScaleX = new DoubleAnimation();
            Storyboard.SetTarget(arc1ScaleX, this.Arc1);
            Storyboard.SetTargetProperty(arc1ScaleX, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleXProperty));
            arc1ScaleX.To = 0;
            arc1ScaleX.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc1ScaleX.BeginTime = TimeSpan.FromSeconds(0.1);
            arc1ScaleX.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            DoubleAnimation arc1ScaleY = new DoubleAnimation();
            Storyboard.SetTarget(arc1ScaleY, this.Arc1);
            Storyboard.SetTargetProperty(arc1ScaleY, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleYProperty));
            arc1ScaleY.To = 0;
            arc1ScaleY.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc1ScaleY.BeginTime = TimeSpan.FromSeconds(0.1);
            arc1ScaleY.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(arc1ScaleX);
            sb.Children.Add(arc1ScaleY);

            DoubleAnimation arc2ScaleX = new DoubleAnimation();
            Storyboard.SetTarget(arc2ScaleX, this.Arc2);
            Storyboard.SetTargetProperty(arc2ScaleX, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleXProperty));
            arc2ScaleX.To = 0;
            arc2ScaleX.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc2ScaleX.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            DoubleAnimation arc2ScaleY = new DoubleAnimation();
            Storyboard.SetTarget(arc2ScaleY, this.Arc2);
            Storyboard.SetTargetProperty(arc2ScaleY, new PropertyPath("(0).(1)[1].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                ScaleTransform.ScaleYProperty));
            arc2ScaleY.To = 0;
            arc2ScaleY.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            arc2ScaleY.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseInOut };

            sb.Children.Add(arc2ScaleX);
            sb.Children.Add(arc2ScaleY);

            DoubleAnimation angleDa = new DoubleAnimation();
            Storyboard.SetTarget(sb, this.Root);
            Storyboard.SetTargetProperty(sb, new PropertyPath("(0).(1)[0].(2)",
                FrameworkElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                RotateTransform.AngleProperty));
            angleDa.To = ((RotateTransform)((TransformGroup)this.Root.RenderTransform).Children[0]).Angle + 360;
            angleDa.Duration = new Duration(TimeSpan.FromSeconds(2));

            sb.Children.Add(angleDa);
            sb.FillBehavior = FillBehavior.HoldEnd;

            this.Sprite().Start(sb);
        }
    }
}
