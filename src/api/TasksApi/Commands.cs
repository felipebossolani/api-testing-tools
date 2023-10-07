using FluentValidation;

namespace TasksApi;

public record InsertUserRequest(string Name)
{    public class Validator : AbstractValidator<InsertUserRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}

public record UpdateUserRequest(Guid Id, string Name)
{
    public class Validator : AbstractValidator<UpdateUserRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}