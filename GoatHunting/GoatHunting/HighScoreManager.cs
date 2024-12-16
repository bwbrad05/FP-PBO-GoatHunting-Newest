using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoatHunting
{
    public static class HighScoreManager
        {
            // Specify the exact path for the high score file
            private static readonly string HSFilePath = @"C:\ITS\Semester 3\PBO\FP yang paling FP\GoatHunting\GoatHunting\bin\Debug\net8.0-windows\highscore.txt";

            public static int LoadHighScore()
            {
                try
                {
                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(HSFilePath));

                    if (!File.Exists(HSFilePath))
                    {
                        File.WriteAllText(HSFilePath, "0"); // Initialize the file with a default score of 0
                        return 0;
                    }

                    string content = File.ReadAllText(HSFilePath);
                    if (int.TryParse(content, out int highScore))
                    {
                        return highScore;
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading high score: {ex.Message}");
                    return 0; 
                }
            }

            public static void SaveHighScore(int highScore)
            {
                try
                {
                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(HSFilePath));

                    // Write the high score to the file
                    File.WriteAllText(HSFilePath, highScore.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving high score: {ex.Message}");
                }
            }
        }
   }
