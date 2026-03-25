using BMS.Application.Common.Exceptions;
using BMS.Application.Common.Interfaces;
using BMS.Application.Models;
using BMS.Application.Users.Commands;
using BMS.Domain.Entities;
using BMS.Domain.Entities.BMS;
using MediatR;


namespace BMS.Application.Users.Handlers;

public sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, ApiResponse<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPersonRepository personRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _personRepository = personRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<Guid>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ Check Person exists
        var person = await _personRepository
            .GetByIdAsync(request.PersonId, cancellationToken);

        if (person is null)
            throw new NotFoundException(
                $"Person with id '{request.PersonId}' not found.");

        // 2️⃣ Prevent duplicate user for Person
        var userExistsForPerson = await _userRepository
            .ExistsByPersonIdAsync(request.PersonId, cancellationToken);

        if (userExistsForPerson)
            throw new BusinessRuleException(
                "This person already has a user account.");


        // 3️⃣ Normalize username (same rule used in Domain)
        var normalizedUserName = request.UserName
            .Trim()
            .ToLowerInvariant();

        // 4️⃣ Prevent duplicate Username
        var usernameExists = await _userRepository
            .ExistsByUserNameAsync(
                normalizedUserName,
                cancellationToken);

        if (usernameExists)
            throw new BusinessRuleException(
                "Username already exists.");

        // 5️⃣ Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 6️⃣ Create User (Domain handles normalization again safely)
        var user = new User(
            request.PersonId,
            request.UserName,
            passwordHash);

        // 7️⃣ Assign roles atomically (Domain-controlled)
        user.SyncRoles(request.RoleIds ?? Array.Empty<int>());

        // 8️⃣ Persist
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<Guid>.SuccessResponse(user.Id, "کاربر ایجاد شد");
    }
}
