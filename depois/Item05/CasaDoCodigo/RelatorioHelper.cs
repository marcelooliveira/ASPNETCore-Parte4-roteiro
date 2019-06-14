using CasaDoCodigo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CasaDoCodigo
{
    public interface IRelatorioHelper
    {
        Task GerarRelatorio(Pedido pedido);
    }

    public class RelatorioHelper : IRelatorioHelper
    {
        private const string RelatorioUri = "/api/values";
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly IHttpHelper httpHelper;

        public RelatorioHelper(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpHelper httpHelper)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.httpHelper = httpHelper;
        }

        public async Task GerarRelatorio(Pedido pedido)
        {
            using (HttpClient httpClient = httpClientFactory.CreateClient())
            {
                var accessToken = await httpHelper.GetAccessToken(httpClient, "CasaDoCodigo.Relatorio");
                httpClient.SetBearerToken(accessToken);

                string linhaRelatorio = await GetLinhaRelatorio(pedido);
                var json = JsonConvert.SerializeObject(linhaRelatorio);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                Uri baseUri = new Uri(configuration["CasaDoCodigo.RelatorioWebAPI"]);
                Uri uri = new Uri(baseUri, RelatorioUri);
                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(uri, httpContent);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new ApplicationException(httpResponseMessage.ReasonPhrase);
                }
            }
        }

        private async Task<string> GetLinhaRelatorio(Pedido pedido)
        {
            StringBuilder sb = new StringBuilder();
            string templatePedido =
                    await System.IO.File.ReadAllTextAsync("TemplatePedido.txt");

            string templateItemPedido =
                await System.IO.File.ReadAllTextAsync("TemplateItemPedido.txt");

            string linhaPedido =
                string.Format(templatePedido,
                    pedido.Id,
                    pedido.Cadastro.Nome,
                    pedido.Cadastro.Endereco,
                    pedido.Cadastro.Complemento,
                    pedido.Cadastro.Bairro,
                    pedido.Cadastro.Municipio,
                    pedido.Cadastro.UF,
                    pedido.Cadastro.Telefone,
                    pedido.Cadastro.Email,
                    pedido.Itens.Sum(i => i.Subtotal));

            sb.AppendLine(linhaPedido);

            foreach (var i in pedido.Itens)
            {
                string linhaItemPedido =
                    string.Format(
                        templateItemPedido,
                        i.Produto.Codigo,
                        i.PrecoUnitario,
                        i.Produto.Nome,
                        i.Quantidade,
                        i.Subtotal);

                sb.AppendLine(linhaItemPedido);
            }
            sb.AppendLine($@"=============================================");

            return sb.ToString();
        }
    }
}