#nullable enable
using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Texo.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            string? test = null;
            var opt = Optional(test);
            
            Console.WriteLine("opt : " + opt.IsNone);
            Console.WriteLine("opt : " + opt.IfNone(() => "is none"));

            var t = Try(() => 1);

            test = "toto";
            
            Console.WriteLine("left : " + test.PadLeft(3));

            TryOption<int> testInt = () => 3;

            Console.WriteLine("is none : " + testInt.IsNone());
            Console.WriteLine("is some : " + testInt.IsSome());
            Console.WriteLine("is fail : " + testInt.IsFail());
                
            TryOption<string> testString = () => null;
            
            Console.WriteLine("is none : " + testString.IsNone());
            Console.WriteLine("is some : " + testString.IsSome());
            Console.WriteLine("is fail : " + testString.IsFail());


        }
    }
}