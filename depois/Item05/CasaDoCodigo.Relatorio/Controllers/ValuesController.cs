using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CasaDoCodigo.RelatorioWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static readonly List<string> Relatorio = new List<string>()
        {
            "primeira linha do relatório",
            "segunda linha do relatório"
        };

        [HttpGet]
        public ActionResult<string> Get()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Relatorio)
            {
                sb.AppendLine(item);
            }
            return sb.ToString();
        }

        [Authorize]
        [HttpPost]
        public void PostAsync([FromBody] string value)
        {
            Relatorio.Add(new string('=', 100));
            Relatorio.Add("Criado por: " + User.FindFirst("name")?.Value);
            Relatorio.Add(new string('=', 100));
            Relatorio.Add(value);
        }
    }
}
