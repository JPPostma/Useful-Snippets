/*
 * Object for the Log file data collection.
 * 
 * The purpose of this object is to furnish log file capability.  When the LogFileState is set
 * to "ON" in the application configuation setting, all passed information is collected in a
 * file as the program is run.  This information is very helpful in the audit of program use.
 * 
 * Revision History
 * ----------------
 * 12/14/2016    Peter Suknaich      Ridgeview Technology, Inc.         Original creation
 * 
 */

using System;
using System.Text;
using System.IO;

/// <summary>
/// Log File Class.  Used to log information as a program is run.
/// </summary>
class logFile
{
    private StreamWriter logOutputFile;     // Log File Object
    private bool logOutputFile_Open;        // Indication Log file is opened
    private bool noDateTime;                // Include Date/Time Stamp on log records
    private string logFileName;             // Name of the Log File
    private int recordCounter;              // Count of records logged

    /// <summary>
    /// Log File Object
    /// </summary>
    /// <param name="fileNamePrefix">Start of file name</param>
    /// <param name="fileType">File Type</param>
    public logFile(string fileNamePrefix, string fileType = "txt", bool skipDateTime = false)
    {
        logOutputFile_Open = false;          // No log file open
        noDateTime = skipDateTime;

        // Create the Log file name -- use the specified folder
        if (skipDateTime)
        {
            logFileName = fileNamePrefix + "." + fileType;
        }
        else
        {
            logFileName = fileNamePrefix + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss.") + fileType;
        }

        try
        {
            // Open the capture file - Begin with the first record
                logOutputFile = new StreamWriter(logFileName);

            logOutputFile_Open = true;      // ... let the logging begin!
            recordCounter = 0;
        }

        catch
        {
            // If any problems -- No Debugging Active.
        }
    }


    /// <summary>
    /// Capture the data in the debug file
    /// </summary>
    /// <param name="s">Data to add to the Debug File.</param>
    public void Log(String s)
    {
        // Debug file open?  If yes - capture the debug data
        if (logOutputFile_Open)
        {
            recordCounter++;                                   // Count another record logged
            if (noDateTime)
            {
                logOutputFile.WriteLine(s);                     // Write debug info
            }
            else
            {
                logOutputFile.WriteLine(DateTime.Now + ": " + s);  // Write Date/Time and debug info
            }

            logOutputFile.Flush();                              // Keep the file data up-to-date 
        }
    }
    /// <summary>
    /// Add the specified file to the output log file
    /// </summary>
    /// <param name="fileName">Name of file to add to the output log file</param>
    public void logFileContents(string msg, string fileName)
    {
        string line;
        bool originalSkipDateTimeSetting;

        Log(msg);
        Log("--- Contents of: " + fileName);

        originalSkipDateTimeSetting = skipDateTimeInDebug(true);

        try
        {

            // Go get the logfile
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);


            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    Log(line);
                }
            }
        }

        catch
        {
            Log("Error in reading file.  File NOT read.");
        }

        Log("---------- End of file ----------");
        skipDateTimeInDebug(originalSkipDateTimeSetting);

    }


    public bool  skipDateTimeInDebug( bool value )
    {
        bool returnValue;

        returnValue = noDateTime;           // Get original setting
        noDateTime = value;

        return returnValue;                 // Return original setting
    }

    /// <summary>
    /// Get the log file contents -- FOR DEBUG
    /// </summary>
    /// <returns>Entire log file ... in a string.</returns>
    public string getLogFileContents()
    {
        try
        {
            string returnLogData;
            string line;

            // Close the log file
            logOutputFile.Close();

            // Go get the logfile
            var fileStream = new FileStream(logFileName, FileMode.Open, FileAccess.Read);

            returnLogData = "\n --- Contents of: " + logFileName;
            returnLogData += "\n";

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    returnLogData += line;
                    returnLogData += "\n";      // Add a newline for readability
                }
            }
            returnLogData += "---------- End of file ----------\n";

            // Open the capture file - For append.
            logOutputFile = new StreamWriter(logFileName, true);

            return returnLogData;
        }

        catch
        {
            return " >>> Error in File: " + logFileName + " <<<";
        }

    }

    /// <summary>
    /// Destructor
    /// </summary>
    ~logFile()
    {

        // Close the file
        if (logOutputFile_Open)
        {
            try { logOutputFile.Close(); }
            catch { }

            if (recordCounter == 0)     // Remove files with no data -- Makes for a cleaner folder
            {
                try { File.Delete(logFileName); } // Cleanup - Delete the file altogether
                catch { }

            }
        }
    }

}
