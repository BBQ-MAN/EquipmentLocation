using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Windows File Browser for selecting CSV files
/// Uses Windows native file dialog
/// </summary>
public class FileBrowser
{
    // Windows File Dialog
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public string filter = null;
        public string customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public string file = null;
        public int maxFile = 0;
        public string fileTitle = null;
        public int maxFileTitle = 0;
        public string initialDir = null;
        public string title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public string defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public string templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

    public static string OpenFileDialog(string title = "Select CSV File", string filter = "CSV Files\0*.csv\0All Files\0*.*\0", string defaultPath = "")
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = filter;
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = string.IsNullOrEmpty(defaultPath) ? Application.dataPath : defaultPath;
        ofn.title = title;
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800; // OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST

        if (GetOpenFileName(ofn))
        {
            Debug.Log($"Selected file: {ofn.file}");
            return ofn.file;
        }
        
        return string.Empty;
    }

    public static string OpenFolderDialog(string title = "Select Folder with CSV Files")
    {
        // For folder selection, we'll use a different approach
        // Unity doesn't have built-in folder browser, so we'll select any CSV file and extract the folder
        string filePath = OpenFileDialog(title + " (Select any CSV file in the folder)", "CSV Files\0*.csv\0");
        
        if (!string.IsNullOrEmpty(filePath))
        {
            string folderPath = System.IO.Path.GetDirectoryName(filePath);
            Debug.Log($"Selected folder: {folderPath}");
            return folderPath;
        }
        
        return string.Empty;
    }

    // Alternative: Simple implementation using Unity's Application.OpenFilePanel (Editor only)
    #if UNITY_EDITOR
    public static string OpenFileDialogEditor()
    {
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
        return path;
    }
    
    public static string OpenFolderDialogEditor()
    {
        string path = UnityEditor.EditorUtility.OpenFolderPanel("Select Folder with CSV Files", "", "");
        return path;
    }
    #endif
}