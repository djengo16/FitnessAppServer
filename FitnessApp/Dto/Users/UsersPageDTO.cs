namespace FitnessApp.Dto.Users
{
    public class UsersPageDTO
    {
        public ICollection<UserDTO> Users { get; set; }
        public int PagesCount { get; set; }
    }
}
