using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Test.Data;
using Test.Models;
using Test.Options;

namespace Test.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;


        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters, DataContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _roleManager = roleManager;
        }


        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var findByEmailAsync = await _userManager.FindByEmailAsync(email);
            if (findByEmailAsync != null)
            {
                return new AuthenticationResult
                {
                    Errors = new string[] { "User with this email adresse is already exist" }
                };

            }
            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };
            var created = await _userManager.CreateAsync(newUser, password);
            
            //await _userManager.AddClaimsAsync(newUser, new []{ new Claim("users.view", "true") });
            await _userManager.AddToRoleAsync(newUser, "Poster");
            if (!created.Succeeded)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = created.Errors.Select(x => x.Description)
                };
            }
            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            try
            {
                var findByEmailAsync = await _userManager.FindByEmailAsync(email);
                if (findByEmailAsync == null)
                {
                    return new AuthenticationResult
                    {
                        Errors = new string[] { "User does not exist" }
                    };

                }

                var user = findByEmailAsync;
                var userValidPassword = await _userManager.CheckPasswordAsync(user, password);
                if (!userValidPassword)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "Verify your Email adresse or  password " }
                    };
                }

                return await GenerateAuthenticationResultForUserAsync(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            try
            {
                var validatedToken = GetPrincipalFromToken(token);

                if (validatedToken == null)
                {
                    return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
                }

                var expiryDateUnix =
                    long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(expiryDateUnix);

                if (expiryDateTimeUtc > DateTime.UtcNow)
                {
                    return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
                }

                var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

                if (storedRefreshToken == null)
                {
                    return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
                }

                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
                }

                if (storedRefreshToken.Invalidated)
                {
                    return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
                }

                if (storedRefreshToken.Used)
                {
                    return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
                }

                if (storedRefreshToken.JwtId != jti)
                {
                    return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
                }

                storedRefreshToken.Used = true;
                _context.RefreshTokens.Update(storedRefreshToken);
                await _context.SaveChangesAsync();

                var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type.ToLower() == "id").Value);
                return  await GenerateAuthenticationResultForUserAsync(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<IdentityUser>> GetUsersAsync()
        {
            return  await _userManager.Users.ToListAsync();
        }


        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

    }
}