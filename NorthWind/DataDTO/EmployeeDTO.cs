namespace NorthWind.DataDTO
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Photo { get; set; }
        public string Notes { get; set; }

        public EmployeeDTO(Models.Employee? employee)
        {
            
        }

        public EmployeeDTO(EmployeeDTO sourceEmployee)
        {
            EmployeeId = sourceEmployee.EmployeeId;
            LastName = sourceEmployee.LastName;
            FirstName = sourceEmployee.FirstName;
            BirthDate = sourceEmployee.BirthDate;
            Photo = sourceEmployee.Photo;
            Notes = sourceEmployee.Notes;
        }
    }
}
