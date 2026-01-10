
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Business
{
    public class FileSystem
    {
        public int GetFilesCount(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            return Directory.GetFiles(path).Length;
        }
    }

}
