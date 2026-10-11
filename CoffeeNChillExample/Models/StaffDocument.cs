using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeNChillExample.Models
{
    public class StaffDocument //(IIE,2026)
    {
        public string FileName { get; set; } = string.Empty;

        public long Size { get; set; }

        public DateTimeOffset? LastModified { get; set; }
    }
}
//FULL REFERENCE LIST
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 1. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622998  
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 2. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5622999 
//IIE, 2026. Cloud Development B: Lecture notes distributed in CLDV6212 Learning Unit 3. https://mystudies.iie.edu.za/d2l/le/lessons/81852/units/5623000




