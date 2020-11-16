using CSScriptLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IranDnn.Modules.BPMS.Kartable.ffffff
{
    public class Test
    {
        public Test()
        {
            dynamic script = CSScript.LoadCode(
                             @"using System;
using IranDnn.Modules.LMS.BusinessLogic;
                             public class Script
                             {
                                 public string SayHello(string greeting)
                                 {
                                   var ss = new AddressController().GetInfo(1);;
return ""ashjjjj"";
                                 }
                             }").CreateObject("*");

            var ss = Convert.ToString(script.SayHello("Hello World!"));
        }
    }
    public interface ICar
    {
        void SayHello(string greeting);
    }
    public class Car : ICar
    {
        public void SayHello(string greeting)
        {
            
        }
    }
}