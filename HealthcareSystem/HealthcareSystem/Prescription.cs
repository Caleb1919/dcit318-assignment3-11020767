using System;

namespace HealthcareSystem
{
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");
            if (patientId <= 0) throw new ArgumentOutOfRangeException(nameof(patientId), "PatientId must be positive.");
            if (string.IsNullOrWhiteSpace(medicationName)) throw new ArgumentException("MedicationName is required.", nameof(medicationName));

            Id = id;
            PatientId = patientId;
            MedicationName = medicationName.Trim();
            DateIssued = dateIssued;
        }

        public override string ToString() => $"Rx#{Id} | Patient:{PatientId} | {MedicationName} | {DateIssued:d}";
    }
}
