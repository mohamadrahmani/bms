using System;


namespace BMS.Domain.Entities
{
    public class Person : BaseEntity<Guid>
    {
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;

        public string? Email { get; private set; }
        public string? Mobile { get; private set; }

        public bool IsActive { get; private set; }

        public User? User { get; private set; }

        private Person() { } // For EF

        public Person(
            string firstName,
            string lastName,
            string? email,
            string? mobile)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required");

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email?.Trim();
            Mobile = mobile?.Trim();

            IsActive = true;
        }

        public void Update(
            string firstName,
            string lastName,
            string? email,
            string? mobile)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required");

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email?.Trim();
            Mobile = mobile?.Trim();

            SetUpdated();
        }

        public void Deactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
            SetUpdated();
        }

        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
            SetUpdated();
        }
    }
}
