namespace OrderService.BusinessLayer.Dtos
{
    public class UserDto
    {
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string PersonName { get; set; }
        public string Gender { get; set; }
    }
}
