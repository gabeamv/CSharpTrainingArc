using SecureNotes.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Shapes;

namespace SecureNotes.Services
{
    public class FileService : IFileService
    {
        public byte[] ReadAllBytes(string path)
        {
            try
            {
                // Read the file 
                byte[] bytes = File.ReadAllBytes(path);
                return bytes;
            }
            catch(FileNotFoundException e)
            {
                throw new FileNotFoundException();
            }
        }

        public void WriteAllBytes(string path, byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(path, bytes);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new DirectoryNotFoundException();
            }
            catch (IOException e)
            {
                throw new IOException();
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException();
            }
        }

        public string ReadTxtFileAsString(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch(FileNotFoundException e)
            {
                throw new FileNotFoundException();
            }
        }

        public void WriteBytesTxtFile(string path, byte[] bytes)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(bytes);
                }     
            }
            catch(FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }

        public void WriteStringTxtFile(string path, string text)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(text);
                }

            }
            catch (FileNotFoundException e) { throw new FileNotFoundException(); }
        }
    }
}
