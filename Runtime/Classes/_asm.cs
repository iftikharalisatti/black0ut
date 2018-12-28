// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;
using System.Text;
using System.Collections.Generic;

namespace Black0ut.Runtime
{
    using Log;

    public class _asm
    {
        public byte[][] Files;

        public InstructionHandler[] InstructionHandlers;

        #region .ctor

        public _asm()
        {
            InstructionHandlers = new InstructionHandler[]
            {
                // PROGRAM
                NOOPERATION,
                RETURN,
                EXECUTE,
                READ,
                IF,
                COPY,

                // NOT LINK INSTRUCTION
                _GOTO,
                _SKIP,
                _LOG,

                // ARITHMETIC
                INCREMENT,
                DECREMENT,
                ADD,
                SUBTRACT,
                MULTIPLY,
                DIVIDE,
                REMAINDER,
                SHIFTLEFT,
                SHIFTRIGHT,
                XOR,
                SQRT,
        
                // COMPILER
                //LINK,
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns -1 on file return, else return exception executePoint
        /// </summary>
        public int Execute(byte[] File, byte executePoint = 0)
        {
            try
            {
                while (true)
                {
                    if (File[executePoint] >= InstructionHandlers.Length)
                        throw new Exception($"No instruction '{File[executePoint]}' handler!");

                    if (InstructionHandlers[File[executePoint]](File, ref executePoint))
                        return -1;
                }

                throw new Exception($"Breaked program loop without return!");
            }
            catch (Exception e)
            {
                Log.Show($"FileSize '{File.Length}', ExecutePoint '{executePoint}', Exception {e}", "Execute");
                return executePoint;
            }
        }

        public int Execute(int fileIndex)
        {
            return Execute(Files[fileIndex]);
        }

        public byte[] Compile(string source)
        {
            var index = 0;

            var instructions = source.Replace("\n", " ").Replace("\r", " ").Split(' ');

            try
            {
                checked
                {
                    var compiledInstructions = new List<byte>();

                    var variableLinks = new Dictionary<string, byte>(); // name, index

                    while (index < instructions.Length)
                    {
                        if (string.IsNullOrEmpty(instructions[index]) || instructions[index][0] == '/')
                        {
                            index++;
                            continue;
                        }

                        if (instructions[index].ToUpper() == InstructionType.LINK.ToString())
                        {
                            if (variableLinks.ContainsKey(instructions[++index].ToUpper()))
                                throw new Exception($"Already contains definition for [{instructions[index].ToUpper()}].");

                            variableLinks.Add(instructions[index], (byte)(compiledInstructions.Count));

                            compiledInstructions.Add(byte.Parse(instructions[++index]));

                            index++;

                            continue;
                        }

                        try
                        {
                            compiledInstructions.Add(byte.Parse(instructions[index]));
                        }
                        catch
                        {
                            try
                            {
                                compiledInstructions.Add((byte)Enum.Parse(typeof(InstructionType), instructions[index].ToUpper()));
                            }
                            catch
                            {
                                if (!variableLinks.ContainsKey(instructions[index]))
                                    throw new Exception($"Variable link [{instructions[index]}] not implemented before.");

                                compiledInstructions.Add(variableLinks[instructions[index]]);
                            }
                        }

                        index++;
                    }

                    return compiledInstructions.ToArray();
                }
            }
            catch (Exception e)
            {
                Log.Show($"Failed to compile program. Instruction [{instructions[index]}]. Exception [{e}]", "Compile");

                return null;
            }
        }

        #endregion

        #region InstructionHandlers

        #region Program

        public bool NOOPERATION(byte[] file, ref byte executionPoint)
        {
            executionPoint++;
            return false;
        }

        public bool RETURN(byte[] file, ref byte executionPoint)
        {
            return true;
        }

        /// <summary>
        /// Executes other file
        /// </summary>
        public bool EXECUTE(byte[] file, ref byte executionPoint)
        {
            if (Execute(Files[file[executionPoint + 1]], file[executionPoint + 2]) != -1)
                throw new Exception("Executed file not returned -1 value");
            executionPoint += 3;
            return false;
        }

        /// <summary>
        /// Copy value from other file
        /// </summary>
        public bool READ(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = Files[file[executionPoint + 1]][file[executionPoint + 2]];
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// If branch, goes to executionPoint by result
        /// </summary>
        public bool IF(byte[] file, ref byte executionPoint)
        {
            // Logic unit
            switch ((LogicUnitType)file[executionPoint + 2])
            {
                case LogicUnitType.EQUALS:
                    if (file[file[executionPoint + 1]] == file[file[executionPoint + 3]])
                        goto TRUE;
                    else
                        goto FALSE;
                case LogicUnitType.NOTEQUALS:
                    if (file[file[executionPoint + 1]] != file[file[executionPoint + 3]])
                        goto TRUE;
                    else
                        goto FALSE;
                case LogicUnitType.BIGGER:
                    if (file[file[executionPoint + 1]] > file[file[executionPoint + 3]])
                        goto TRUE;
                    else
                        goto FALSE;
                case LogicUnitType.BIGGEREQUALS:
                    if (file[file[executionPoint + 1]] >= file[file[executionPoint + 3]])
                        goto TRUE;
                    else
                        goto FALSE;
                case LogicUnitType.SMALLER:
                    if (file[file[executionPoint + 1]] < file[file[executionPoint + 3]])
                        goto TRUE;
                    else
                        goto FALSE;
                case LogicUnitType.SMALLEREQUALS:
                    if (file[file[executionPoint + 1]] <= file[file[executionPoint + 3]])
                        goto TRUE;
                    else
                        goto FALSE;
            }

        TRUE:
            executionPoint = file[file[executionPoint + 4]];
        FALSE:
            executionPoint = file[file[executionPoint + 5]];

            executionPoint += 6;
            return false;


            /// MAKE WITH DELEGATES
        }

        /// <summary>
        /// Copy from one link value to other
        /// </summary>
        public bool COPY(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 2]] = file[executionPoint + 1];
            executionPoint += 3;
            return false;
        }

        #endregion

        #region Not Link Instructions

        /// <summary>
        /// Sets executionPoint
        /// </summary>
        public bool _GOTO(byte[] file, ref byte executionPoint)
        {
            executionPoint = file[executionPoint + 1];
            executionPoint += 2;
            return false;
        }

        /// <summary>
        /// Skips specifed instructions count, also skips this instruction size.
        /// </summary>
        public bool _SKIP(byte[] file, ref byte executionPoint)
        {
            executionPoint += (byte)(2 + file[executionPoint + 1]);
            return false;
        }

        /// <summary>
        /// Debug log
        /// </summary>
        public bool _LOG(byte[] file, ref byte executionPoint)
        {
            Log.Show(Encoding.ASCII.GetString(file, executionPoint + 2, file[executionPoint + 1]), "LOG");
            executionPoint += (byte)(2 + file[executionPoint + 1]);
            return false;
        }

        #endregion

        #region Arithmetic

        /// <summary>
        /// ++
        /// </summary>
        public bool INCREMENT(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 1]]++;
            executionPoint += 2;
            return false;
        }

