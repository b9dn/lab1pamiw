using P04WeatherForecastAPI.Client.Models;
using P04WeatherForecastAPI.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace P04WeatherForecastAPI.Client {

    public partial class MainWindow : Window {
        AccuWeatherService accuWeatherService;
        public MainWindow() {
            InitializeComponent();
            accuWeatherService = new AccuWeatherService();
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e) {
            try {
                City[] cities = await accuWeatherService.GetLocations(txtCity.Text);

                // standardowy sposób dodawania elementów
                //lbData.Items.Clear();
                //foreach (var c in cities)
                //    lbData.Items.Add(c.LocalizedName);

                // teraz musimy skorzystac z bindowania danych bo chcemy w naszej kontrolce przechowywac takze id miasta 
                lbData.ItemsSource = cities;
            } catch {
                clearLabels();
                lblTemperatureValue.Content = "Unable to retrieve all information correctly";
            }
        }

        private async void lbData_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selectedCity = (City)lbData.SelectedItem;
            if (selectedCity != null) {
                clearLabels();
                try {
                    var weather = await accuWeatherService.GetCurrentConditions(selectedCity.Key);
                    lblCityName.Content = $"City: {selectedCity.LocalizedName}";
                    double tempValue = weather.Temperature.Metric.Value;
                    lblTemperatureValue.Content = $"Actual temperature: {Convert.ToString(tempValue)}";

                    var weather6ago = await accuWeatherService.GetHistoricalConditions6h(selectedCity.Key);
                    double tempValue6ago = weather6ago.Temperature.Metric.Value;
                    lblHistorical6hTemp.Content = $"Temperature 6h ago: {Convert.ToString(tempValue6ago)}";

                    var weather24ago = await accuWeatherService.GetHistoricalConditions24h(selectedCity.Key);
                    double tempValue24ago = weather24ago.Temperature.Metric.Value;
                    lblHistorical24hTemp.Content = $"Temperature 24h ago: {Convert.ToString(tempValue24ago)}";

                    City[] cities = await accuWeatherService.GetNearCities(selectedCity.Key);
                    if (cities != null && cities.Length >= 3) {
                        lblNearCities.Content = "Near cities: \n";
                        for (int i = 0; i < 3; i++) {
                            lblNearCities.Content += cities[i].LocalizedName + Environment.NewLine;
                        }
                    }

                    lblIndices.Content = "";
                    IndexValues index = await accuWeatherService.GetActivityIndex(selectedCity.Key,
                        AccuWeatherService.Activity.Tennis);
                    lblIndices.Content += index.Text + "\n";

                    index = await accuWeatherService.GetActivityIndex(selectedCity.Key,
                        AccuWeatherService.Activity.Swimming);
                    lblIndices.Content += index.Text + "\n";

                    index = await accuWeatherService.GetActivityIndex(selectedCity.Key,
                        AccuWeatherService.Activity.Running);
                    lblIndices.Content += index.Text;
                } catch {
                    clearLabels();
                    lblTemperatureValue.Content = "Unable to retrieve all information correctly";
                }
            }
        }
        private void clearLabels() {
            lblCityName.Content = "City: ";
            lblTemperatureValue.Content = "";
            lblHistorical6hTemp.Content = "";
            lblHistorical24hTemp.Content = "";
            lblNearCities.Content = "";
            lblIndices.Content = "";
        }
    }
}
