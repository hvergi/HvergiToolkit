using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Interfaces;

public interface IDirectoryPicker
{
    Task<string> GetDirectoryPathAsync();
}
