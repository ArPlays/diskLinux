using System;
using System.IO;
using System.Diagnostics;

public class DiskUsage
{
    public static void GetDiskUsage(out long totalDiskCapacity, out long usedDiskCapacity)
    {
        totalDiskCapacity = 0;
        usedDiskCapacity = 0;

        try
        {
            // Read the /proc/mounts file to get the mount points
            string[] lines = File.ReadAllLines("/proc/mounts");

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');

                if (parts.Length < 2)
                    continue;

                string mountPoint = parts[1];

                // Use the df command to get the disk usage for the mount point
                Process process = new Process();
                process.StartInfo.FileName = "df";
                process.StartInfo.Arguments = $"-k \"{mountPoint}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Parse the output of the df command
                string[] outputLines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (outputLines.Length < 2)
                    continue;

                string[] diskInfo = outputLines[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (diskInfo.Length < 5)
                    continue;

                long total = long.Parse(diskInfo[1]) * 1024; // Convert from KB to Bytes
                long used = long.Parse(diskInfo[2]) * 1024;  // Convert from KB to Bytes

                totalDiskCapacity += total;
                usedDiskCapacity += used;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static void Main()
    {
        GetDiskUsage(out long totalDiskCapacity, out long usedDiskCapacity);

        Console.WriteLine($"Total Disk Capacity: {totalDiskCapacity / (1024 * 1024)} MB");
        Console.WriteLine($"Used Disk Capacity: {usedDiskCapacity / (1024 * 1024)} MB");
    }
}
