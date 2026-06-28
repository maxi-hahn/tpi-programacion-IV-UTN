using Application.Dtos.Request;
using Application.Dtos.Responses;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Application.Templates;
using Application.Constants;

namespace Infrastructure.Service
{
    public class AuthService : IAuthService
    {


        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _hasher;
        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IPasswordHasherService hasher, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _hasher = hasher;
            _emailService = emailService;
        }

        //Agregar validaciones de registro
        public async Task<AuthResponse?> SingUp(SingUpRequest request)
        {

            var baseUrl = _configuration["AppSettings:BaseUrl"];
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, patron))
            {
                throw new ValidationException(
                    "Invalid email format");
            }
            if (request.Password.Length < 8)
            {
                throw new ValidationException(
                    "Password must be at least 8 characters");
            }
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(c => c.Email == request.Email);
            if (existingUser != null)
            {
                throw new ConflictException(
                 "Invalid email format");
            }

            var hashedPassword = _hasher.Hash(request.Password);
            var verificationToken = Guid.NewGuid().ToString();
            var verificationExpiration = DateTime.UtcNow.AddHours(24);


            var newUser = new Client
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = hashedPassword,
                Dni = request.Dni,
                EmailVerified = false,
                VerificationToken = verificationToken,
                VerificationTokenExpiration = verificationExpiration
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            //MOdificar el link para que apunte a la ruta correcta en el frontend

            var verificationLink = $"{baseUrl}/api/clients/verify-email?token={verificationToken}";
            await _emailService.SendEmailAsync(
                 newUser.Email,
                 EmailSubjects.VerifyEmail,
                 EmailTemplates.VerifyAccount(
                     newUser.Name,
                     verificationLink)
                 );

            return new AuthResponse
            {
                Token = GenerarToken(
                    newUser.Id,
                    newUser.Email,
                    newUser.GetType().Name),
                Rol = newUser.GetType().Name,
                Id = newUser.Id,
                Email = newUser.Email
            };
        }
        public async Task<AuthResponse?> SingIn(SingInRequest request)
        {
            var cliente = await _context.Users
                .FirstOrDefaultAsync(c => c.Email == request.Email);

            if (cliente == null)
                throw new UnauthorizedException("Invalid email or password");

            if (!_hasher.Verify(request.Password, cliente.Password))
                throw new UnauthorizedException("Invalid email or password");

            return new AuthResponse
            {
                Token = GenerarToken(
                    cliente.Id,
                    cliente.Email,
                    cliente.GetType().Name),

                Rol = cliente.GetType().Name,
                Id = cliente.Id,
                Email = cliente.Email
            };
        }

        public async Task<bool> VerifyEmail(string token)
        {
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.VerificationToken == token);
            
            if (user == null)
                throw new NotFoundException("Invalid verification token");
            if (user.VerificationTokenExpiration == null ||
                user.VerificationTokenExpiration < DateTime.UtcNow)
            {
                throw new UnauthorizedException("Verification token expired");
            }
            user.EmailVerified = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiration = null;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResendVerificationEmail(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                throw new NotFoundException("Email not found");

            if (user.EmailVerified)
                throw new ConflictException("Email is already verified");

            var verificationToken = Guid.NewGuid().ToString();

            user.VerificationToken = verificationToken;
            user.VerificationTokenExpiration = DateTime.UtcNow.AddHours(24);

            await _context.SaveChangesAsync();

            var verificationLink =
                $"https://localhost:7001/api/clients/verify-email?token={verificationToken}";

            await _emailService.SendEmailAsync(
                user.Email,
                EmailSubjects.VerifyEmail,
                EmailTemplates.ResendVerification(
                    user.Name,
                    verificationLink)
            );

            return true;
        }

        private string GenerarToken(Guid userId, string email, string rol)
        {
            string key = _configuration["Jwt:Key"]!;
            string issuer = _configuration["Jwt:Issuer"]!;
            string audience = _configuration["Jwt:Audience"]!;
            int expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"]!);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GeneratePasswordResetToken(User user)
        {
            string key = _configuration["Jwt:Key"]!;
            string issuer = _configuration["Jwt:Issuer"]!;
            string audience = _configuration["Jwt:Audience"]!;

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    "purpose",
                    "reset-password")
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
        public async Task<bool> ForgotPassword(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                throw new NotFoundException("Email not found");

            var token = GeneratePasswordResetToken(user);

            var resetLink =
                $"https://localhost:7001/reset-password?token={token}";

            await _emailService.SendEmailAsync(
                user.Email,
                EmailSubjects.ResetPassword,
                EmailTemplates.ResetPassword(
                    user.Name,
                    resetLink)
            );

            return true;
        }

        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = _configuration["Jwt:Audience"],

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    _configuration["Jwt:Key"]!))
                    },
                    out SecurityToken validatedToken);

                var userId =
                    principal.FindFirst(
                        ClaimTypes.NameIdentifier)?.Value;

                var purpose =
                    principal.FindFirst("purpose")?.Value;

                if (purpose != "reset-password")
                    throw new UnauthorizedException("Invalid token");

                var user = await _context.Users
                    .FirstOrDefaultAsync(
                        u => u.Id == Guid.Parse(userId!));

                if (user == null)
                    throw new NotFoundException("User not found");

                user.Password =
                    _hasher.Hash(newPassword);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedException("The token has expired");
            }
            catch (SecurityTokenException)
            {
                throw new UnauthorizedException("Invalid token");
            }
        }
    }
}

