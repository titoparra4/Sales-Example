namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Services;
    using Sales.Common.Models;

    public class AddProductViewModel : BaseViewModel
    {
        #region Attributes
        private ApiService apiService;

        private bool isRunning;

        private bool isEnabled;
        #endregion

        #region Properties
        public string Description { get; set; }

        public string Price { get; set; }

        public string Remarks { get; set; }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.SetValue(ref this.isRunning, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.SetValue(ref this.isEnabled, value); }
        }
        #endregion

        #region Constructors
        public AddProductViewModel()
        {
            this.apiService = new ApiService();
            this.IsEnabled = true;
        }
        #endregion

        #region Commands
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Description))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error the product description", "Accept");
                return;
            }

            if (string.IsNullOrEmpty(this.Price))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error the product price", "Accept");
                return;
            }

            var price = decimal.Parse(this.Price);

            if (price < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error the price must be greater than zero", "Accept");
                return;
            }

            this.isRunning = true;
            this.isEnabled = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.isRunning = false;
                this.isEnabled = true;
                await Application.Current.MainPage.DisplayAlert("Error", connection.Message, "Accept");
                return;
            }

            var product = new Product
            {
                Description = this.Description,
                Price = price,
                Remarks = this.Remarks,
            };

            var url = Application.Current.Resources["UrlAPI"].ToString();
            var response = await this.apiService.Post(url, "/api", "/Products", product);

            if (!response.IsSuccess)
            {
                this.isRunning = false;
                this.isEnabled = true;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            this.isRunning = false;
            this.isEnabled = true;
            await Application.Current.MainPage.Navigation.PopAsync();

        }
        #endregion
    }
}
