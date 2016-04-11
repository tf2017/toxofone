// Based on 
// http://www.codeproject.com/Articles/528178/Load-DLL-From-Embedded-Resource

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

/// <summary>
/// A class for loading Embedded Assembly
/// </summary>
public static class EmbeddedAssembly
{
    // Version 1.3

    private static readonly IDictionary<string, EmbeddedResourceInfo> knownAssemblies = new Dictionary<string, EmbeddedResourceInfo>();

    /// <summary>
    /// Load Assembly, DLL from Embedded Resources into memory.
    /// </summary>
    public static void LoadAssembly(string embeddedResourceName, string assemblyName)
    {
        if (string.IsNullOrEmpty(embeddedResourceName))
        {
            throw new ArgumentException("embeddedResourceName");
        }
        if (string.IsNullOrEmpty(assemblyName))
        {
            throw new ArgumentException("assemblyName");
        }

        byte[] ba = null;

        using (Stream mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceName))
        {
            // Either the file is not existed or it is not mark as embedded resource
            if (mrs == null)
            {
                throw new Exception(embeddedResourceName + " is not found in Embedded Resources.");
            }

            // Get byte[] from the file from embedded resource
            ba = new byte[(int)mrs.Length];
            mrs.Read(ba, 0, (int)mrs.Length);
            try
            {
                Assembly managedAsm = Assembly.Load(ba);

                // Add the assembly/dll into dictionary
                knownAssemblies.Add(managedAsm.FullName, new EmbeddedResourceInfo(embeddedResourceName, assemblyName));
                return;
            }
            catch
            {
                // Purposely do nothing
                // Unmanaged dll or assembly cannot be loaded directly from byte[]
                // Let the process fall through for next part
            }
        }

        bool fileOk = false;
        string tempFilePath = string.Empty;

        using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
        {
            // Get the hash value from embedded DLL/assembly
            string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);

            // Define the storage location of the DLL/assembly
            tempFilePath = Path.Combine(Path.GetTempPath(), assemblyName);

