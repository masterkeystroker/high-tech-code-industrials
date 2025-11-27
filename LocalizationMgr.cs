using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;
using System.Globalization;

namespace CretaBase
{
    static public class LocalizationMgr
    {
        /// <summary>
        /// Translation, gets a identifier and returns the translation according to the main language set
        ///  (i.e.: key "CretaBase:Strings:G_CLOSE" returns "Cerrar" if lang is setted to spanish or "Close" if english
        ///  LocalizationMgr.GetUIString("CretaBase:Strings:G_CLOSE")
        /// </summary>
        /// <param name="key">Text identifier</param>
        /// <returns>The string translated according to Language App</returns>
        static public string GetUIString(string key)
        {
            string uiString = "";
            try 
            {
                LocTextExtension locExtension = new LocTextExtension(key);
                locExtension.ResolveLocalizedValue(out uiString);
            }
            catch (Exception ex)
            {
                uiString = key;
                //Para coger solo el token
                string[] asUiString = key.Split(':');
                if (asUiString.Length >= 3)
                    uiString = asUiString[2];
                uiString = "#" + uiString + "#";
                //TODO: Enviar los identificadores no encontrados a un log
                string sMessage = "LOG_EXCEPTION;Error Base.LocalizationMgr.GetUIString: " + key + " - " + ex.Message + ex.StackTrace;
                CretaUtils.WriteLogEvent(sMessage);
            }
            return uiString;
        }

        static public void ChangeLanguage(string sLang)
        {
            LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(sLang);
        }
    }
}
