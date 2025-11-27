using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CretaBase
{
    static public class CretaProperties
    {
        //Aquí voy a ir poniendo propiedades que luego voy a utilizar en las modificaciones de los controles
        #region Property ButtonBlinkRed
        public static readonly DependencyProperty BlinkRedProperty = DependencyProperty.RegisterAttached("BlinkRed", typeof(bool), typeof(CretaProperties), new FrameworkPropertyMetadata(false));

        public static bool GetBlinkRed(DependencyObject d)
        {
            return (bool)d.GetValue(BlinkRedProperty);
        }
        public static void SetBlinkRed(DependencyObject d, bool value)
        {
            d.SetValue(BlinkRedProperty, value);
        }
        #endregion
        #region Property ButtonBlinkYellow
        public static readonly DependencyProperty BlinkYellowProperty = DependencyProperty.RegisterAttached("BlinkYellow", typeof(bool), typeof(CretaProperties), new FrameworkPropertyMetadata(false));

        public static bool GetBlinkYellow(DependencyObject d)
        {
            return (bool)d.GetValue(BlinkYellowProperty);
        }
        public static void SetBlinkYellow(DependencyObject d, bool value)
        {
            d.SetValue(BlinkYellowProperty, value);
        }
        #endregion

        #region Property IsButtonSelected
        public static readonly DependencyProperty IsButtonSelectedProperty = DependencyProperty.RegisterAttached("IsButtonSelected", typeof(bool), typeof(CretaProperties), new FrameworkPropertyMetadata(false));

        public static bool GetIsButtonSelected(DependencyObject d)
        {
            return (bool)d.GetValue(IsButtonSelectedProperty);
        }
        public static void SetIsButtonSelected(DependencyObject d, bool value)
        {
            d.SetValue(IsButtonSelectedProperty, value);
        }
        #endregion

        #region Property TextInfo
        public static readonly DependencyProperty TextInfoProperty = DependencyProperty.RegisterAttached("TextInfo", typeof(String), typeof(CretaProperties), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender));

        public static string GetTextInfo(UIElement element)
        {
            return (string)element.GetValue(TextInfoProperty);
        }
        public static void SetTextInfo(UIElement element, string value)
        {
            element.SetValue(TextInfoProperty, value);
        }
        #endregion
        #region Property Content1
        public static readonly DependencyProperty Content1Property = DependencyProperty.RegisterAttached("Content1", typeof(object), typeof(CretaProperties), new UIPropertyMetadata(null));

        public static object GetContent1(UIElement element)
        {
            return (object)element.GetValue(Content1Property);
        }
        public static void SetContent1(UIElement element, object value)
        {
            element.SetValue(Content1Property, value);
        }
        #endregion
        #region Property Content2
        public static readonly DependencyProperty Content2Property = DependencyProperty.RegisterAttached("Content2", typeof(object), typeof(CretaProperties), new UIPropertyMetadata(null));

        public static object GetContent2(UIElement element)
        {
            return (object)element.GetValue(Content2Property);
        }
        public static void SetContent2(UIElement element, object value)
        {
            element.SetValue(Content2Property, value);
        }
        #endregion

        #region Property Scale
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.RegisterAttached("Scale", typeof(double), typeof(CretaProperties), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static double GetScale(UIElement element)
        {
            return (double)element.GetValue(ScaleProperty);
        }
        public static void SetScale(UIElement element, double value)
        {
            element.SetValue(ScaleProperty, value);
        }
        #endregion
    }
}
