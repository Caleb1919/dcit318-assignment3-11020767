using System;

namespace HealthcareSystem
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var app = new HealthSystemApp();

            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            int chosenPatientId = 2; // Change to test different patients
            app.PrintPrescriptionsForPatient(chosenPatientId);
        }
    }
}
