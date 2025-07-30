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
        public byte[] ReadTxtFileAsBytes(string path)
        {
            try
            {
                // Read the file 
                byte[] text = File.ReadAllBytes(path);
                return text;
            }
            catch(FileNotFoundException e)
            {
                throw new FileNotFoundException();
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
