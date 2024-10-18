using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class Program
{
    public static void Main()
    {
        var a = 6;
        var b = 1;

        Console.WriteLine($"a={a} b={b}");
        
        a += b;
        b = -(b - a);
        a = a - b;

        Console.WriteLine($"a={a} b={b}");

        string userName = "Tsyns Andrei";

       
        Console.WriteLine("Введите номер  файла:  ");
        string fileChoice = Console.ReadLine();

        // Файлы в зависимости от выбора
        string sequencesFile = $"sequences.{fileChoice}.txt";
        string commandsFile = $"commands.{fileChoice}.txt";
        string outputFile = $"genedata.{fileChoice}.txt";

        /*if (fileChoice == "0")
        {
            sequencesFile = ;
            commandsFile = ;
            outputFile = ;
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
        }*/
        
        
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

        // Поиск последовательностей для обоих белков
        for (int i = 0; i < data.Count; i++)
        {
            var item = data[i];
            if (item.name == protein1)
                seq1 = item.formula;
            if (item.name == protein2)
                seq2 = item.formula;
        }

        // Проверка на наличие последовательностей
        if (seq1 == "" || seq2 == "")
        {
            writer.WriteLine("amino-acids difference:");
            writer.WriteLine("NOT FOUND");
            return;
        }

        
        seq1 = Number_to_letter(seq1);
        seq2 = Number_to_letter(seq2);
        // Подсчет различий
        int diffCount = 0;
        int minLength = Math.Min(seq1.Length, seq2.Length);

        // сравниваем символы до минимальной длины
        for (int i = 0; i < minLength; i++)
        {
            if (seq1[i] != seq2[i])
                diffCount++;
        }

        diffCount += Math.Abs(seq1.Length - seq2.Length);
       
        writer.WriteLine("amino-acids difference:");
        writer.WriteLine(diffCount);
    }

    
    public static string Number_to_letter(string sequence)
    {
        string expandedSequence = "";
        int i = 0;

        while (i < sequence.Length)
        {
            if (char.IsDigit(sequence[i]))
            {
                
                int repeatCount = int.Parse(sequence[i].ToString());
                i++; 
                if (i < sequence.Length && char.IsLetter(sequence[i]))
                {
                   
                    expandedSequence += new string(sequence[i], repeatCount);
                }
            }
            else
            {
               
                expandedSequence += sequence[i];
            }
            i++;
        }

        return expandedSequence;
    }


    // метод для определения наиболее часто встречающейся аминокислоты
    public static void Mode(string protein, List<GeneticData> data, StreamWriter writer)
    {
        string sequence = "";

        // Поиск нужного белка 
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

            // Находим аминокислоту с наибольшим количеством вхождений
            var mostCommonAmino = aminoCount
                .OrderByDescending(x => x.Value)    
                .ThenBy(x => x.Key)                 
                .First();                           

            writer.WriteLine("amino-acid occurs:");
            writer.WriteLine($"{mostCommonAmino.Key}\t\t{mostCommonAmino.Value}");
        }
    }


    // Структура для хранения данных о белке
    public struct GeneticData
    {
        public string name;      
        public string organism;  
        public string formula;   
    }
}
