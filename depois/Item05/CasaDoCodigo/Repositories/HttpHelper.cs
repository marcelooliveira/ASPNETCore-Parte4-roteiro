﻿using CasaDoCodigo.Areas.Identity.Data;
using CasaDoCodigo.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CasaDoCodigo
{
    //TAREFA 05: INJETAR UserManager PARA OBTER clienteId
    public class HttpHelper : IHttpHelper
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly UserManager<AppIdentityUser> userManager;

        public IConfiguration Configuration { get; }

        public HttpHelper(IHttpContextAccessor contextAccessor
            , IConfiguration configuration
            , UserManager<AppIdentityUser> userManager)
        {
            this.contextAccessor = contextAccessor;
            Configuration = configuration;
            this.userManager = userManager;
        }

        public int? GetPedidoId()
        {
            return contextAccessor.HttpContext.Session.GetInt32($"pedidoId_{GetClienteId()}");
        }

        public void SetPedidoId(int pedidoId)
        {
            contextAccessor.HttpContext.Session.SetInt32($"pedidoId_{GetClienteId()}", pedidoId);
        }

        public void ResetPedidoId()
        {
            contextAccessor.HttpContext.Session.Remove($"pedidoId_{GetClienteId()}");
        }

        private string GetClienteId()
        {
            var claimsPrincipal = contextAccessor.HttpContext.User;
            return userManager.GetUserId(claimsPrincipal);
        }

        public async Task<string> GetAccessToken(HttpClient client, string scope)
        {
            var response = await client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = Configuration["CasaDoCodigo.Identity"] + "/connect/token",
                    ClientId = "CasaDoCodigo.MVC",
                    ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0",
                    Scope = scope
                });

            return response.AccessToken ?? response.Error;
        }
    }
}
