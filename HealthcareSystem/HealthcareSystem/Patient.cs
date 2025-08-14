using System;

namespace HealthcareSystem
{
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (age < 0) throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");
            if (string.IsNullOrWhiteSpace(gender)) throw new ArgumentException("Gender is required.", nameof(gender));

            Id = id;
            Name = name.Trim();
            Age = age;
            Gender = gender.Trim();
        }

        public override string ToString() => $"#{Id} | {Name} | {Age} | {Gender}";
    }
}
