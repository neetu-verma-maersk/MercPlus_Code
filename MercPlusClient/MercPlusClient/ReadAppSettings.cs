using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace MercPlusClient
{
    public class ReadAppSettings
    {
       public static Dictionary<string,string> ReadAllSettings()
        {
           Dictionary<string, string> appSettingDic = new Dictionary<string, string>();  
            try
            {
                var appSettings = ConfigurationManager.AppSettings;                

                if (appSettings.Count == 0)
                {
                    return appSettingDic;
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        appSettingDic.Add(key, appSettings[key]);
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
            return appSettingDic;
        }

       public static string ReadSetting(string key)
        {
            try
            {
                return Properties.Settings.Default[key].ToString() ?? "Not Found";
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
        }

       public static bool AddUpdateAppSettings(string key, string value)
        {
            bool ret = false;
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                ret = true;
            }
            catch (ConfigurationErrorsException)
            {
               
            }
            return ret;
        }
    }
}