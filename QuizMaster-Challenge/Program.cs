using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                bool validInput = false;
                while (!validInput)
                {
                    Console.WriteLine("Type 'start' to begin the quiz:");
                    string input = Console.ReadLine();
                    if (input.Trim().ToLower() == "start")
                    {
                        validInput = true;
                        StartQuiz();
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please type 'start' to begin the quiz.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Quiz completed.");
            }
        }

        static void StartQuiz()
        {
            var questions = new List<(string Question, string[] Options, int CorrectAnswer)>
            {
                ("What does CPU stand for?", new string[] {"Central Processing Unit", "Central Process Unit", "Computer Personal Unit", "Central Processor Unit"}, 0),
                ("What is the main function of the ALU?", new string[] {"Store data", "Perform arithmetic and logical operations", "Control input and output", "Manage memory"}, 1),
                ("What does RAM stand for?", new string[] {"Random Access Memory", "Read Access Memory", "Run Access Memory", "Rapid Access Memory"}, 0),
                ("Which of the following is a programming language?", new string[] {"HTTP", "HTML", "FTP", "Python"}, 3),
                ("What does GPU stand for?", new string[] {"Graphics Process Unit", "General Processor Unit", "Graphics Processing Unit", "General Processing Unit"}, 2),
                ("Which company developed the Windows operating system?", new string[] {"Apple", "Microsoft", "Google", "IBM"}, 1),
                ("What does SSD stand for?", new string[] {"Solid State Drive", "Solid State Disk", "Solid State Device", "Super Speed Disk"}, 0),
                ("What is the function of the BIOS?", new string[] {"Manage power", "Boot up the computer", "Run applications", "Manage memory"}, 1),
                ("Which device is used to input graphical images into a computer?", new string[] {"Mouse", "Scanner", "Keyboard", "Printer"}, 1),
                ("What is the primary function of an operating system?", new string[] {"Run applications", "Manage hardware resources", "Store data", "Display graphics"}, 1)
            };

            int score = 0;
            int questionTimeLimit = 30; // seconds

            for (int i = 0; i < questions.Count; i++)
            {
                var (question, options, correctAnswer) = questions[i];
                Console.Clear();
                Console.WriteLine($"Q{i + 1}: {question}");
                for (int j = 0; j < options.Length; j++)
                {
                    Console.WriteLine($"{j + 1}. {options[j]}");
                }

                bool answered = false;
                var cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                var inputTask = Task.Run(() =>
                {
                    Console.WriteLine("\nChoose the correct answer number:");
                    string userAnswer = Console.ReadLine();
                    answered = true;
                    return userAnswer;
                });

                var timerTask = Task.Run(async () =>
                {
                    for (int t = questionTimeLimit; t > 0; t--)
                    {
                        if (answered)
                        {
                            break;
                        }
                        Console.Write($"\rTime remaining: {t} seconds  ");
                        await Task.Delay(1000);
                    }
                    if (!answered)
                    {
                        Console.WriteLine("\rTime's up!                      ");
                        cancellationTokenSource.Cancel();
                    }
                });

                try
                {
                    Task.WaitAny(timerTask, inputTask);

                    if (inputTask.IsCompleted)
                    {
                        string userAnswer = inputTask.Result;
                        if (int.TryParse(userAnswer, out int selectedOption) && selectedOption >= 1 && selectedOption <= options.Length)
                        {
                            if (selectedOption - 1 == correctAnswer)
                            {
                                Console.WriteLine("Correct!");
                                score++;
                            }
                            else
                            {
                                Console.WriteLine($"Incorrect. The correct answer is: {correctAnswer + 1}. {options[correctAnswer]}.");
                            }
                            Console.WriteLine("Loading... Get ready for the next question");
                            Thread.Sleep(6000);
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a number corresponding to one of the options.");
                            Console.WriteLine("Loading... Get ready for re answer");
                            Thread.Sleep(6000);
                            i--;
                        }
                    }
                    else
                    {
                        
                        Console.WriteLine($"The correct answer is: {correctAnswer + 1}. {options[correctAnswer]}.");
                    }
                }
                catch (OperationCanceledException)
                {
                }

                Console.WriteLine($"Your current score is {score} out of {i + 1}.\n");

            }

            Console.Clear();
            Console.WriteLine($"Your final score is {score} out of {questions.Count}.");
        }
    }
}
