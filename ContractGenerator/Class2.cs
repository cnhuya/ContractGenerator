using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContractGenerator
{
    public class Class2
    {
        // Volně dostupná public promněná
        public static bool launched = false;
        public static void Launch()
        {
            // Pokud program není spuštěn
            if (launched == false)
            {
                // Jak spustit/zobrazit winform aplikaci skrze launcher aplikaci
                // zdroj: https://stackoverflow.com/questions/2618830/show-a-form-from-another-form
                var form = new ContractGenerator();
                form.Show();
                launched = true;
            }

            // Pokud program je spuštěn
            else if (launched == true)
            {
              
            }


        }
    }
}
