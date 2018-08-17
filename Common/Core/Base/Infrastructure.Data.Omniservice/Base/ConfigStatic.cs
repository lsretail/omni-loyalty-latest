using System;
using System.Reflection;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.Base
{
    public static class ConfigStatic
    {
        private static int defaultTimeoutInSec = 100;
        private static string defaultVersion = "2.4.0"; //default in case not set
        private static string defaultLang = "en"; //default in case not set

        //basic auth
        private static string defaultAuthUser = "LSOmniServiceUser";
        private static string defaultAuthPwd = "XYZ.LS@mniServiceUser.321";

        private static int timeout;
        private static string url;
        private static string uniqueDeviceId;
        private static string languageCode;
        private static string securityToken;
        private static string authUser;
        private static string authPassword;

        /// <summary>
        /// Gets or sets the URL. http://mobiledemo.lsretail.com/LSOmniService/json.svc
        /// </summary>
        /// <value>
        /// The URL. http://mobiledemo.lsretail.com/LSOmniService/json.svc
        /// </value>
        public static string Url
        {
            get
            {
                //get from storage (if possible), else return url 
                try
                {
                    url = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Url", "Url");
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                return url;
            }
            set
            {
                //save to storage before setting the static url
                try
                {
                    url = value;
                    // Helpers.Settings.GeneralSettings = "kk";
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Url", value);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                url = value;
            }
        }

        /// <summary>
        /// The UniqueDeviceId sent to LSOmniService.  
        /// </summary>
        public static string UniqueDeviceId
        {
            get
            {
                //get from storage (if possible), else return UniqueDeviceId 
                try
                {
                    uniqueDeviceId = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("UniqueDeviceId", "UniqueDeviceId");
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                return uniqueDeviceId;
            }
            set
            {
                //save to storage before setting the static UniqueDeviceId
                try
                {
                    uniqueDeviceId = value;
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("UniqueDeviceId", value);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                uniqueDeviceId = value;
            }
        }

        /// <summary>
        /// The LanguageCode sent to LSOmniService.  Defaults to  en
        /// </summary>
        public static string LanguageCode
        {
            get
            {
                //get from storage (if possible), else return LanguageCode 
                try
                {
                    languageCode = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("LanguageCode", defaultLang);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                    languageCode = (string.IsNullOrWhiteSpace(authUser) ? defaultLang : languageCode);
                }
                return languageCode;
            }
            set
            {
                //save to storage before setting the static LanguageCode
                try
                {
                    languageCode = value;
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("LanguageCode", value);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                languageCode = value;
            }
        }

        /// <summary>
        /// The version sent to LSOmniService. ex. 1.0.0
        /// </summary>
        public static string Version
        {
            get
            {
                try
                {
                    var assembly = typeof(ConfigStatic).GetTypeInfo().Assembly;
                    // In some PCL profiles the above line is: var assembly = typeof(MyType).Assembly;
                    AssemblyName assemblyName = new AssemblyName(assembly.FullName);
                    return assemblyName.Version.ToString();
                }
                catch
                {
                    return defaultVersion;
                }
            }
        }

        /// <summary>
        /// Timeout in seconds.  Defaults to 15 seconds
        /// </summary>
        public static int Timeout
        {
            get
            {
                //get from storage (if possible), else return Timeout 
                try
                {
                    timeout = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Timeout", defaultTimeoutInSec);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                    timeout = (0 == timeout ? defaultTimeoutInSec : timeout); // default to 15
                }
                return timeout;
            }
            set
            {
                //save to storage before setting the static Timeout
                try
                {
                    value = (value <= 0 ? defaultTimeoutInSec : value); //default to 15 sec
                    timeout = value;
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Timeout", value);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                timeout = value;
            }
        }

        /// <summary>
        /// Authorization user name. Used when basic authentication is set on IIS
        /// </summary>
        public static string AuthUser
        {
            get
            {
                //get from storage (if possible), else return authUser 
                try
                {
                    authUser = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("AuthUser", defaultAuthUser);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                    authUser = (string.IsNullOrWhiteSpace(authUser) ? defaultAuthUser : authUser);
                }
                return authUser;
            }
            set
            {
                //save to storage before setting the static AuthUser
                try
                {
                    authUser = value;
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("AuthUser", value);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                authUser = value;
            }
        }

        /// <summary>
        /// Authorization Password. Used when basic authentication is set on IIS
        /// </summary>
        public static string AuthPassword
        {
            get
            {
                //get from storage (if possible), else return authPassword 
                try
                {
                    authPassword = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("AuthPassword", defaultAuthPwd);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                    authPassword = defaultAuthPwd;
                }
                return authPassword;
            }
            set
            {
                //save to storage before setting the static authPassword
                try
                {
                    authPassword = value;
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("AuthPassword", value);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                authPassword = value;
            }
        }

        /// <summary>
        /// The version sent to LSOmniService. ex. 1.0.0
        /// </summary>
        public static string SecurityToken
        {
            get
            {
                //get from storage (if possible), else return SecurityToken 
                try
                {
                    string encryptedSaved = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("SecurityToken", "SecurityToken");

                    //TODO : Temp fix, need to resolve this so this will also work for iOS and Android
                    //securityToken = LSOmniServicePCL.Helpers.Security.Decrypt(saved);  //only works for Windows Phone?
                    securityToken = (string.IsNullOrEmpty(encryptedSaved) ? "" : encryptedSaved); //null not allowed
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                return securityToken;
            }
            set
            {
                //save to storage
                try
                {
                    securityToken = value;
                    //Refractored.Xam.Settings.CrossSettings.Current.AddOrUpdateValue("SecurityToken", LSOmniServicePCL.Helpers.Security.Encrypt(value));
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("SecurityToken", value);
                }
                catch (Exception ex)
                {
                    //not implemented for Windows desktop !
                    if (ex.GetType() != typeof(NotImplementedException))
                    {
                        throw;
                    }
                }
                securityToken = value;
            }
        }
    }
}
