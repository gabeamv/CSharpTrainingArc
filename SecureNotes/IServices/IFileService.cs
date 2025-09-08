using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureNotes.IServices
{
    public interface IFileService
    {
        public byte[] ReadAllBytes(string path);
        public string ReadTxtFileAsString(string path);
        public void WriteBytesTxtFile(string path, byte[] bytes);
        public void WriteStringTxtFile(string path, string text);
        
    }
}
