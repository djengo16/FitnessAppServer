namespace FitnessApp.Services.ServiceConstants
{
    public static class ErrorMessages
    {
        public const string TrainingProgramIsNotAssigned = "Training program is not assigned for this user!";
        public const string WorkoutPlanDaysNotInRange = "Workout days should be 3,4 or 5 days!";
        public const string InvalidWorkoutPlanDifficulty = "Invalid workout plan difficulty!";
        public const string UserIdIsRequired = "User Id is required!";
        public const string InvalidWorkoutPlanGoal = "Invalid workout plan goal!";
        public const string WorkoutPlansCountNotInRange = "Please choose between 0 and 10 plans to be generated!";
        public const string UserWithIdDoNoNotExists= "User with the given id do not exist!";
        public const string InvalidDaysOfWeek = "Invalid days of week!";
        public const string ExerciseNotFound = "Exercise not found!";
        public const string PlanIsNotAssignedToUser = "Workout plan is not assigned to this user!";
        public const string UserWithPasswordOrEmailNotExists = "User with this password or email does not exists!";
        public const string UserWithEmailAlreadyExists = "User with this email already exists!";
        public const string UserNotFound = "User not found!";
        public const string AccesToPlanDenied = "Access to this workout plan is denied!";
        public const string ExerciseAlreadyInProgram = "This exercise is already in your program!";
        public const string RoleNotExist = "Role with the given name do not exist!";

    }
}
