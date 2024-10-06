using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{
    public static void Main()
    {
        // Имя пользователя
        string userName = "Tsyns Andrei";

       
        Console.WriteLine("Введите номер  файла  ");
        string fileChoice = Console.ReadLine();

        // Файлы в зависимости от выбора
        string sequencesFile = "";
        string commandsFile = "";
        string outputFile = "";

        if (fileChoice == "0")
        {
            sequencesFile = "sequences.0.txt";
            commandsFile = "commands.0.txt";
            outputFile = "genedata.0.txt";
        }
        else if (fileChoice == "1")
        {
            sequencesFile = "sequences.1.txt";
            commandsFile = "commands.1.txt";
            outputFile = "genedata.1.txt";
        }
        else if (fileChoice == "2")
        {
            sequencesFile = "sequences.2.txt";
            commandsFile = "commands.2.txt";
            outputFile = "genedata.2.txt";
        }
        
        
        // Список с белками
        List<GeneticData> geneticData = new List<GeneticData>();

        
        StreamReader sequencesLines = new StreamReader(sequencesFile);
        while(!sequencesLines.EndOfStream)
        {
            string line = sequencesLines.ReadLine();
            string[] parts = line.Split('\t');
            if (parts.Length == 3)
            {
                GeneticData data = new GeneticData();
                data.name = parts[0];
                data.organism = parts[1];
                data.formula = parts[2];
                geneticData.Add(data);
            }
        }

        
        StreamWriter writer = new StreamWriter(outputFile);
        {
            
            writer.WriteLine(userName);
            writer.WriteLine("Genetic Searching");

            // Чтение команд
            string[] commandsLines = File.ReadAllLines(commandsFile);
            int operationNumber = 1;

            for (int i = 0; i < commandsLines.Length; i++)
            {
                var commandLine = commandsLines[i];
                var parts = commandLine.Split('\t');
                if (parts.Length > 0)
                {
                    string command = parts[0].ToLower();
                    writer.WriteLine("--------------------------------------------------------------------------");
                    writer.WriteLine($"{operationNumber:D3}   {commandLine}");

                    if (command == "search")
                    {
                        string sequence = parts[1];
                        Search(sequence, geneticData, writer);
                    }
                    else if (command == "diff")
                    {
                        string protein1 = parts[1];
                        string protein2 = parts[2];
                        Diff(protein1, protein2, geneticData, writer);
                    }
                    else if (command == "mode")
                    {
                        string protein = parts[1];
                        Mode(protein, geneticData, writer);
                    }

                    operationNumber++;
                }
            }

           
            writer.WriteLine("--------------------------------------------------------------------------");
        }
        writer.Close();
        Console.WriteLine($"Обработка завершена. Результаты записаны в {outputFile}");
    }

    // метод поиска последовательности аминокислот
    public static void Search(string sequence, List<GeneticData> data, StreamWriter writer)
    {
        writer.WriteLine("organism\t\tprotein");
        bool found = false;

        for (int i = 0; i < data.Count; i++)
        {
            var item = data[i];
            if (item.formula.Contains(sequence))
            {
                writer.WriteLine($"{item.organism}\t\t{item.name}");
                found = true;
            }
        }

        if (!found)
        {
            writer.WriteLine("NOT FOUND");
        }
    }

    // метод сравнения двух белков
    public static void Diff(string protein1, string protein2, List<GeneticData> data, StreamWriter writer)
    {
        string seq1 = "";
        string seq2 = "";

        for (int i = 0; i < data.Count; i++)
        {
            var item = data[i];
            if (item.name == protein1)
                seq1 = item.formula;
            if (item.name == protein2)
                seq2 = item.formula;
        }


        if (seq1 == "" || seq2 == "")
        {
            writer.WriteLine("amino-acids difference:");
            writer.WriteLine("NOT FOUND");
        }
        else
        {
            int diffCount = 0;
            int minLength = Math.Min(seq1.Length, seq2.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (seq1[i] != seq2[i])
                    diffCount++;
            }
            diffCount += Math.Abs(seq1.Length - seq2.Length);
            writer.WriteLine("amino-acids difference:");
            writer.WriteLine(diffCount);
        }
    }

    // метод для определения наиболее часто встречающейся аминокислоты
    public static void Mode(string protein, List<GeneticData> data, StreamWriter writer)
    {
        string sequence = "";

        for (int i = 0; i < data.Count; i++)
        {
            var item = data[i];
            if (item.name == protein)
            {
                sequence = item.formula;
                break;
            }
        }


        if (sequence == "")
        {
            writer.WriteLine("amino-acid occurs:");
            writer.WriteLine("NOT FOUND");
        }
        else
        {
            Dictionary<char, int> aminoCount = new Dictionary<char, int>();

            for (int i = 0; i < sequence.Length; i++)
            {
                char amino = sequence[i];
                if (aminoCount.ContainsKey(amino))
                    aminoCount[amino]++;
                else
                    aminoCount[amino] = 1;
            }


            var mostCommonAmino = aminoCount.OrderByDescending(x => x.Value).First();
            writer.WriteLine("amino-acid occurs:");
            writer.WriteLine($"{mostCommonAmino.Key}\t\t{mostCommonAmino.Value}");
        }
    }

    // Структура для хранения данных о белке
    public struct GeneticData
    {
        public string name;      // Название белка
        public string organism;  // Организм
        public string formula;   // Последовательность аминокислот
    }
}
