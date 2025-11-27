using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CretaBase
{
    /// <summary>
    /// User control CretaTextBoxNumeric with buttons - and +
    /// Propperties: Value="{Binding tralari}" Increment="0.2" Unidades="m" UnidadesDelante="1:" Decimals="2" Min="-5" Max="25"
    /// (El minimo y el maximo no sirven, se cambia primero el valor en el origen y luego se comprueba, por lo que la propiedad de origen se hace un set con un valor incorrecto)
    /// </summary>
    public partial class CretaTextBoxNumericC4 : System.Windows.Controls.UserControl
    {
        #region Members
        private double _increment;
        private double _min;
        private double _max;
        private int _decimals;
        #endregion

        public CretaTextBoxNumericC4()
        {
            InitializeComponent();
            Increment = 0.1;
            Min = 0;
            Max = 50;
            Decimals = 2;
        }

        private void CretaTextBoxNumericC4_Loaded(object sender, RoutedEventArgs e)
        {
            ManageIsReadOnly();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the value assigned to the control.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set 
            {
                SetValue(ValueProperty, value);
            }
        }               
        public double Increment
        {
            get { return _increment; }
            set
            {
                _increment = value;
            }
        }

        public string Unidades { get; set; }
        public string UnidadesDelante { get; set; }
        public string UnidadesDetras { get; set; }
        public bool SmallSize { get; set; }
		public bool Vertical { get; set;}
        

        public bool VerticalSmall
        {
            get { return Vertical && SmallSize; }
        }
        public bool VerticalBig
        {
            get { return Vertical && !SmallSize; }
        }
        public bool HorizontalSmall
        {
            get { return !Vertical && SmallSize; }
        }
        public bool HorizontalBig
        {
            get { return !Vertical && !SmallSize; }
        }

        private static double _minSt;
        public double Min
        {
            get { return _min; }
            set
            {
                if (value != _min)
                {
                    _min = value;
                    _minSt = value;
                }
            }
        }

        private static double _maxSt;
        public double Max
        {
            get { return _max; }
            set
            {
                if (value != _max)
                {
                    _max = value;
                    _maxSt = value;
                }
            }
        }

        private static int _decimalsSt;
        public int Decimals
        {
            get { return _decimals; }
            set
            {
                if (value != _decimals)
                {
                    _decimals = value;
                    _decimalsSt = value;
                }
            }
        }
        #endregion

        #region Functions (and Dependency Propperties)
        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CretaTextBoxNumericC4),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnValueChanged), new CoerceValueCallback(CoerceValue)));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            CretaTextBoxNumericC4 control = (CretaTextBoxNumericC4)obj;

            RoutedPropertyChangedEventArgs<double> e = new RoutedPropertyChangedEventArgs<double>(
                (double)args.OldValue, (double)args.NewValue, ValueChangedEvent);
            control.OnValueChanged(e);
        }

        private static object CoerceValue(DependencyObject element, object value)
        {
            double newValue = (double)value;
            CretaTextBoxNumericC4 control = (CretaTextBoxNumericC4)element;
/*
            if (newValue > _maxSt)
                newValue = _maxSt;
            else if (newValue < _minSt)
                newValue = _minSt;
*/
            newValue = Math.Round(newValue, _decimalsSt);

            return newValue;
        }

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(CretaTextBoxNumericC4));

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> args)
        {
            if (!IsReadOnly)
                RaiseEvent(args);
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(CretaTextBoxNumericC4), new FrameworkPropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set 
            { 
                SetValue(IsReadOnlyProperty, value);
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            if (!IsReadOnly)
                Value += Increment;
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            if (!IsReadOnly)
                Value -= Increment;
        }
        #endregion

        public void ManageIsReadOnly()
        {
            Plus.IsEnabled = !IsReadOnly;
            Minus.IsEnabled = !IsReadOnly;
            TxtBox.IsReadOnly = IsReadOnly;

            PlusSm.IsEnabled = !IsReadOnly;
            MinusSm.IsEnabled = !IsReadOnly;
            TxtBoxSm.IsReadOnly = IsReadOnly;

            Plus1.IsEnabled = !IsReadOnly;
            Minus1.IsEnabled = !IsReadOnly;
            TxtBox1.IsReadOnly = IsReadOnly;

            Plus2.IsEnabled = !IsReadOnly;
            Minus2.IsEnabled = !IsReadOnly;
            TxtBox2.IsReadOnly = IsReadOnly;
        }
    }
}
