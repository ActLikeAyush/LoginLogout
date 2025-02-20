namespace TestAPIadminPortal.Models.Entites
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }
        public string TypeOfUser { get; set; }

        public decimal Salary { get; set; }
    }
}
