namespace FitnessApp.Dto.Users
{
    public class UsersPageDTO : Pageable
    {
        public ICollection<UserDTO> Users { get; set; }
    }
}
