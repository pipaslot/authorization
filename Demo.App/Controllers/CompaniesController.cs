using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Demo.App.Database.Entities;
using Demo.App.Database;
using Demo.App.Models;
using Demo.ModuleA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pipaslot.Authorization;
using Pipaslot.Authorization.Models;

namespace Demo.App.Controllers
{
    [Route("api/companies")]
    public class CompaniesController : Controller
    {
        private readonly IUser<long> _user;
        private readonly IOptions<JwtTokenOptions> _jwtOptions;
        private readonly CompanyRepository _companyRepository;
        private readonly IClaimsPrincipalProvider _claimsProvider;

        public CompaniesController(IUser<long> user, IOptions<JwtTokenOptions> jwtOptions, CompanyRepository companyRepository, IClaimsPrincipalProvider claimsProvider)
        {
            _user = user;
            _jwtOptions = jwtOptions;
            _companyRepository = companyRepository;
            _claimsProvider = claimsProvider;
        }

        [HttpGet("login-admin")]
        public string LoginAdmin()
        {
            var role = new TokenRole
            {
                Id = 1,
                Name = "Admin",
                Type = RoleType.Admin
            };
            return CreateNewToken(1, role);
        }

        [HttpGet("login-user")]
        public string LoginUser()
        {
            var role = new TokenRole
            {
                Id = 2,
                Name = "User",
                Type = RoleType.User,
            };
            return CreateNewToken(1, role);
        }

        private string CreateNewToken(long userId, TokenRole role)
        {
            var config = _jwtOptions.Value;
            var claims = new List<Claim>
            {
                _claimsProvider.RoleToClaim(role),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(config.ExpirationInMinutes);

            var token = new JwtSecurityToken(
                config.Issuer,
                config.Audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );
            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return "Bearer " + tokenValue;
        }

        private class TokenRole : IRole
        {
            public object Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public RoleType Type { get; set; }
        }

        [HttpGet]
        public async Task<ICollection<Company>> GetAllCompanies()
        {
            _user.CheckPermission(ModuleAPermission.View);
            return await _companyRepository.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<Company> GetCompany(int id)
        {
            _user.CheckPermission(ModuleAPermission.View, id);
            return await _companyRepository.Get(id);
        }

        [HttpGet("view")]
        public ActionResult GetView()
        {
            return View("Index");
        }

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
