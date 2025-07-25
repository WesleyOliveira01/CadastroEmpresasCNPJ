using Localize.API.Data;
using Localize.API.DTOs;
using Localize.API.Models;
using Localize.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Localize.API.Endpoints
{
    public static class CompanyEndpoints
    {
        public static void MapCompanyEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/companies", [Authorize] async (RegisterCompanyRequest request, ApplicationDbContext db, ReceitaWsService receitaWsService, ClaimsPrincipal user) =>
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Results.Unauthorized();
                }

                if (string.IsNullOrWhiteSpace(request.Cnpj) || request.Cnpj.Length != 14 || !request.Cnpj.All(char.IsDigit))
                {
                    return Results.BadRequest("CNPJ inválido. Deve conter 14 dígitos numéricos.");
                }

                var existingCompany = await db.Companies.FirstOrDefaultAsync(c => c.Cnpj == request.Cnpj);
                if (existingCompany != null)
                {
                    return Results.Conflict("Empresa com este CNPJ já cadastrada.");
                }

                var cnpjData = await receitaWsService.GetCompanyDataByCnpjAsync(request.Cnpj);

                if (cnpjData == null || cnpjData.Status?.ToUpper() == "ERROR" || cnpjData.Cnpj == null)
                {
                    return Results.BadRequest($"Não foi possível obter dados para o CNPJ: {request.Cnpj}. Mensagem da API: {cnpjData?.Message ?? "Erro desconhecido."}");
                }

                var newCompany = new Company
                {
                    Cnpj = cnpjData.Cnpj,
                    NomeEmpresarial = cnpjData.NomeEmpresarial,
                    NomeFantasia = cnpjData.NomeFantasia,
                    Situacao = cnpjData.Situacao,
                    Abertura = cnpjData.Abertura,
                    Tipo = cnpjData.Tipo,
                    NaturezaJuridica = cnpjData.NaturezaJuridica,
                    AtividadePrincipal = cnpjData.AtividadePrincipal?.FirstOrDefault()?.Text ?? string.Empty,
                    Logradouro = cnpjData.Logradouro,
                    Numero = cnpjData.Numero,
                    Complemento = cnpjData.Complemento,
                    Bairro = cnpjData.Bairro,
                    Municipio = cnpjData.Municipio,
                    Uf = cnpjData.Uf,
                    Cep = cnpjData.Cep,
                    UserId = userId
                };

                db.Companies.Add(newCompany);
                await db.SaveChangesAsync();

                return Results.Created($"/companies/{newCompany.Cnpj}", new
                {
                    newCompany.Cnpj,
                    newCompany.NomeEmpresarial,
                    newCompany.NomeFantasia,
                    newCompany.UserId
                });
            })
            .WithName("RegisterCompany")
            .WithOpenApi();
        }
    }
}