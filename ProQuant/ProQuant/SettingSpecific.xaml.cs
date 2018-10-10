﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;

namespace ProQuant
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingSpecific : ContentPage
	{
        SettingsObject _setting;
        Connection Maincnx;

		public SettingSpecific (SettingsObject setting, Connection cnx)
		{
			InitializeComponent ();
            _setting = setting;
            Maincnx = cnx;
            Start();
		}

        public void Start()
        {
            Description.Text = _setting.description;
            OrigionalValue.Text = $"{_setting.uom} {_setting.original}";
            NewValue.Placeholder = _setting.value;
        }

        private void NewValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            double newValue;
            double max;
            double min;

            try
            {
                newValue = Convert.ToDouble(NewValue.Text);
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                max = Convert.ToDouble(_setting.max);
                min = Convert.ToDouble(_setting.min);
            }
            catch (Exception)
            {
                return;
            }
            if (newValue > max)
            {
                Warning.Text = $"Maximum allowed: {_setting.uom}{_setting.max}";
                return;
            }

            if (newValue < min)
            {
                Warning.Text = $"Minimum allowed: {_setting.uom}{_setting.min}";
                return;
            }

            if (newValue > min && newValue < max || string.IsNullOrWhiteSpace(NewValue.Text))
            {
                Warning.Text = "";
            }
        }

        private async void UpdateClicked(object sender, EventArgs e)
        {
            double newValue;
            double max;
            double min;

            try
            {
                newValue = Convert.ToDouble(NewValue.Text);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "There has been an error converting your number to a value, please check it is written correctly  \n\n Error Code \"SS####\"", "Ok");
                return;
            }

            try
            {
                max = Convert.ToDouble(_setting.max);
                min = Convert.ToDouble(_setting.min);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "There has been an error with our database value, please contact us and state:\n\n Error Code \"SS####\"", "Ok");
                return;
            }

            //do checks for max and min.
            if (newValue > max)
            {
                await DisplayAlert("Value Too Big", $"Sorry the value you have entered is too large.\n\n Our largest allowed value is:  {_setting.uom}{_setting.max} \n\nIf this wasn't a mistake, please contact us to change it.", "Ok");
                return;
            }

            if (newValue < min)
            {
                await DisplayAlert("Value Too Small", $"Sorry the value you have entered is too small.\n\n Our smallest allowed value is:  {_setting.uom}{_setting.min} \n\n If this wasn't a mistake, please contact us to change it.", "Ok");
                return;
            }
            _setting.value = NewValue.Text;
            Start();

            //send message to server
            PostSetting(Maincnx, _setting);

            //send update message to last page
            
        }

        private async void PostSetting(Connection maincnx, SettingsObject setting)
        {
            List<SettingsObject> OGSettingList = new List<SettingsObject>();
            
            OGSettingList.Add(setting);
            SettingsObject[] OGSettings = OGSettingList.ToArray();

            var OGSettingsJson = JsonConvert.SerializeObject(OGSettings, Formatting.None, new JsonSerializerSettings()
                                                                                          {
                                                                                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                                                          });
            var x = OGSettingsJson;







            MessagingCenter.Send<SettingSpecific, SettingsObject>(this, "send", _setting);
        }
    }
}