        /// <summary>
        /// --
        /// </summary>
        public bool DECREMENT(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 1]]--;
            executionPoint += 2;
            return false;
        }

        /// <summary>
        /// +
        /// </summary>
        public bool ADD(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = (byte)(file[file[executionPoint + 1]] + file[file[executionPoint + 2]]);
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// -
        /// </summary>
        public bool SUBTRACT(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = (byte)(file[file[executionPoint + 1]] - file[file[executionPoint + 2]]);
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// *
        /// </summary>
        public bool MULTIPLY(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = (byte)(file[file[executionPoint + 1]] * file[file[executionPoint + 2]]);
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// /
        /// </summary>
        public bool DIVIDE(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = (byte)(file[file[executionPoint + 1]] / file[file[executionPoint + 2]]);
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// /
        /// </summary>
        public bool REMAINDER(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = (byte)(file[file[executionPoint + 1]] % file[file[executionPoint + 2]]);
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// <<
        /// </summary>
        public bool SHIFTLEFT(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = (byte)(file[file[executionPoint + 1]] << file[file[executionPoint + 2]]);
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// >>
        /// </summary>
        public bool SHIFTRIGHT(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = (byte)(file[file[executionPoint + 1]] >> file[file[executionPoint + 2]]);
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// ^
        /// </summary>
        public bool XOR(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 3]] = (byte)(file[file[executionPoint + 1]] ^ file[file[executionPoint + 2]]);
            executionPoint += 4;
            return false;
        }

        /// <summary>
        /// sqrt()
        /// </summary>
        public bool SQRT(byte[] file, ref byte executionPoint)
        {
            file[file[executionPoint + 1]] = (byte)Math.Sqrt(file[file[executionPoint + 1]]);
            executionPoint += 2;
            return false;
        }

        #endregion

        #endregion
    }
}