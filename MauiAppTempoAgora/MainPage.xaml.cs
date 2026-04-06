using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Text.Json;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        /* private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";

                        dados_previsao = $"Latitude: {t.lat} \n" +
                                         $"Longitude: {t.lon} \n" +
                                         $"Nascer do Sol: {t.sunrise} \n" +
                                         $"Por do Sol: {t.sunset} \n" +
                                         $"Temp Máx: {t.temp_max} \n" +
                                         $"Temp Min: {t.temp_min} \n" +
                                         $"Descrição: {t.description} \n" +
                                         $"Vel do vento: {t.speed} \n" +
                                         $"Visibilidade: {t.visibility} \n";

                        lbl_res.Text = dados_previsao;

                    }
                    else
                    {

                        lbl_res.Text = "Sem dados de Previsão";
                    }

                }
                else
                {
                    lbl_res.Text = "Preencha a cidade.";
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        } */

        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            using var client = new HttpClient();
            string url = $"https://api.exemplo.com/previsao?cidade={cidade}";

            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Erro na requisição", null, response.StatusCode);

            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Tempo>(json);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            try
            {
                // O próprio DataService deve lançar exceção se a cidade estiver vazia
                Tempo t = await DataService.GetPrevisao(txt_cidade.Text);

                lbl_res.Text =
                    $"Latitude: {t.lat}\n" +
                    $"Longitude: {t.lon}\n" +
                    $"Nascer do Sol: {t.sunrise}\n" +
                    $"Por do Sol: {t.sunset}\n" +
                    $"Temp Máx: {t.temp_max}\n" +
                    $"Temp Min: {t.temp_min}\n" +
                    $"Descrição: {t.description}\n" +
                    $"Vel do vento: {t.speed}\n" +
                    $"Visibilidade: {t.visibility}\n";
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await DisplayAlert("Cidade não encontrada",
                    "A cidade informada não foi localizada. Verifique o nome e tente novamente.",
                    "OK");
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == null)
            {
                await DisplayAlert("Sem conexão",
                    "Parece que você está sem internet. Verifique sua conexão e tente novamente.",
                    "OK");
            }
            catch (HttpRequestException httpEx)
            {
                await DisplayAlert("Erro na API",
                    $"O servidor retornou um erro: {httpEx.StatusCode}",
                    "OK");
            }
            catch (ArgumentException)
            {
                await DisplayAlert("Campo obrigatório",
                    "Informe o nome da cidade antes de consultar.",
                    "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }


    }

}