            // Determines whether the DLL/assembly is existed or not
            if (File.Exists(tempFilePath))
            {
                // Get the hash value of the existed file
                byte[] bb = File.ReadAllBytes(tempFilePath);
                string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

                // Compare the existed DLL/assembly with the Embedded DLL/assembly
                if (fileHash == fileHash2)
                {
                    // Same file
                    fileOk = true;
                }
                else
                {
                    // Not same
                    fileOk = false;
                }
            }
            else
            {
                // The DLL/assembly is not existed yet
                fileOk = false;
            }
        }

        // Create the file on disk
        if (!fileOk)
        {
            File.WriteAllBytes(tempFilePath, ba);
        }

        // Load it into memory
        Assembly mixedAsm = Assembly.LoadFile(tempFilePath);
        knownAssemblies.Add(mixedAsm.FullName, new EmbeddedResourceInfo(embeddedResourceName, assemblyName));
    }

    /// <summary>
    /// Retrieve specific loaded DLL/assembly from memory
    /// </summary>
    /// <param name="assemblyFullName"></param>
    /// <returns></returns>
    public static Assembly GetAssembly(string assemblyFullName)
    {
        if (knownAssemblies == null || knownAssemblies.Count == 0)
        {
            return null;
        }

        if (!knownAssemblies.ContainsKey(assemblyFullName))
        {
            // Don't throw Exception if the dictionary does not contain the requested assembly.
            // This is because the event of AssemblyResolve will be raised for every
            // Embedded Resources (such as pictures) of the projects.
            // Those resources will not be loaded by this class and will not exist in dictionary.

            return null;
        }

        EmbeddedResourceInfo resourceInfo = knownAssemblies[assemblyFullName];
        string embeddedResourceName = resourceInfo.ResourceName;
        string assemblyName = resourceInfo.FileName;

        byte[] ba = null;

        using (Stream mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceName))
        {
            // Either the file is not existed or it is not mark as embedded resource
            if (mrs == null)
            {
                throw new Exception(embeddedResourceName + " is not found in Embedded Resources.");
            }

            // Get byte[] from the file from embedded resource
            ba = new byte[(int)mrs.Length];
            mrs.Read(ba, 0, (int)mrs.Length);
            try
            {
                return Assembly.Load(ba);
            }
            catch
            {
                // Purposely do nothing
                // Unmanaged dll or assembly cannot be loaded directly from byte[]
                // Let the process fall through for next part
            }
        }

        bool fileOk = false;
        string tempFilePath = string.Empty;

        using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
        {
            // Get the hash value from embedded DLL/assembly
            string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);

            // Define the storage location of the DLL/assembly
            tempFilePath = Path.Combine(Path.GetTempPath(), assemblyName);

            // Determines whether the DLL/assembly is existed or not
            if (File.Exists(tempFilePath))
            {
                // Get the hash value of the existed file
                byte[] bb = File.ReadAllBytes(tempFilePath);
                string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

                // Compare the existed DLL/assembly with the Embedded DLL/assembly
                if (fileHash == fileHash2)
                {
                    // Same file
                    fileOk = true;
                }
                else
                {
                    // Not same
                    fileOk = false;
                }
            }
            else
            {
                // The DLL/assembly is not existed yet
                fileOk = false;
            }
        }

        // Create the file on disk
        if (!fileOk)
        {
            File.WriteAllBytes(tempFilePath, ba);
        }

        // Load it into memory
        return Assembly.LoadFile(tempFilePath);
    }

    public static void LoadDll(string embeddedResourceName, string dllName)
    {
        if (string.IsNullOrEmpty(embeddedResourceName))
        {
            throw new ArgumentException("embeddedResourceName");
        }
        if (string.IsNullOrEmpty(dllName))
        {
            throw new ArgumentException("dllName");
        }

        byte[] ba = null;

        using (Stream mrs = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceName))
        {
            // Either the file is not existed or it is not mark as embedded resource
            if (mrs == null)
            {
                throw new Exception(embeddedResourceName + " is not found in Embedded Resources.");
            }

            // Get byte[] from the file from embedded resource
            ba = new byte[(int)mrs.Length];
            mrs.Read(ba, 0, (int)mrs.Length);
        }

        bool fileOk = false;
        string tempFilePath = string.Empty;

        using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
        {
            // Get the hash value from embedded DLL/assembly
            string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);

            // Define the storage location of the DLL/assembly
            tempFilePath = Path.Combine(Path.GetTempPath(), dllName);

            // Determines whether the DLL/assembly is existed or not
            if (File.Exists(tempFilePath))
            {
                // Get the hash value of the existed file
                byte[] bb = File.ReadAllBytes(tempFilePath);
                string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

                // Compare the existed DLL/assembly with the Embedded DLL/assembly
                if (fileHash == fileHash2)
                {
                    // Same file
                    fileOk = true;
                }
                else
                {
                    // Not same
                    fileOk = false;
                }
            }
            else
            {
                // The DLL/assembly is not existed yet
                fileOk = false;
            }
        }

        // Create the file on disk
        if (!fileOk)
        {
            File.WriteAllBytes(tempFilePath, ba);
        }

        IntPtr hDll = NativeMethods.LoadLibrary(tempFilePath);
        if (hDll == IntPtr.Zero)
        {
            Exception e = new Win32Exception();
            throw new DllNotFoundException("Unable to load library: " + dllName + " from " + tempFilePath, e);
        }
    }

    private class EmbeddedResourceInfo
    {
        public EmbeddedResourceInfo(string resourceName, string fileName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentException("resourceName");
            }

            if (string.IsNullOrEmpty("fileName"))
            {
                throw new ArgumentException("fileName");
            }

            this.ResourceName = resourceName;
            this.FileName = fileName;
        }

        public string ResourceName { get; private set; }

        public string FileName { get; private set; }
    }

    private static class NativeMethods
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);
    }
}