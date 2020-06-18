﻿using Covid19Radar.Common;
using Covid19Radar.Model;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.LocalNotification;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.ExposureNotifications;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    /// <summary>
    /// This service registers, retrieves, stores, and automatically updates user data.
    /// </summary>
    public class UserDataService
    {
        private readonly HttpDataService httpDataService;
        private UserDataModel current;
        public event EventHandler<UserDataModel> UserDataChanged;

        public UserDataService(HttpDataService httpDataService)
        {
            this.httpDataService = httpDataService;
            current = Get();
        }

        public bool IsExistUserData { get => current != null; }

        public async Task<UserDataModel> RegisterUserAsync()
        {
            var userData = await httpDataService.PostRegisterUserAsync();
            if (userData == null)
            {
                return null;
            }
            userData.StartDateTime = DateTime.UtcNow;
            await SetAsync(userData);
            return userData;
        }

        public UserDataModel Get()
        {
            if (Application.Current.Properties.ContainsKey("UserData"))
            {
                return Utils.DeserializeFromJson<UserDataModel>(Application.Current.Properties["UserData"].ToString());
            }
            return null;
        }

        public async Task SetAsync(UserDataModel userData)
        {
            var newdata = Utils.SerializeToJson(userData);
            var currentdata = Utils.SerializeToJson(current);
            if (currentdata.Equals(newdata))
            {
                return;
            }
            current = userData;
            Application.Current.Properties["UserData"] = Utils.SerializeToJson(current);
            await Application.Current.SavePropertiesAsync();

            UserDataChanged?.Invoke(this, current);
        }
    }

}
