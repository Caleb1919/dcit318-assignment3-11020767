using System;
using System.Collections.Generic;
using System.IO;

// ------------------------
// 1. Custom Exceptions
// ------------------------
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// ------------------------
// 2. Student Class
// ------------------------
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

// ------------------------
// 3. StudentResultProcessor Class
// ------------------------
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new List<Student>();

        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                string[] parts = line.Split(',');

                if (parts.Length != 3)
                {
                    throw new MissingFieldException($"Line {lineNumber}: Missing required fields.");
                }

                try
                {
                    int id = int.Parse(parts[0].Trim());
                    string fullName = parts[1].Trim();

                    if (string.IsNullOrWhiteSpace(fullName))
                    {
                        throw new MissingFieldException($"Line {lineNumber}: Name field is empty.");
                    }

                    if (!int.TryParse(parts[2].Trim(), out int score))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format.");
                    }

                    students.Add(new Student(id, fullName, score));
                }
                catch (FormatException ex)
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score must be an integer. {ex.Message}");
                }
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

// ------------------------
// 4. Main Application Flow
// ------------------------
public class Program
{
    public static void Main()
    {
        StudentResultProcessor processor = new StudentResultProcessor();

        string inputFile = "students.txt";       // Input file path
        string outputFile = "report.txt";        // Output file path

        try
        {
            List<Student> students = processor.ReadStudentsFromFile(inputFile);
            processor.WriteReportToFile(students, outputFile);
            Console.WriteLine("Report generated successfully!");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: The input file was not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred: " + ex.Message);
        }
    }
}
