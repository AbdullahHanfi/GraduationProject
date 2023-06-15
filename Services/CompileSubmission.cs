using GraduationProject.BindingModels;
using GraduationProject.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GraduationProject.Services
{
    static public class CompileSubmission
    {
        static public bool CheckStatus(ref OutputData4Submission CodeOutPut, ref Submission submission, string OutputCase)
        {
            submission.ExecutionTime = Math.Max((double)CodeOutPut.time, submission.ExecutionTime);
            submission.Memory = Math.Max((int)CodeOutPut.memory, submission.Memory);

            if (CodeOutPut.Result.StartsWith("Time limit exceeded"))
            {
                submission.Status = (int)SubmissionStatus.TimeLimit;
                return false;
            }
            else if (CodeOutPut.Result.StartsWith("Compilation error"))
            {
                submission.Status = (int)SubmissionStatus.Compilationerror;
                return false;
            }
            else if (CodeOutPut.Result.StartsWith("Memory limit exceeded"))
            {
                submission.Status = (int)SubmissionStatus.MemoryLimit;
                return false;
            }
            else
            {
                var outputcase = System.IO.File.ReadAllText(Path.GetFullPath($"~/App_Data/TestCases/{OutputCase}"));
                outputcase = Regex.Replace(outputcase, @"\s+", " ");
                var Check = CodeOutPut.Result.Equals(outputcase);
                if (!Check)
                {
                    submission.Status = (int)SubmissionStatus.Wrong;
                    return false;
                }
                return true;
            }
        }
        static public List<TestCasesBinding>? TestCases4Problem(int id)
        {
            List<TestCasesBinding> testCases = new List<TestCasesBinding>();
            using (var dbWR = new ProjectDbContext())
            {
                var inputCases = dbWR.InputCases.Where(item => item.ProblemId == id);

                foreach (var inputCase in inputCases)
                {
                    TestCasesBinding item = new TestCasesBinding()
                    {
                        InputCase = inputCase.Id,
                        OutputCase = dbWR.OutputCases.FirstOrDefault(item => item.InputId == inputCase.Id)?.Id ?? ""
                    };
                    if (item != null && !string.IsNullOrEmpty(item?.InputCase) && !string.IsNullOrEmpty(item?.OutputCase))
                    {
                        testCases.Add(item);
                    }
                    else
                    {
                        return default;
                    }
                }
                return testCases;
            }
        }
        static public OutputData4Submission ExecuteCppCode(string code, string input, decimal timeLimit, int memoryLimit)
        {
            var outputData = new OutputData4Submission();
            // Define the file names and paths
            string sourceFile = "source.cpp", executableFile = "executable.exe";
            // Save the C++ code to a file
            File.WriteAllText(sourceFile, code);


            // Define the process startup info
            var startInfo = new ProcessStartInfo
            {
                FileName = "g++",
                Arguments = $"{Path.Combine(".", sourceFile)} -o {Path.Combine(".", executableFile)}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Compile the C++ code
            var compilerProcess = Process.Start(startInfo);
            compilerProcess.WaitForExit();

            if (compilerProcess.ExitCode != 0)
            {
                // Compilation error occurred
                outputData.Result = "Compilation error: " + compilerProcess.StandardError.ReadToEnd();
                return outputData;
            }

            // Define the process startup info for the executable
            startInfo.FileName = Path.Combine(".", executableFile);
            startInfo.Arguments = "";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;

            // Execute the executable with the input
            var executorProcess = new Process();
            executorProcess.StartInfo = startInfo;
            executorProcess.EnableRaisingEvents = true;

            // Define the memory limit and time limit for the process
            executorProcess.StartInfo.EnvironmentVariables["RLIMIT_AS"] = $"{memoryLimit}M";
            executorProcess.StartInfo.EnvironmentVariables["RLIMIT_CPU"] = $"{timeLimit}";

            // Start the process
            executorProcess.Start();

            // Write the input to the standard input stream of the process
            executorProcess.StandardInput.Write(input);


            // Wait for the process to exit or reach the time limit or memory limit
            while (!executorProcess.HasExited)
            {
                while (!executorProcess.WaitForExit(100))
                {
                    // Check memory usage and kill the process if it exceeds the memory limit
                    long memoryUsage = executorProcess.WorkingSet64;
                    if (memoryUsage >= memoryLimit * 1024L * 1024L)
                    {
                        executorProcess.Kill();
                        outputData.Result = "Memory limit exceeded";
                        outputData.memory = executorProcess.WorkingSet64 / 1024L;
                        outputData.time = executorProcess.TotalProcessorTime.Milliseconds;
                        return outputData;
                    }
                }
                timeLimit -= 0.1M;
                //Check For Time Limit and kill the process if it exceeds 
                if (timeLimit <= 0)
                {
                    executorProcess.Kill();
                    outputData.Result = "Time limit exceeded";
                    outputData.memory = executorProcess.WorkingSet64 / 1024L;
                    outputData.time = executorProcess.TotalProcessorTime.Milliseconds;
                    return outputData;
                }
            }
            // Read the output from the process
            outputData.Result = Regex.Replace(executorProcess.StandardOutput.ReadToEnd(), @"\s+", " ");
            outputData.memory = executorProcess.WorkingSet64 / 1024L;
            outputData.time = executorProcess.TotalProcessorTime.Milliseconds;

            // Return the output
            return outputData;
        }

    }
}