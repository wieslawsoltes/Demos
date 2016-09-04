
namespace Logic.Model
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.InteropServices;
    
    #endregion

    #region Binary File Format

    // command Id: sizeof(UInt16)
    // command data: sizeof(command)
    // ...
    // elements counter: sizeof(UInt32)
    //
    // where:
    // Id   Command
    // ---------------
    // 0    Solution
    // 1    Project
    // 2    Context
    // 3    Pin
    // 4    Signal
    // 5    AndGate
    // 6    OrGate
    // 7    TimerOn
    // 8    TimerOff
    // 9    TimerPulse
    // 10   Connect

    // Solution command:
    // UInt32 Id;

    // Project command:
    // UInt32 Id;

    // Context command:
    // UInt32 Id;

    // Pin command:
    // UInt32 Id;

    // Signal command:
    // UInt32 Id;
    // UInt32 InputPinId;
    // UInt32 OutputPinId;

    // AndGate command:
    // UInt32 Id;
    // UInt32 LeftPinId;
    // UInt32 RightPinId;
    // UInt32 TopPinId;
    // UInt32 BottomPinId;

    // OrGate command:
    // UInt32 Id;
    // UInt32 LeftPinId;
    // UInt32 RightPinId;
    // UInt32 TopPinId;
    // UInt32 BottomPinId;

    // TimerOn command:
    // UInt32 Id;
    // UInt32 LeftPinId;
    // UInt32 RightPinId;
    // UInt32 TopPinId;
    // UInt32 BottomPinId;
    // Single Delay;

    // TimerOff command:
    // UInt32 Id;
    // UInt32 LeftPinId;
    // UInt32 RightPinId;
    // UInt32 TopPinId;
    // UInt32 BottomPinId;
    // Single Delay;

    // TimerPulse command:
    // UInt32 Id;
    // UInt32 LeftPinId;
    // UInt32 RightPinId;
    // UInt32 TopPinId;
    // UInt32 BottomPinId;
    // Single Delay;

    // Connect command:
    // UInt32 Id;
    // UInt32 SrcPinId;
    // UInt32 DstPinId;
    // Byte InvertStart;
    // Byte InvertEnd;

    #endregion

    #region Binary Parser

    public static class BinaryParser
    {
        #region Fields

        private static int sizeOfCommandId = Marshal.SizeOf(typeof(UInt16));
        private static int sizeOfTotalElements = Marshal.SizeOf(typeof(UInt32));

        private static Element[] Elements;

        public static Solution CurrentSolution;
        public static Project CurrentProject;
        public static Context CurrentContext;

        #endregion

        #region GZip

        public static void CompressFile(string sourcePath, string destinationPath)
        {
            using (var inputStream = File.OpenRead(sourcePath))
            {
                using (var outputStream = File.Create(destinationPath))
                {
                    using (var compressedStream = new GZipStream(outputStream, CompressionMode.Compress))
                    {
                        inputStream.CopyTo(compressedStream);
                    }
                }
            }
        }

        public static void DeCompressFile(string sourcePath, string destinationPath)
        {
            using (var inputStream = File.OpenRead(sourcePath))
            {
                using (var outputStream = File.Create(destinationPath))
                {
                    using (var deCompressedStream = new GZipStream(outputStream, CompressionMode.Decompress))
                    {
                        inputStream.CopyTo(deCompressedStream);
                    }
                }
            }
        }

        #endregion

        #region Open

        public static void OpenCompressed(string path)
        {
            using (var fileStream = File.OpenRead(path))
            {
                using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(memoryStream);

                        ReadData(memoryStream);
                    }
                }
            }
        }

        public static void OpenUnCompressed(string path)
        {
            using (var fileStream = File.OpenRead(path))
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);

                    ReadData(memoryStream);
                }
            }
        }

        private static void ReadData(MemoryStream memoryStream)
        {
            using (var reader = new BinaryReader(memoryStream))
            {
                UInt32 totalElements = 0;
                UInt16 commandId = 0;

                long size = memoryStream.Length;
                long dataSize = size - sizeOfTotalElements;

                // get element counter
                memoryStream.Seek(size - sizeOfTotalElements, SeekOrigin.Begin);

                totalElements = reader.ReadUInt32();

                memoryStream.Seek(0, SeekOrigin.Begin);

                // allocate elements array
                Elements = new Element[totalElements];

                while (memoryStream.Position < dataSize)
                {
                    commandId = reader.ReadUInt16();

                    switch (commandId)
                    {
                        // Solution
                        case 0:
                            {
                                UInt32 Id = reader.ReadUInt32();

                                AddSolution(ref Id);
                            }
                            break;
                        // Project
                        case 1:
                            {
                                UInt32 Id = reader.ReadUInt32();

                                AddProject(ref Id);
                            }
                            break;
                        // Context
                        case 2:
                            {
                                UInt32 Id = reader.ReadUInt32();

                                AddContext(ref Id);
                            }
                            break;
                        // Pin
                        case 3:
                            {
                                UInt32 Id = reader.ReadUInt32();

                                AddPin(ref Id);
                            }
                            break;
                        // Signal
                        case 4:
                            {
                                UInt32 Id = reader.ReadUInt32();
                                UInt32 InputPinId = reader.ReadUInt32();
                                UInt32 OutputPinId = reader.ReadUInt32();

                                AddSignal(ref Id, ref InputPinId, ref OutputPinId);
                            }
                            break;
                        // AndGate
                        case 5:
                            {
                                UInt32 Id = reader.ReadUInt32();
                                UInt32 LeftPinId = reader.ReadUInt32();
                                UInt32 RightPinId = reader.ReadUInt32();
                                UInt32 TopPinId = reader.ReadUInt32();
                                UInt32 BottomPinId = reader.ReadUInt32();

                                AddAndGate(ref Id, ref LeftPinId, ref RightPinId, ref TopPinId, ref BottomPinId);
                            }
                            break;
                        // OrGate
                        case 6:
                            {
                                UInt32 Id = reader.ReadUInt32();
                                UInt32 LeftPinId = reader.ReadUInt32();
                                UInt32 RightPinId = reader.ReadUInt32();
                                UInt32 TopPinId = reader.ReadUInt32();
                                UInt32 BottomPinId = reader.ReadUInt32();

                                AddOrGate(ref Id, ref LeftPinId, ref RightPinId, ref TopPinId, ref BottomPinId);
                            }
                            break;
                        // TimerOn
                        case 7:
                            {
                                UInt32 Id = reader.ReadUInt32();
                                UInt32 LeftPinId = reader.ReadUInt32();
                                UInt32 RightPinId = reader.ReadUInt32();
                                UInt32 TopPinId = reader.ReadUInt32();
                                UInt32 BottomPinId = reader.ReadUInt32();
                                Single Delay = reader.ReadSingle();

                                AddTimerOn(ref Id, ref LeftPinId, ref RightPinId, ref TopPinId, ref BottomPinId, ref Delay);
                            }
                            break;
                        // TimerOff
                        case 8:
                            {
                                UInt32 Id = reader.ReadUInt32();
                                UInt32 LeftPinId = reader.ReadUInt32();
                                UInt32 RightPinId = reader.ReadUInt32();
                                UInt32 TopPinId = reader.ReadUInt32();
                                UInt32 BottomPinId = reader.ReadUInt32();
                                Single Delay = reader.ReadSingle();

                                AddTimerOff(ref Id, ref LeftPinId, ref RightPinId, ref TopPinId, ref BottomPinId, ref Delay);
                            }
                            break;
                        // TimerPulse
                        case 9:
                            {
                                UInt32 Id = reader.ReadUInt32();
                                UInt32 LeftPinId = reader.ReadUInt32();
                                UInt32 RightPinId = reader.ReadUInt32();
                                UInt32 TopPinId = reader.ReadUInt32();
                                UInt32 BottomPinId = reader.ReadUInt32();
                                Single Delay = reader.ReadSingle();

                                AddTimerPulse(ref Id, ref LeftPinId, ref RightPinId, ref TopPinId, ref BottomPinId, ref Delay);
                            }
                            break;
                        // Connect
                        case 10:
                            {
                                UInt32 Id = reader.ReadUInt32();
                                UInt32 SrcPinId = reader.ReadUInt32();
                                UInt32 DstPinId = reader.ReadUInt32();
                                Byte InvertStart = reader.ReadByte();
                                Byte InvertEnd = reader.ReadByte();

                                AddWire(ref Id, ref SrcPinId, ref DstPinId, ref InvertStart, ref InvertEnd);
                            }
                            break;
                    }
                }

                // reset elements cache array
                for (UInt32 i = 0; i < totalElements; i++)
                {
                    Elements[i] = null;
                }

                Elements = null;
            }
        }

        #endregion

        #region Save

        public static void SaveCompressed(string path, Solution solution)
        {
            throw new NotImplementedException("SaveCompressed");
        }

        public static void SaveUnCompressed(string path, Solution solution)
        {
            throw new NotImplementedException("SaveUnCompressed");
        }

        #endregion

        #region Add

        private static void AddWire(ref UInt32 Id, ref UInt32 SrcPinId, ref UInt32 DstPinId, ref Byte InvertStart, ref Byte InvertEnd)
        {
            var context = CurrentContext;
            var children = context.Children;

            var p_src = Elements[SrcPinId];
            var p_dst = Elements[DstPinId];

            if (p_src != null && p_dst != null && p_src is Pin && p_dst is Pin)
            {
                var wire = new Wire(); //WIRE

                wire.Id = Id;

                wire.Start = p_src as Pin;
                wire.End = p_dst as Pin;

                wire.InvertStart = InvertStart == 0x01;
                wire.InvertEnd = InvertEnd == 0x01;

                wire.Parent = context;

                children.Add(wire);

                Elements[Id] = wire;
            }
        }

        private static void AddTimerPulse(ref UInt32 Id, ref UInt32 LeftPinId, ref UInt32 RightPinId, ref UInt32 TopPinId, ref UInt32 BottomPinId, ref Single Delay)
        {
            var context = CurrentContext;
            var children = context.Children;

            var tp = new TimerPulse(Delay); //TP

            var p_top = new Pin(PinType.Undefined, true); //TOP
            var p_bottom = new Pin(PinType.Undefined, true); //BOTTOM
            var p_left = new Pin(PinType.Undefined, true); //LEFT
            var p_right = new Pin(PinType.Undefined, true); //RIGHT

            tp.Id = Id;
            p_top.Id = TopPinId;
            p_bottom.Id = BottomPinId;
            p_left.Id = LeftPinId;
            p_right.Id = RightPinId;

            tp.Children.Add(p_top);
            tp.Children.Add(p_bottom);
            tp.Children.Add(p_left);
            tp.Children.Add(p_right);

            tp.Parent = context;

            p_top.Parent = tp;
            p_bottom.Parent = tp;
            p_left.Parent = tp;
            p_right.Parent = tp;

            children.Add(tp);
            children.Add(p_top);
            children.Add(p_bottom);
            children.Add(p_left);
            children.Add(p_right);

            Elements[Id] = tp;
            Elements[TopPinId] = p_top;
            Elements[BottomPinId] = p_bottom;
            Elements[LeftPinId] = p_left;
            Elements[RightPinId] = p_right;
        }

        private static void AddTimerOff(ref UInt32 Id, ref UInt32 LeftPinId, ref UInt32 RightPinId, ref UInt32 TopPinId, ref UInt32 BottomPinId, ref Single Delay)
        {
            var context = CurrentContext;
            var children = context.Children;

            var toff = new TimerOff(Delay); //TOFF

            var p_top = new Pin(PinType.Undefined, true); //TOP
            var p_bottom = new Pin(PinType.Undefined, true); //BOTTOM
            var p_left = new Pin(PinType.Undefined, true); //LEFT
            var p_right = new Pin(PinType.Undefined, true); //RIGHT

            toff.Id = Id;
            p_top.Id = TopPinId;
            p_bottom.Id = BottomPinId;
            p_left.Id = LeftPinId;
            p_right.Id = RightPinId;

            toff.Children.Add(p_top);
            toff.Children.Add(p_bottom);
            toff.Children.Add(p_left);
            toff.Children.Add(p_right);

            toff.Parent = context;

            p_top.Parent = toff;
            p_bottom.Parent = toff;
            p_left.Parent = toff;
            p_right.Parent = toff;

            children.Add(toff);
            children.Add(p_top);
            children.Add(p_bottom);
            children.Add(p_left);
            children.Add(p_right);

            Elements[Id] = toff;
            Elements[TopPinId] = p_top;
            Elements[BottomPinId] = p_bottom;
            Elements[LeftPinId] = p_left;
            Elements[RightPinId] = p_right;
        }

        private static void AddTimerOn(ref UInt32 Id, ref UInt32 LeftPinId, ref UInt32 RightPinId, ref UInt32 TopPinId, ref UInt32 BottomPinId, ref Single Delay)
        {
            var context = CurrentContext;
            var children = context.Children;

            var ton = new TimerOn(Delay); //TON

            var p_top = new Pin(PinType.Undefined, true); //TOP
            var p_bottom = new Pin(PinType.Undefined, true); //BOTTOM
            var p_left = new Pin(PinType.Undefined, true); //LEFT
            var p_right = new Pin(PinType.Undefined, true); //RIGHT

            ton.Id = Id;
            p_top.Id = TopPinId;
            p_bottom.Id = BottomPinId;
            p_left.Id = LeftPinId;
            p_right.Id = RightPinId;

            ton.Children.Add(p_top);
            ton.Children.Add(p_bottom);
            ton.Children.Add(p_left);
            ton.Children.Add(p_right);

            ton.Parent = context;

            p_top.Parent = ton;
            p_bottom.Parent = ton;
            p_left.Parent = ton;
            p_right.Parent = ton;

            children.Add(ton);
            children.Add(p_top);
            children.Add(p_bottom);
            children.Add(p_left);
            children.Add(p_right);

            Elements[Id] = ton;
            Elements[TopPinId] = p_top;
            Elements[BottomPinId] = p_bottom;
            Elements[LeftPinId] = p_left;
            Elements[RightPinId] = p_right;
        }

        private static void AddOrGate(ref UInt32 Id, ref UInt32 LeftPinId, ref UInt32 RightPinId, ref UInt32 TopPinId, ref UInt32 BottomPinId)
        {
            var context = CurrentContext;
            var children = context.Children;

            var og = new OrGate(); //ORGATE

            var p_top = new Pin(PinType.Undefined, true); //TOP
            var p_bottom = new Pin(PinType.Undefined, true); //BOTTOM
            var p_left = new Pin(PinType.Undefined, true); //LEFT
            var p_right = new Pin(PinType.Undefined, true); //RIGHT

            og.Id = Id;
            p_top.Id = TopPinId;
            p_bottom.Id = BottomPinId;
            p_left.Id = LeftPinId;
            p_right.Id = RightPinId;

            og.Children.Add(p_top);
            og.Children.Add(p_bottom);
            og.Children.Add(p_left);
            og.Children.Add(p_right);

            og.Parent = context;

            p_top.Parent = og;
            p_bottom.Parent = og;
            p_left.Parent = og;
            p_right.Parent = og;

            children.Add(og);
            children.Add(p_top);
            children.Add(p_bottom);
            children.Add(p_left);
            children.Add(p_right);

            Elements[Id] = og;
            Elements[TopPinId] = p_top;
            Elements[BottomPinId] = p_bottom;
            Elements[LeftPinId] = p_left;
            Elements[RightPinId] = p_right;
        }

        private static void AddAndGate(ref UInt32 Id, ref UInt32 LeftPinId, ref UInt32 RightPinId, ref UInt32 TopPinId, ref UInt32 BottomPinId)
        {
            var context = CurrentContext;
            var children = context.Children;

            var ag = new AndGate(); //ANDGATE

            var p_top = new Pin(PinType.Undefined, true); //TOP
            var p_bottom = new Pin(PinType.Undefined, true); //BOTTOM
            var p_left = new Pin(PinType.Undefined, true); //LEFT
            var p_right = new Pin(PinType.Undefined, true); //RIGHT

            ag.Id = Id;
            p_top.Id = TopPinId;
            p_bottom.Id = BottomPinId;
            p_left.Id = LeftPinId;
            p_right.Id = RightPinId;

            ag.Children.Add(p_top);
            ag.Children.Add(p_bottom);
            ag.Children.Add(p_left);
            ag.Children.Add(p_right);

            ag.Parent = context;

            p_top.Parent = ag;
            p_bottom.Parent = ag;
            p_left.Parent = ag;
            p_right.Parent = ag;

            children.Add(ag);
            children.Add(p_top);
            children.Add(p_bottom);
            children.Add(p_left);
            children.Add(p_right);

            Elements[Id] = ag;
            Elements[TopPinId] = p_top;
            Elements[BottomPinId] = p_bottom;
            Elements[LeftPinId] = p_left;
            Elements[RightPinId] = p_right;
        }

        private static void AddSignal(ref UInt32 Id, ref UInt32 InputPinId, ref UInt32 OutputPinId)
        {
            var context = CurrentContext;
            var children = context.Children;

            var signal = new Signal(); //SIGNAL

            var p_input = new Pin(PinType.Input, false); //INPUT
            var p_output = new Pin(PinType.Output, false); //OUTPUT   

            signal.Id = Id;
            p_input.Id = InputPinId;
            p_output.Id = OutputPinId;

            signal.Input = p_input;
            signal.Output = p_output;

            signal.Children.Add(p_input);
            signal.Children.Add(p_output);

            signal.Parent = context;

            p_input.Parent = signal;
            p_output.Parent = signal;

            children.Add(signal);
            children.Add(p_input);
            children.Add(p_output);

            Elements[Id] = signal;
            Elements[InputPinId] = p_input;
            Elements[OutputPinId] = p_output;
        }

        private static void AddPin(ref UInt32 Id)
        {
            var context = CurrentContext;
            var children = context.Children;

            var p = new Pin(PinType.Undefined, true); //PIN

            p.Id = Id;

            p.Parent = context;

            children.Add(p);

            Elements[Id] = p;
        }

        private static void AddContext(ref UInt32 Id)
        {
            var context = new Context();
            Elements[Id] = context;

            CurrentProject.Children.Add(context);
            CurrentContext = context;
            CurrentSolution.CurrentContext = context;
        }

        private static void AddProject(ref UInt32 Id)
        {
            var project = new Project();
            Elements[Id] = project;

            CurrentSolution.Children.Add(project);
            CurrentProject = project;
            CurrentSolution.CurrentProject = project;
        }

        private static void AddSolution(ref UInt32 Id)
        {
            var solution = new Solution();
            Elements[Id] = solution;

            CurrentSolution = solution;
        }

        #endregion
    }

    #endregion
}
