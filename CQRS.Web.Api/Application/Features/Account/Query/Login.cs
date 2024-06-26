using AutoWrapper.Wrappers;
using CQRS.Web.Api.Infrastructure.Data.Context;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace CQRS.Web.Api.Features.Account.Queries
{
    public class Login
    {
        public class Query : IRequest<ApiResponse>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be null");
                RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be null");
            }
        }
        public class Handler : IRequestHandler<Login.Query, ApiResponse>
        {
            private readonly IApplicationDbContext _context;

            public IConfiguration _configuration;
            public Handler(IApplicationDbContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<ApiResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    var password = BC.HashPassword(request.Password);
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email && x.isActive);
                    if (user == null)
                        return new ApiResponse("Username is not found", statusCode: 404);

                    bool verify = BC.Verify(request.Password, user.Password);

                    if (!verify)
                        throw new ApiException("Password doesn't match", statusCode: 401);

                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Name", user.Name),
                    new Claim("Email", user.Email),
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: signIn);

                    var generateToken = new JwtSecurityTokenHandler().WriteToken(token);

                    return new ApiResponse("Authentication Success", generateToken);
                }
                catch (System.Exception ex)
                {
                    throw new ApiException(ex.Message);
                }
            }
        }

    }
}
