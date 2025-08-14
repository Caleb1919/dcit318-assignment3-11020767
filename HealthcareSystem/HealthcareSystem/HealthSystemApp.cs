using System;
using System.Collections.Generic;

namespace HealthcareSystem
{
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Ama Mensah", 29, "Female"));
            _patientRepo.Add(new Patient(2, "Kwame Boateng", 41, "Male"));
            _patientRepo.Add(new Patient(3, "Efua Owusu", 35, "Female"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin 500mg", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen 200mg", DateTime.Today.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Metformin 500mg", DateTime.Today.AddDays(-20)));
            _prescriptionRepo.Add(new Prescription(104, 2, "Lisinopril 10mg", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(105, 3, "Cetirizine 10mg", DateTime.Today.AddDays(-1)));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();

            foreach (var rx in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.TryGetValue(rx.PatientId, out var list))
                {
                    list = new List<Prescription>();
                    _prescriptionMap[rx.PatientId] = list;
                }
                list.Add(rx);
            }

            foreach (var kvp in _prescriptionMap)
                kvp.Value.Sort((a, b) => b.DateIssued.CompareTo(a.DateIssued));
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== Patients ===");
            foreach (var p in _patientRepo.GetAll())
                Console.WriteLine(p);
            Console.WriteLine();
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list)
                ? new List<Prescription>(list)
                : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var patient = _patientRepo.GetById(p => p.Id == patientId);
            if (patient is null)
            {
                Console.WriteLine($"No patient found with Id {patientId}.\n");
                return;
            }

            Console.WriteLine($"=== Prescriptions for {patient.Name} (#{patient.Id}) ===");
            var prescriptions = GetPrescriptionsByPatientId(patientId);

            if (prescriptions.Count == 0)
            {
                Console.WriteLine("No prescriptions found.\n");
                return;
            }

            foreach (var rx in prescriptions)
                Console.WriteLine(rx);
            Console.WriteLine();
        }
    }
}
