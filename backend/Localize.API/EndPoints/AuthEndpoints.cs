using Localize.API.Data;
using Localize.API.DTOs;
using Localize.API.Models;
using Localize.API.Services;
using Microsoft.EntityFrameworkCore; 

namespace Localize.API.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/register", async (RegisterUserRequest request, ApplicationDbContext db, AuthService authService) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return Results.BadRequest("Nome, e-mail e senha são obrigatórios.");
                }

                if (request.Password.Length < 6)
                {
                    return Results.BadRequest("A senha deve ter no mínimo 6 caracteres.");
                }

                var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return Results.Conflict("Já existe um usuário com este e-mail."); 
                }

                var passwordHash = authService.HashPassword(request.Password);

                var newUser = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = passwordHash
                };

                db.Users.Add(newUser);
                await db.SaveChangesAsync();

                var userResponse = new
                {
                    newUser.Id,
                    newUser.Name,
                    newUser.Email
                };

                return Results.Created($"/users/{newUser.Id}", userResponse); 
            })
            .WithName("RegisterUser")
            .WithOpenApi();
        }
    }
}