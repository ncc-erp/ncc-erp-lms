using Mono.Unix;
using System;

namespace RMALMS.SymLinker
{
    public class LinuxSymLinkCreator : ISymLinkCreator
    {
        public bool CreateSymLink(string linkPath, string targetPath, bool file)
        {
            try
            {
                UnixFileInfo f = new UnixFileInfo(targetPath);
                f.CreateSymbolicLink(linkPath);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
