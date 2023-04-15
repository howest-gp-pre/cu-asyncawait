using ShellProgressBar;

namespace Pre.AsyncAwait.Cons;

public class App
{
    private readonly string _sourcePath = "../../../data/source";
    private readonly string _destinationPath = "../../../data/destination";
    private readonly string _calculationsPath = "../../../data/calculations";

    public App()
    {
    }

    private ProgressBar InitProgressBar()
    {
        var options = new ProgressBarOptions()
        {
            ProgressCharacter = '#',
            ForegroundColor = (ConsoleColor)new Random().Next(0, 14)
        };

        return new ProgressBar(100, "Initial message", options);
    }

    public async Task Run()
    {
        // RunCopyFilesSync();

        // RunCopyFilesAsync();

        // RunCalculateSum();

        CalculateAndCopyAsync();

        // Wait to fininsh && cleanup
        // Console.ReadLine();
        ClearFolder(_destinationPath);
    }

    private void RunCalculateSum()
    {
        Console.WriteLine("Calculate sum of all data");
        int sum = CalculateSum(_calculationsPath);
        Console.WriteLine($"The total sum is: {sum}");
    }

    private async void RunCopyFilesAsync()
    {
        using var progressBar = InitProgressBar();

        Console.WriteLine("Deleting files...");
        ClearFolder(_destinationPath);
        Console.WriteLine("Copy files async");
        Task<List<Task>> taskList =
            CopyFilesAsync(Path.GetFullPath(_sourcePath), Path.GetFullPath(_destinationPath), progressBar);

        Task.WaitAll(taskList);
}

    private void RunCopyFilesSync()
    {
        Console.WriteLine("Deleting files...");
        ClearFolder(_destinationPath);

        Console.WriteLine("Copying files from source to dest (sync)");
        using var progressBar = InitProgressBar();
        CopyFilesSync(Path.GetFullPath(_sourcePath), Path.GetFullPath(_destinationPath), progressBar);
    }

    private async Task CalculateAndCopyAsync()
    {
        Console.WriteLine("Deleting files...");
        ClearFolder(_destinationPath);
        Console.WriteLine("Copy files and calculate sum async");
        using var progressBar = InitProgressBar();
        Task<List<Task>> taskList =
            CopyFilesAsync(Path.GetFullPath(_sourcePath), Path.GetFullPath(_destinationPath), progressBar);
        progressBar.MaxTicks = progressBar.MaxTicks + 1;
        Console.WriteLine("Calculate sum of all data");

        int totalSum = CalculateSum(_calculationsPath);
        Console.WriteLine($"The total sum is: {totalSum}");
        progressBar.Tick($"The total sum is: {totalSum}");

        Task.WaitAll(taskList);
    }

    private int CalculateSum(string calculationsPath)
    {
        int totalSum = 0;
        foreach (var inputFile in Directory.EnumerateFiles(calculationsPath))
        {
            int inputNumber = int.Parse(File.ReadAllText(inputFile));
            totalSum += inputNumber;
        }

        Directory.EnumerateFiles(calculationsPath);

        return totalSum;
    }

    private void ClearFolder(string destinationPath)
    {
        foreach (var file in Directory.EnumerateFiles(destinationPath))
        {
            File.Delete(file);
        }
    }

    private async Task<List<Task>> CopyFilesAsync(string sourcePath, string destinationPath, ProgressBar progressBar)
    {
        string[] files = Directory.GetFiles(sourcePath);
        progressBar.MaxTicks = files.Length;

        List<Task> taskList = new List<Task>();

        foreach (var filename in files)
        {
            Task t = CopySingleFileAsync(filename, filename.Replace(sourcePath, destinationPath), progressBar);
            taskList.Add(t);
        }

        return taskList;
    }

    private async Task CopySingleFileAsync(string sourcePath, string destinationPath, ProgressBar progressBar)
    {
        progressBar.Message = $"Copying {destinationPath} asynchronously";
        File.Copy(sourcePath, sourcePath.Replace(sourcePath, destinationPath));
        progressBar.Tick();
    }

    private async Task CopyFilesSync(string sourcePath, string destinationPath, ProgressBar progressBar)
    {
        string[] files = Directory.GetFiles(sourcePath);
        progressBar.MaxTicks = files.Length;

        foreach (string filename in files)
        {
            File.Copy(filename, filename.Replace(sourcePath, destinationPath));
            progressBar.Tick($"Copied {filename.Replace(sourcePath, destinationPath)} synchronously");
        }
    }
}