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
    public static class PopUpMgr
    {
        private static Window _window;
        /// <summary>
        /// Open a new PopoUp with a UserControl as content, when close it returns a MessageBoxResult
        /// </summary>
        /// <param name="WindowContent">The user control to open</param>
        /// <param name="sTitle">The Window Title</param>
        /// <param name="iWidth">Width Size</param>
        /// <param name="iHeight">Height Size</param>
        /// 
        public static MessageBoxResult OpenCretaPopup(UserControl WindowContent, string sTitle = "", object pDataContext = null, 
            int iWidth = 1280, int iHeight = 1024, bool bShowStyle=false, bool bBlurBg=true, bool bShowDialog=true)
        {
            if (_window != null)
                CloseCretaPopup();

            _window = new Window
            {
                Title = sTitle,
                Content = WindowContent,
                Width = iWidth,
                Height = iHeight,
                ResizeMode = ResizeMode.NoResize,
                //WindowStyle = WindowStyle.None,
                //AllowsTransparency = true,
                //ShowInTaskbar = false,

                Background = System.Windows.Media.Brushes.Transparent,
                

                //ShowInTaskbar = false,
            };

            //vsa (28/07/2014) Cuando el popup se lanza durante el arranque del mainwindow salta una excepción ya que aún no está creado correctamente
            // esto es poco frecuente pero hay que prevenirlo.
            try
            {
                //Cambiado para centrar los popups en la pantalla principal
                _window.Owner = Application.Current.MainWindow;
                _window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            catch
            {
                _window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }               


            if (!bShowStyle)
            {
                _window.WindowStyle = WindowStyle.None;
                _window.AllowsTransparency = true;
            }

            if (bBlurBg)
            {
                var blur = new BlurEffect();
                blur.Radius = 5;
                Application.Current.MainWindow.Effect = blur;
                Application.Current.MainWindow.Opacity = 0.75;
            }

            if (pDataContext != null)
            {
                _window.DataContext = pDataContext;
            }
            else
            {
                _window.DataContext = Application.Current.MainWindow.DataContext;
            }

            Nullable<bool> dialogResult = null;
            if (bShowDialog)
                dialogResult = _window.ShowDialog();
            else
                _window.Show();

            if (dialogResult != null)
                ClearPopUp();

            return exitResult;
        }

        private static MessageBoxResult exitResult = MessageBoxResult.None;
        /// <summary>
        /// Close and Returns the window to normal state (clearing the effects) and pass an exit ressult as an argument
        /// </summary>
        public static void CloseCretaPopup(UserControl pContent = null, MessageBoxResult res=MessageBoxResult.None)
        {
            if (_window != null)
            {
                //Closes the window that contents the userControl
                Window parentwin = Window.GetWindow(_window);
                if (parentwin != null)
                {
                    parentwin.Close();
                }
                
                ClearPopUp();
            }

            exitResult = res;
        }

        private static void ClearPopUp()
        {
            if (_window != null)
            {
                //Clears the effect applied in main window
                Window MainW = Application.Current.MainWindow;
                MainW.Effect = null;
                Application.Current.MainWindow.Opacity = 1.0;

                _window = null;
            }
        }
    }
}
