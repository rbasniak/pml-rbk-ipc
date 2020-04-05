using Aveva.Core.Utilities.CommandLine;
using Aveva.Core.Utilities.Messaging;
using Aveva.ApplicationFramework;
using Aveva.ApplicationFramework.Presentation;
using Aveva.Core.Presentation;
using Aveva.Core.Database;
using System;
using Newtonsoft.Json;
using Command = Aveva.Core.Utilities.CommandLine.Command;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RbkPmlTools.Commands
{
   public class CommandLineWrapper
   {
        public CommandLineWrapper()
        {

        } 

        public CommandResponse Execute(CommandInput input)
        {
            try
            {
                if (input.Command.ToUpper() == "Q CE")
                {
                    return new CommandResponse
                    {
                        Command = input,
                        Error = null,
                        Result = JsonConvert.SerializeObject(new
                        {
                            Name = CurrentElement.Element.Name(),
                            Type = CurrentElement.Element.ElementType.Name,
                            RefNo = $"{CurrentElement.Element.RefNo()[0]}/{CurrentElement.Element.RefNo()[1]}"
                        }, Formatting.Indented)
                    };
                }


                //Command.UpdateOn(false);
                //Command.EventsOn(false);

                var tempFilename = Path.GetTempFileName();

                Command.CreateCommand("ALPHA LOG /" + tempFilename + " OVERWRITE").Run();
                
                var command = Command.CreateCommand(input.Command);
                var result = command.Run();

                Command.CreateCommand("ALPHA LOG END").Run();

                //Thread.Sleep(250);

                //Command.UpdateOn(true);
                //Command.EventsOn(true);

                //Command.Update();

                if (command.Error.MessageNumber == 0)
                {
                    return new CommandResponse
                    {
                        Result = File.ReadAllText(tempFilename),
                        Command = input,
                        Error = null
                    };
                }
                else
                {
                    return new CommandResponse
                    {
                        Command = input,
                        Result = null,
                        Error = command.Error,
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResponse
                {
                    Command = input,
                    Result = null,
                    Error = null,
                };
            }
        } 

        public class CommandInput
        {
            public CommandInput(string command)
            {
                Id = Guid.NewGuid();
                Command = command;
            }

            public Guid Id { get; private set; }
            public string Command { get; private set; }
        }

        public class CommandResponse
        {
            public CommandInput Command { get; internal set; }
            public string Result { get; internal set; }
            public bool Success => Error == null;
            public PdmsMessage Error { get; internal set; }
        }
    }
}
