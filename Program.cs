using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

class Program
{
    static void Main()
    {
        string inputCsvFilePath = "./SampleCSVFile.csv"; // Replace with the path to your input CSV file
        string uniqueNamesCsvFilePath = "uniquenames.csv"; // Path to uniquenames.csv
        string outputCsvFilePath = "./output-SampleCSVFile.csv"; // Output CSV file path

        // Read unique test case names from uniquenames.csv
        HashSet<string> uniqueTestNames = new HashSet<string>();
        using (TextFieldParser parser = new TextFieldParser(uniqueNamesCsvFilePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            bool isFirstLine = true;

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                uniqueTestNames.Add(fields[0]);
            }
        }

        List<TestCase> testCases = new List<TestCase>();
        TestCase currentTestCase = null;

        using (TextFieldParser parser = new TextFieldParser(inputCsvFilePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters("\t");

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                // Check if it's a new test case
                if (fields.Length >= 5 && uniqueTestNames.Contains(fields[1]))
                {
                    currentTestCase = new TestCase
                    {
                        TestID = int.Parse(fields[0]),
                        TestName = fields[1],
                        Steps = new List<TestStep>()
                    };

                    testCases.Add(currentTestCase);
                }

                // Add test steps
                if (currentTestCase != null && fields.Length >= 5)
                {
                    TestStep step = new TestStep
                    {
                        StepName = fields[2],
                        Expected = fields[3],
                        Status = fields[4]
                    };

                    currentTestCase.Steps.Add(step);
                }
            }
        }

        // Write the extracted test case data to a new CSV file
        using (StreamWriter writer = new StreamWriter(outputCsvFilePath))
        {
            writer.WriteLine("Test ID,Test Name,Step Name,Expected,Status");

            foreach (var testCase in testCases)
            {
                foreach (var step in testCase.Steps)
                {
                    writer.WriteLine($"{testCase.TestID},{testCase.TestName},{step.StepName},{step.Expected},{step.Status}");
                }
            }
        }

        Console.WriteLine($"Extracted test case data has been written to {outputCsvFilePath}");
    }
}