using System;
using System.Collections.Generic;
using System.Text;

namespace FotogramClient
{
    public class Funcoes
    {
        readonly Windows.Storage.ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public void SaveOrUpdateOnLocalStorage(string key, object value)
        {
            _localSettings.Values[key] = value;
        }

        public object GetValuesOnLocalStorage(string key)
        {
            var value = _localSettings.Values.ContainsKey(key) ? _localSettings.Values[key] : null;
            return value;
        }

        public void SaveUserNameAndPassword(string username, string password)
        {
            _localSettings.Values["username"] = username;
            _localSettings.Values["password"] = password;
        }

        public bool HasUsernameAndPassword()
        {
            return _localSettings.Values.ContainsKey("username") && _localSettings.Values.ContainsKey("password");
        }

        public void RemoveUsernameAndPasswordFromLocalStorage()
        {
            _localSettings.Values.Remove("username");
            _localSettings.Values.Remove("password");
        }
    }
}
