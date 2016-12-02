using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicViewer
{
    public class Program
    {
        const int First = 128;
        const int Last =  251;

        static void Main(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                Usage();
                return;
            }
            ViewLevel2BasicFile(args[0], Console.Out);
        }

        private static void ViewLevel2BasicFile(string basicFileName, TextWriter output)
        {
            using (Stream stm1 = File.OpenRead(basicFileName))
            {
                int b = stm1.ReadByte();
                if (b != 0xFF)
                {
                    Console.Error.WriteLine("{0}: expected first byte to be 0xFF", basicFileName);
                    return;
                }
                for (;;)
                {
                    // Line format:
                    // +-------+-------+-     -+------+
                    // | uAddr | line# |  ...  | 0x00 |
                    // +---'---+---'---+-     -+------+
                    // uAddr = UINT16 - LSB - Address of line (ignored)
                    // line# = UINT16 - LSB - Line number
                    // Tokens and text follow, terminated by a zero byte
                    ushort uAddr, lineNo;
                    if (!TryReadLeUInt16(stm1, out uAddr) || uAddr == 0)
                        break;
                    if (!TryReadLeUInt16(stm1, out lineNo))
                        break;
                    output.Write("{0} ", lineNo);
                    ExpandTokens(stm1, output);
                    output.WriteLine();
                }
            }
        }

        private static void ExpandTokens(Stream input, TextWriter output)
        {
            bool linedone = false;
            while (!linedone)
            {
                int b = input.ReadByte();
                if (b < 0)
                    break;
                byte j = (byte)b;
                if (j == 0)
                {
                    linedone = true;
                }
                else
                {
                    if (j >= First && j <= Last)
                    {
                        output.Write(tokens[j - First]);
                    }
                    else
                    {
                        output.Write((char)j);
                    }
                }
            }
        }

        private static void Usage()
        {
            Console.WriteLine("BasicViewer <tokenized-basic-file> [<output-file>]");
        }

        private static bool TryReadLeUInt16(Stream stm, out ushort ui)
        {
            int b = stm.ReadByte();
            int b2 = stm.ReadByte();
            if (b < 0 || b2 < 0)
            {
                ui = 0;
                return false;
            }
            ui = (ushort)((b2 << 8) | b);
            return true;
        }

        private static string[] tokens = new string[]
        {
            "END", "FOR", "RESET", "SET", "CLS", "CMD", "RANDOM", "NEXT",
            "DATA", "INPUT", "DIM", "READ", "LET", "GOTO", "RUN", "IF",
            "RESTORE", "GOSUB", "RETURN", "REM", "STOP", "ELSE", "TRON", "TROFF",
            "DEFSTR", "DEFINT", "DEFSNG", "DEFDBL", "LINE", "EDIT", "ERROR", "RESUME",
            "OUT", "ON", "OPEN", "FIELD", "GET", "PUT", "CLOSE", "LOAD",
            "MERGE", "NAME", "KILL", "LSET", "RSET", "SAVE", "SYSTEM", "LPRINT",
            "DEF", "POKE", "PRINT", "CONT", "LIST", "LLIST", "DELETE", "AUTO",
            "CLEAR", "CLOAD", "CSAVE", "NEW", "TAB(", "TO", "FN", "USING",
            "VARPTR", "USR", "ERL", "ERR", "STRING$", "INSTR", "POINT", "TIME$",
            "MEM", "INKEY$", "THEN", "NOT", "STEP", "+", "-", "*",
            "/", "^", "AND", "OR", ">", "=", "<", "SGN",
            "INT", "ABS", "FRE", "INP", "POS", "SQR", "RND", "LOG",
            "EXP", "COS", "SIN", "TAN", "ATN", "PEEK", "CVI", "CVS",
            "CVD", "EOF", "LOC","LOF", "MKI$", "MKS$", "MKD$", "CINT",
            "CSNG", "CDBL", "FIX", "LEN", "STR$", "VAL", "ASC","CHR$",
            "LEFT$", "RIGHT$", "MID$", "'"
        };
    }
}
