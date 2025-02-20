using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestAPIadminPortal.Data;
using TestAPIadminPortal.Models;
using TestAPIadminPortal.Models.Entites;

namespace TestAPIadminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private ApplicationDbContext dbcontext;

        private IConfiguration configuration;
        public EmployeesController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            this.dbcontext = dbContext;
            this.configuration = configuration;
        }

        [HttpGet("AllUserDetails")]
        public IActionResult showAllUser()
        {
            var allUser = new
            {
                AllName = dbcontext.Employees.Select(e => e.Name).ToList(),
                AllEmail = dbcontext.Employees.Select(e => e.Email).ToList(),
                AllPasswords = dbcontext.Employees.Select(e => e.Password).ToList()
        };
           
            return Ok(allUser);
        }


        [HttpPost("LoginPage")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            var user = dbcontext.Employees.FirstOrDefault(x => x.Email == loginDTO.Email && x.Password == loginDTO.Password);

            var skey = 0;
            if (user != null)
            {
                var userType = user.TypeOfUser;
                if (userType.Equals( "A")) {
                    
                    Console.WriteLine(" A");
                }
                else if(userType.Equals("B"))
                {
                    Console.WriteLine("B");
                    skey = 1112;
                }
                else
                {
                    Console.WriteLine("error..........");
                }
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("UserEmail", user.Email.ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:key"]));

                var SignIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var Token = new JwtSecurityToken(
                    configuration["Jwt:Issuer"],
                    configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMilliseconds(100),
                    signingCredentials: SignIn
                    );
                string tokenValue = new JwtSecurityTokenHandler().WriteToken(Token);

                return Ok(new { Token = tokenValue, User = user,skey});
            }
            return NoContent();
        }







        // New method to allow access for a user by their token
        [Authorize]
        [HttpGet("user")]
        public IActionResult GetUserFromToken()
        {
            try
            {
                // Get the token from the Authorization header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Validate the token
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:key"]);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = handler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                // Extract the user ID from the claims
                var userIdClaim = principal.FindFirst("UserId")?.Value;

                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    var user = dbcontext.Employees.FirstOrDefault(e => e.Id == userId);
                    if (user != null)
                    {
                        return Ok(user);
                    }
                }
                return Unauthorized("Invalid token or user not found.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error validating token: {ex.Message}");
            }
        }








        [Authorize]
        [HttpGet]
        public IActionResult GetAllEmployee()
        {
            var allEmployee = dbcontext.Employees.ToList();
            return Ok(allEmployee);
        }






        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetEmployeeById(Guid id)
        {
            var empDetails = dbcontext.Employees.Find(id);
            if (empDetails is null)
            {
                return NotFound();
            }
            return Ok(empDetails);
        }





        [HttpPut("edit")]
        public IActionResult UpdateEmployee(Guid id, Employee emp)
        {
            //var updEmp = new Employee();
            var employee = dbcontext.Employees.Find(id);
            if (employee is null)
            {
                return NotFound();
            }
            employee.Name = emp.Name;
            employee.Phone = emp.Phone;
            employee.Salary = emp.Salary;

            dbcontext.SaveChanges();
            return Ok(employee);
        
        }


        //[Authorize]
        //[HttpPut("editByToken")]
        //public IActionResult UpdateUser(string token,Employee emp) 
        //{
        //    var employeeEntity = new Employee();
        //    if(emp is null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok();
        //}


        [HttpPost("add")]
        public IActionResult AddEmployee(EmployeeDataClass EDC)
        {
            var employeeEntity = new Employee()
            {
                Name = EDC.Name,
                Email = EDC.Email,
                Phone = EDC.Phone,
                Password = EDC.Password,
                Salary = EDC.Salary
            };

            dbcontext.Employees.Add(employeeEntity);
            dbcontext.SaveChanges();
            return Ok(employeeEntity);
        }



        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteEmployeeById(Guid id)
        {
            var employee = dbcontext.Employees.Find(id);
            if (employee is null)
            {
                return NotFound();
            }
            dbcontext.Employees.Remove(employee);
            dbcontext.SaveChanges();
            return Ok(employee.Name + "this employee is removed");
        }
    }
}
