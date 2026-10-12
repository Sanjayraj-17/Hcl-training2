
using Employee_Management.Data;
using Employee_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/employees?department=IT
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
            [FromQuery] string? department)
        {
            var query = _context.Employees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(
                    e => e.Department == department.Trim());
            }

            return Ok(await query.OrderBy(e => e.Id).ToListAsync());
        }

        // GET: api/employees/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(
            Employee employee)
        {
            employee.Id = 0;
            employee.FullName = employee.FullName.Trim();
            employee.Email = employee.Email.Trim();
            employee.Department = employee.Department.Trim();
            employee.JobTitle = employee.JobTitle.Trim();

            _context.Employees.Add(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.GetBaseException() is SqlException sql &&
                      (sql.Number == 2601 || sql.Number == 2627))
            {
                return Conflict(new
                {
                    message = "An employee with this email already exists."
                });
            }

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = employee.Id },
                employee);
        }

        // PUT: api/employees/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(
            int id, Employee input)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            input.FullName = input.FullName.Trim();
            input.Email = input.Email.Trim();
            input.Department = input.Department.Trim();
            input.JobTitle = input.JobTitle.Trim();

            employee.FullName = input.FullName;
            employee.Email = input.Email;
            employee.Department = input.Department;
            employee.JobTitle = input.JobTitle;
            employee.Salary = input.Salary;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
                when (ex.GetBaseException() is SqlException sql &&
                      (sql.Number == 2601 || sql.Number == 2627))
            {
                return Conflict(new
                {
                    message = "An employee with this email already exists."
                });
            }

            return Ok(employee);
        }

        // DELETE: api/employees/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}