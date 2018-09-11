﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Refit;
using System.Net;
using Xamarin.Essentials;
using Plugin.Connectivity;
using System.Net.Http;

namespace ProQuant
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        bool busy = false;

        public static bool connected = false;


        public static Connection cnx = new Connection();


        public LoginPage()
        {
            InitializeComponent();
            ConnectionCheck();
        }

        public async void ConnectionCheck()
        {
            bool Connected = App.CheckConnection();
            if (Connected == false)
            {
                connected = false;
                await DisplayAlert("No Connection", "Internet Connection is needed for this app to function", "Ok, Exit App");
                App.ExitApp();
            }
            connected = true;
        }

        private void SignUpClicked(object sender, EventArgs e)
        {
            ConnectionCheck();
            if (connected == true)
            {
                if (busy == false)
                {
                    busy = true;
                    //add methods


                    busy = false;
                }
            }

            //potentially a pop up to ring us
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void ForgotPasswordClicked(object sender, EventArgs e)
        {
            ConnectionCheck();

            //pop up?
        }

        private async void LogInClicked(object sender, EventArgs e)
        {
            ConnectionCheck();
            if (connected == true)
            {
                if (busy == false)
                {
                    busy = true;
                    cnx.User = EmailEntry.Text;
                    cnx.Pass = PassEntry.Text;

                    string tokenKey = "/api/api/5?id=cmd$~gettoken";



                    //################################################################
                    //WHEN NOT TESTING COMMENT THESE LINES OUT AND REPLACE WITH BELOW
                    string user = "wayne_mcgrath1@hotmail.com"; //change to EmailEntry.Text                           //when not testing comment these lines out.
                    string pass = "proQuant97"; //change to PassEntry.Text
                    string response = await Client.GET_Token(tokenKey, "Basic", user, pass);
                    List<string> tokenInfo = GetTokenInfo(response).Result;
                    

                    //################################################################

                    //VV this works when not testing uncomment this and comment the user and other stuff

                    //string response = await Client.GET_Token(tokenKey, "Basic", cnx.User, cnx.Pass);
                    //List<string> tokenInfo = GetTokenInfo(response).Result;
                    
                    cnx.Token = tokenInfo[0];
                    cnx.ID = tokenInfo[1];
                    cnx.Name = tokenInfo[2];

                    if (tokenInfo[0] != "failed")
                    {
                        go();
                    }

                    busy = false;
                }
            }
        }


    async private void go()
        {
            ConnectionCheck();
            if (cnx != null)
            {
                await Navigation.PushAsync(new main(cnx)
                {
                    BarBackgroundColor = Color.FromHex("#fe0000"),
                    BarTextColor = Color.White
                });
            }
        }

        async Task<List<string>> GetTokenInfo(string response)
        {

            //Token Format = "\"Token=/MlLmCETCEuwmBs2GX6rkQ==~id=2534~name=EGB Builders \""

            Console.WriteLine("GOT RESPONSE");

            if (response.Contains("error unautherised user"))
            {
                Console.WriteLine("## BAD RESPONSE ---##--- UNAUTH USER ##");

                //display pop up
                await DisplayAlert("Login Failed", "Your Email or Password has not been recognised", "Ok");

                List<string> tokenInfoList = new List<string>()
                {
                    "failed",
                    "failed",
                    "failed"
                };
                return tokenInfoList;
            }
            else
            {
                Console.WriteLine("GOOD RESPONSE");
                List<string> tokenInfoList = TokenResponse.TokenParse(response);
                //0 - token
                //1 - id
                //2 - name
                return tokenInfoList;

            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


    }
}