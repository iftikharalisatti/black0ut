// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;

namespace Black0ut.Runtime
{
    using Log;

    public class Casm
    {
        public byte[][] Files;

        public InstructionHandler[] InstructionHandlers;

        public Log Log;

        public Casm()
        {
            InstructionHandlers = new InstructionHandler[]
            {
                NOOPERATION,
                EXECUTE,
                READ,
                GOTO,
                IF,
                COPY,
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
            };
        }

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

        #region InstructionHandlers

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
        /// Sets executionPoint
        /// </summary>
        public bool GOTO(byte[] file, ref byte executionPoint)
        {
            executionPoint = file[file[executionPoint + 1]];
            executionPoint += 2;
            return false;
        }

        /// <summary>
        /// If branch, goes to executionPoint by result
        /// </summary>
        public bool IF(byte[] file, ref byte executionPoint)
        {
            // Logic unit
            switch((LogicUnitType)file[executionPoint + 2])
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

        #endregion
    }
}