using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpecimenTransfer.Model.OutputModule;

namespace SpecimenTransfer.Model
{
    public class MachineSetting: AbstractRecipe
    {

        public LoadModuleParamer LoadModuleParam { get; set; } = new LoadModuleParamer();
        public DumpModuleParamer DumpModuleParam { get; set; } = new DumpModuleParamer();
        public OutputModuleParamer OutputModuleParam { get; set; } = new OutputModuleParamer();


    }
}
