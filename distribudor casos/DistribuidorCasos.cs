using CliWrap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace run_python_script_argv
{
    class DistribuidorCasos
    {
        public static async Task DistribuirConPriorizacion(string ultimoCortePath, string trazadoresFilePath, string casosPorTrazador)
        {
            var args = new[] {
                "priorizacion_corte.py",
                ultimoCortePath,
                trazadoresFilePath,
                casosPorTrazador
            };
            Directory.SetCurrentDirectory("../Distribucion de cortes");
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();
            await Cli.Wrap("python").WithArguments(args)
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();
            Console.WriteLine();
        }
    }
}
