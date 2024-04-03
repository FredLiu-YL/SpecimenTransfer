using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpecimenTransfer.Model
{
    public class MachineSetting: AbstractRecipe
    {

        public LoadModuleParamer LoadModuleParam { get; set; } = new LoadModuleParamer();
        public DumpModuleParamer DumpModuleParam { get; set; }= new DumpModuleParamer();
        
    }
}
