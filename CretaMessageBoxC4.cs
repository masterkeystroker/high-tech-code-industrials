using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace CretaBase
{
    public class CretaMessageBoxC4 : ViewModelBase
    {
        const int BUTTONS_SIZE_BIG = 100;
        const int BUTTONS_SIZE_SMALL = 70;
        const int BUTTONS_IMG_SIZE_BIG = 64;
        const int BUTTONS_IMG_SIZE_SMALL = 40;
        const int MIN_WIDTH_BIG = 600;
        const int MIN_WIDTH_SMALL = 400;
        const int MIN_HEIGHT_CONT_BIG = 130;
        const int MIN_HEIGHT_CONT_SMALL = 75;
        public string StrMessage { get; set; }
        public bool IsYesNoVisible { get; set; }
        public bool IsCancelVisible { get; set; }
        public bool IsOkVisible { get; set; }
        public bool IsWarningImgVisible { get; set; }
        public bool IsErrorImgVisible { get; set; }
        public int ButtonSize { get; set; }
        public int ButtonImgSize { get; set; }
        public int MinWidth { get; set; }
        public int MinHeightContainer { get; set; }

        #region CretaMessageBoxC4 Management
        private Window _window;
        private MessageBoxResult _msgBoxResult;
        private bool _bBgEffectApplied = false;
        
        /// <summary>
        /// Open a MessageBox that stops the execution
        /// </summary>
        /// <param name="sMessage">Message to show (with CretaBase:strings:TOKEN if translation needed)</param>
        /// <param name="msgBoxType">Type of buttons -> Ok - OkCancel - YesNo - YesNoCancel</param>
        /// <param name="bLocalize">If translation is needed</param>
        /// <param name="sTitle">Window title if needed</param>
        /// <param name="bSmallSize">Reduces the size of buttons, imgs and container</param>
        /// <returns>Returns the button pressed value (Yes,No,Cancel,Ok or None if the window is force closed)</returns>
        public MessageBoxResult Show(string sMessage, MessageBoxButton msgBoxType = MessageBoxButton.OK, bool bLocalize = true, string sTitle = "", 
                                                bool bSmallSize = false, MessageBoxImage msgBoxImg=MessageBoxImage.None)
        {
            ButtonSize = bSmallSize ? BUTTONS_SIZE_SMALL : BUTTONS_SIZE_BIG;
            ButtonImgSize = bSmallSize ? BUTTONS_IMG_SIZE_SMALL : BUTTONS_IMG_SIZE_BIG;
            MinWidth = bSmallSize ? MIN_WIDTH_SMALL : MIN_WIDTH_BIG;
            MinHeightContainer = bSmallSize ? MIN_HEIGHT_CONT_SMALL : MIN_HEIGHT_CONT_BIG;

            switch (msgBoxType)
            {
                case MessageBoxButton.OK:
                    IsOkVisible = true;
                    IsYesNoVisible = false;
                    IsCancelVisible = false;
                    break;
                case MessageBoxButton.OKCancel:
                    IsOkVisible = true;
                    IsYesNoVisible = false;
                    IsCancelVisible = true;
                    break;
                case MessageBoxButton.YesNo:
                    IsOkVisible = false;
                    IsYesNoVisible = true;
                    IsCancelVisible = false;
                    break;
                case MessageBoxButton.YesNoCancel:
                    IsOkVisible = false;
                    IsYesNoVisible = true;
                    IsCancelVisible = true;
                    break;
            }

            IsWarningImgVisible = false;
            IsErrorImgVisible = false;
            switch (msgBoxImg)
            {
                case MessageBoxImage.Warning:
                    IsWarningImgVisible = true;
                    break;
                case MessageBoxImage.Error:
                    IsErrorImgVisible = true;
                    break;
            }

            if (bLocalize)
                StrMessage = LocalizationMgr.GetUIString(sMessage);
            else
                StrMessage = sMessage;

            _msgBoxResult = MessageBoxResult.None;
            UserControl WinContent = new CretaMessageBoxCtrlC4();

            if (_window != null)
                CloseCretaMessageBox();

            _window = new Window
            {
                Title = sTitle,
                Content = WinContent,
                Width = Application.Current.MainWindow.Width,
                Height = Application.Current.MainWindow.Height,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                //ShowInTaskbar = false,
                Background = System.Windows.Media.Brushes.Transparent,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                //ShowInTaskbar = false,
            };

            if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                _window.WindowState = WindowState.Maximized;

            if (Application.Current.MainWindow.Effect == null)
            {
                var blur = new BlurEffect();
                blur.Radius = 5;
                Application.Current.MainWindow.Effect = blur;
                //if (Application.Current.MainWindow.Content.GetType().BaseType == WinContent.GetType().BaseType)
                {
                    Application.Current.MainWindow.Opacity = 0.75;
                }
                _bBgEffectApplied = true;
            }
            else
                _bBgEffectApplied = false;

            _window.DataContext = this;
            
            Nullable<bool> dialogResult = _window.ShowDialog();
            if (dialogResult != null)
                ClearCretaMessageBox();

            //Pasarle esta clase como datacontext y añadir los comandos de YES, NO, CANCEL y OK
            return _msgBoxResult;
        }
        /// <summary>
        /// Open a MessageBox that stops the execution and shows the Warning image
        /// </summary>
        /// <param name="sMessage">Message to show (with CretaBase:strings:TOKEN if translation needed)</param>
        /// <param name="msgBoxType">Type of buttons -> Ok - OkCancel - YesNo - YesNoCancel</param>
        /// <param name="bSmallSize">Reduces the size of buttons, imgs and container</param>
        /// <returns>Returns the button pressed value (Yes,No,Cancel,Ok or None if the window is force closed)</returns>
        public MessageBoxResult ShowWarning(string sMessage, MessageBoxButton msgBoxType, bool bSmallSize)
        {
            return Show(sMessage, msgBoxType, true, "", bSmallSize, MessageBoxImage.Warning);
        }
        /// <summary>
        /// Open a MessageBox that stops the execution and shows the Error image
        /// </summary>
        /// <param name="sMessage">Message to show (with CretaBase:strings:TOKEN if translation needed)</param>
        /// <param name="msgBoxType">Type of buttons -> Ok - OkCancel - YesNo - YesNoCancel</param>
        /// <param name="bSmallSize">Reduces the size of buttons, imgs and container</param>
        /// <returns>Returns the button pressed value (Yes,No,Cancel,Ok or None if the window is force closed)</returns>
        public MessageBoxResult ShowError(string sMessage, MessageBoxButton msgBoxType, bool bSmallSize)
        {
            return Show(sMessage, msgBoxType, true, "", bSmallSize, MessageBoxImage.Error);
        }
        /// <summary>
        /// Open a MessageBox that stops the execution
        /// </summary>
        /// <param name="sMessage">Message to show (with CretaBase:strings:TOKEN if translation needed)</param>
        /// <param name="msgBoxType">Type of buttons -> Ok - OkCancel - YesNo - YesNoCancel</param>
        /// <param name="bSmallSize">Reduces the size of buttons, imgs and container</param>
        /// <returns>Returns the button pressed value (Yes,No,Cancel,Ok or None if the window is force closed)</returns>
        public MessageBoxResult Show(string sMessage, MessageBoxButton msgBoxType, bool bSmallSize)
        {
            return Show(sMessage, msgBoxType, true, "", bSmallSize);
        }
        /// <summary>
        /// Open an "OK" MessageBox that stops the execution
        /// </summary>
        /// <param name="sMessage">Message to show (with CretaBase:strings:TOKEN if translation needed)</param>
        /// <param name="bSmallSize">Reduces the size of buttons, imgs and container</param>
        /// <returns>Returns the button pressed value (Yes,No,Cancel,Ok or None if the window is force closed)</returns>
        public MessageBoxResult Show(string sMessage, bool bSmallSize)
        {
            return Show(sMessage, MessageBoxButton.OK, true, "", bSmallSize);
        }

        private void ClearCretaMessageBox()
        {
            //Clears the effect applied in main window
            Window MainW = Application.Current.MainWindow;
            if (_bBgEffectApplied)
            {
                MainW.Effect = null;
                //if (Application.Current.MainWindow.Content.GetType().BaseType == _window.Content.GetType().BaseType)
                {
                    Application.Current.MainWindow.Opacity = 1.0;
                }
            }
            _window = null;
        }

        /// <summary>
        /// Close and Returns the window to normal state (clearing the effects)
        /// </summary>
        public void CloseCretaMessageBox()
        {
            if (_window != null)
            {
                //Closes the window that contents the userControl
                Window parentwin = Window.GetWindow(_window);
                if (parentwin != null)
                {
                    parentwin.Close();
                }
            }
        }
        #endregion

        #region CretaMsgBoxResultCommand
        RelayCommandWithParams _cretaMsgBoxResultCommand;
        public ICommand CretaMsgBoxResultCommand
        {
            get
            {
                if (_cretaMsgBoxResultCommand == null)
                {
                    _cretaMsgBoxResultCommand = new RelayCommandWithParams(p => CretaMsgBoxPressed(p), p => true);
                }
                return _cretaMsgBoxResultCommand;
            }
        }
        private void CretaMsgBoxPressed(object Result)
        {
            string res = (string)Result;
            if (res == "YES")
                _msgBoxResult = MessageBoxResult.Yes;
            else if (res == "NO")
                _msgBoxResult = MessageBoxResult.No;
            else if (res == "CANCEL")
                _msgBoxResult = MessageBoxResult.Cancel;
            else if (res == "OK")
                _msgBoxResult = MessageBoxResult.OK;
            else
                _msgBoxResult = MessageBoxResult.None;

            CloseCretaMessageBox();
        }
        #endregion
    }
}
