using System.Collections.Generic;
using System.IO;

namespace RootData.Models
{
    public abstract class RootCommand
    {
        public bool IsValid { get; protected set; }
        protected abstract bool IsValidData(string[] data);       
    }
}
