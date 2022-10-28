namespace FitnessApp.Services.Data
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using FitnessApp.Common;
    using FitnessApp.Dto.Users;
    using FitnessApp.Models;
    using FitnessApp.Models.Repositories;
    using FitnessApp.Services.ServiceConstants;
    using Microsoft.AspNetCore.Identity;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        public UsersService(
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            this.usersRepository = usersRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        public async Task HardDeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            usersRepository.HardDelete(user);
            await usersRepository.SaveChangesAsync();
        }

        public async Task<UserDetailsDTO> GetUserByIdAsync(string id)
        {
            var user =  await userManager.FindByIdAsync(id);

            if(user == null)
            {
                throw new ArgumentException(ErrorMessages.UserNotFound);
            }

            return mapper.Map<UserDetailsDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync(string searchParams, int? take = null, int skip = 0)
        {
            var usersQueryModel = usersRepository
                .All().Where(x => !string.IsNullOrEmpty(searchParams) 
                ? 
                 (x.Email.Contains(searchParams) 
                    || x.FirstName.Contains(searchParams) 
                    || x.LastName.Contains(searchParams))
                : 
                true);

            if (usersQueryModel.Count() < take)
            {
                take = usersQueryModel.Count();
            }

            usersQueryModel = 
                take.HasValue? usersQueryModel.Skip(skip).Take(take.Value) : usersQueryModel.Skip(skip);

            var users =  usersQueryModel.ProjectTo<UserDTO>(mapper.ConfigurationProvider).ToList();

            foreach(var user in users)
            {
                user.Role = await this.GetRoleNameAsync(user.Id);
            }

            return users;
        }

        public int GetCount()

        {
            return this.usersRepository.All().Count();
        }

        public int GetCountBySearchParams(string searchParams)

        {
            return this.usersRepository.All().Where(x => x.Email.Contains(searchParams)
                    || x.FirstName.Contains(searchParams)
                    || x.LastName.Contains(searchParams)).Count();
        }

        public async Task<string> UpdateUserDetailsAsync(UpdateUserDetailsInputModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Description = model.Description;
            user.PhoneNumber = model.PhoneNumber.ToString();

            await userManager.UpdateAsync(user);

            return model.Id;
        }

        public string GetActiveWorkoutPlanId(string userId)
        {
            var plan = this.usersRepository.All().FirstOrDefault(x => x.Id == userId);

            if(plan == null)
            {
                throw new ArgumentException(ErrorMessages.PlanIsNotAssignedToUser);
            }

            return plan.WorkoutPlanId;
        }

        public async Task AssignTrainingProgramToUser(string programId, string userId)
        {
             this.usersRepository.All().FirstOrDefault(x => x.Id == userId).WorkoutPlanId = programId;
             await this.usersRepository.SaveChangesAsync();
        }

        public async Task UpdateProfilePictureAsync(string userId, string pictureUrl)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                throw new ArgumentException(ErrorMessages.UserWithIdDoNoNotExists);
            }

            user.ProfilePicture = pictureUrl;

            await userManager.UpdateAsync(user);
        }

        public string GetProfilePictureUrl(string userId) =>
            usersRepository.GetById(userId).ProfilePicture;

        public async Task AssignRoleAsync(string userId, string roleName)
        {
            var role = await GetRoleAsync(roleName);
            var user = await userManager.FindByIdAsync(userId);

            await userManager.AddToRoleAsync(user, role.Name);
        }

        public async Task RemoveFromRoleAsync(string userId, string roleName)
        {
            var role = await GetRoleAsync(roleName);
            var user = await userManager.FindByIdAsync(userId);

            await userManager.RemoveFromRoleAsync(user, role.Name);
        }

        private async Task<IdentityRole> GetRoleAsync(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if(role == null)
            {
                throw new ArgumentException(ErrorMessages.RoleNotExist);
            }

            return role;
        }

        public async Task<string> GetRoleNameAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var isAdmin = await userManager.IsInRoleAsync(user, GlobalConstants.AdministratorRoleName);

            if (isAdmin)
            {
                return GlobalConstants.AdministratorRoleName;
            }

            return "User";
        }

        public async Task<string> GetEmailAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            return user.Email;
        }

        public async Task<string> GetNameAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            return $"{user.FirstName} {user.LastName}";
        }
    }
}