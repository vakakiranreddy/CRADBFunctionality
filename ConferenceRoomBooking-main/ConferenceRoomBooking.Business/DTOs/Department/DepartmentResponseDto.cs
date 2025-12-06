namespace ConferenceRoomBooking.Business.DTOs.Department
{
    public class DepartmentResponseDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}








