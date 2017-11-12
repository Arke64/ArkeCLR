using ArkeCLR.Utilities;
using ArkeCLR.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ArkeCLR.Runtime.Files {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CliHeader {
        public uint HeaderSize;
        public ushort MajorRuntimeVersion;
        public ushort MinorRuntimeVersion;
        public RvaAndSize Metadata;
        public CliRuntimeFlags Flags;
        public uint EntryPointToken;
        public RvaAndSize Resources;
        public RvaAndSize StrongNameSignature;
        public ulong CodeManagerTable;
        public RvaAndSize VTableFixups;
        public ulong ExportAddressTableJumps;
        public ulong ManagedNativeHeader;
    }

    public struct CilMetadataRootHeader : ICustomByteReader {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct RootHeader1 {
            public uint Signature;
            public ushort MajorVersion;
            public ushort MinorVersion;
            public uint Reserved1;
            public uint VersionLength;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct RootHeader2 {
            public ushort Flags;
            public ushort StreamCount;
        }

        public uint Signature;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint Reserved1;
        public uint VersionLength;
        public string Version;
        public ushort Flags;
        public ushort StreamCount;

        public void Read(ByteReader file) {
            var root1 = file.ReadStruct<RootHeader1>();
            this.Signature = root1.Signature;
            this.MajorVersion = root1.MajorVersion;
            this.MinorVersion = root1.MinorVersion;
            this.Reserved1 = root1.Reserved1;
            this.VersionLength = root1.VersionLength;

            this.Version = file.ReadString(Encoding.UTF8, this.VersionLength);

            var root2 = file.ReadStruct<RootHeader2>();
            this.Flags = root2.Flags;
            this.StreamCount = root2.StreamCount;
        }
    }

    public struct CilMetadataStreamHeader : ICustomByteReader {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct StreamHeader1 {
            public uint Offset;
            public uint Size;
        }

        public uint Offset;
        public uint Size;
        public string Name;

        public void Read(ByteReader file) {
            var header1 = file.ReadStruct<StreamHeader1>();
            this.Offset = header1.Offset;
            this.Size = header1.Size;

            this.Name = file.ReadStringTerminated(Encoding.ASCII, 0, 4);
        }
    }

    public struct CilTableStreamHeader : ICustomByteReader {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct StreamHeader1 {
            public uint Reserved1;
            public byte MajorVersion;
            public byte MinorVersion;
            public byte HeapSizes;
            public byte Reserved2;
            public ulong Valid;
            public ulong Sorted;
        }

        public uint Reserved1;
        public byte MajorVersion;
        public byte MinorVersion;
        public BitVector HeapSizes;
        public byte Reserved2;
        public BitVector Valid;
        public BitVector Sorted;
        public uint[] Rows;

        public void Read(ByteReader file) {
            var header1 = file.ReadStruct<StreamHeader1>();
            this.Reserved1 = header1.Reserved1;
            this.MajorVersion = header1.MajorVersion;
            this.MinorVersion = header1.MinorVersion;
            this.HeapSizes = new BitVector(header1.HeapSizes);
            this.Reserved2 = header1.Reserved2;
            this.Valid = new BitVector(header1.Valid);
            this.Sorted = new BitVector(header1.Sorted);

            var rows = file.ReadArray<uint>(this.Valid.CountSet());
            var i = 0;

            this.Rows = this.Valid.Select(v => v ? rows[i++] : 0).ToArray();
        }
    }

    public struct MethodHeader : ICustomByteReader {
        public MethodFlags Flags;
        public byte Size;
        public ushort MaxStack;
        public uint CodeSize;
        public uint LocalVarSigTok;
        public MethodBody Body;
        public MethodDataSectionHeader[] DataSections;

        public void Read(ByteReader file) {
            if (((MethodFlags)file.PeekU1()).FlagSet(MethodFlags.FatFormat)) {
                var first = file.ReadU2();

                this.Flags = (MethodFlags)((first >> 4) & 0b1111_1111_1111);
                this.Size = (byte)(first & 0b1111);

                file.Read(out this.MaxStack);
                file.Read(out this.CodeSize);
                file.Read(out this.LocalVarSigTok);
            }
            else {
                var first = file.ReadU1();

                this.Flags = (MethodFlags)(first & 0b11);
                this.CodeSize = (first & 0b1111_1100U) >> 2;
                this.MaxStack = 8;
            }

            this.Body = file.ReadByteReader(this.CodeSize).ReadCustom<MethodBody>();

            var sects = new List<MethodDataSectionHeader>();

            if (this.Flags.FlagSet(MethodFlags.MoreSects)) {
                file.SeekToMultiple(4);

                var sect = default(MethodDataSectionHeader);

                do {
                    sects.Add(sect = file.ReadCustom<MethodDataSectionHeader>());
                } while (sect.Kind.FlagSet(MethodDataSectionFlags.MoreSects));
            }

            this.DataSections = sects.ToArray();
        }
    }

    public struct MethodBody : ICustomByteReader {
        public uint[] Offsets;
        public MethodInstruction[] Instructions;

        public void Read(ByteReader file) {
            var offsets = new List<uint>();
            var instructions = new List<MethodInstruction>();

            while (file.Position < file.Length) {
                offsets.Add((uint)file.Position);
                instructions.Add(file.ReadCustom<MethodInstruction>());
            }

            offsets.Add((uint)file.Position);

            this.Offsets = offsets.ToArray();
            this.Instructions = instructions.ToArray();
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MethodInstruction : ICustomByteReader {
        [FieldOffset(0)]
        public int[] SwitchTable;

        [FieldOffset(8)]
        public InstructionType Op;

        [FieldOffset(12)]
        public int BrTarget;
        [FieldOffset(12)]
        public uint Field;
        [FieldOffset(12)]
        public int I;
        [FieldOffset(12)]
        public long I8;
        [FieldOffset(12)]
        public uint Method;
        [FieldOffset(12)]
        public double R;
        [FieldOffset(12)]
        public uint Sig;
        [FieldOffset(12)]
        public uint String;
        [FieldOffset(12)]
        public uint Switch;
        [FieldOffset(12)]
        public uint Tok;
        [FieldOffset(12)]
        public uint Type;
        [FieldOffset(12)]
        public ushort Var;
        [FieldOffset(12)]
        public sbyte ShortBrTarget;
        [FieldOffset(12)]
        public sbyte ShortI;
        [FieldOffset(12)]
        public float ShortR;
        [FieldOffset(12)]
        public byte ShortVar;

        public void Read(ByteReader file) {
            var first = file.ReadU1();

            this.Op = (InstructionType)(first != 0xFE ? first : (0xFE00 | file.ReadU1()));

            switch (this.Op) {
                case InstructionType.beq:
                case InstructionType.bge:
                case InstructionType.bgt:
                case InstructionType.ble:
                case InstructionType.blt:
                case InstructionType.bne_un:
                case InstructionType.bge_un:
                case InstructionType.bgt_un:
                case InstructionType.ble_un:
                case InstructionType.blt_un:
                case InstructionType.br:
                case InstructionType.brfalse:
                case InstructionType.brtrue:
                case InstructionType.leave:
                    file.Read(out this.BrTarget);
                    break;

                case InstructionType.ldfld:
                case InstructionType.ldflda:
                case InstructionType.stfld:
                case InstructionType.ldsfld:
                case InstructionType.ldsflda:
                case InstructionType.stsfld:
                    file.Read(out this.Field);
                    break;

                case InstructionType.ldc_i4:
                    file.Read(out this.I);
                    break;

                case InstructionType.ldc_i8:
                    file.Read(out this.I8);
                    break;

                case InstructionType.callvirt:
                case InstructionType.newobj:
                case InstructionType.ldftn:
                case InstructionType.ldvirtftn:
                case InstructionType.jmp:
                case InstructionType.call:
                    file.Read(out this.Method);
                    break;

                case InstructionType.ldc_r8:
                    file.Read(out this.R);
                    break;

                case InstructionType.calli:
                    file.Read(out this.Sig);
                    break;

                case InstructionType.ldstr:
                    file.Read(out this.String);
                    break;

                case InstructionType.@switch:
                    file.Read(out this.Switch);
                    file.ReadArray(this.Switch, out this.SwitchTable);
                    break;

                case InstructionType.ldtoken:
                    file.Read(out this.Tok);
                    break;

                case InstructionType.initobj:
                case InstructionType.cpobj:
                case InstructionType.ldobj:
                case InstructionType.castclass:
                case InstructionType.isinst:
                case InstructionType.newarr:
                case InstructionType.ldelema:
                case InstructionType.ldelem:
                case InstructionType.stelem:
                case InstructionType.unbox_any:
                case InstructionType.constrained_prefix:
                case InstructionType.@sizeof:
                case InstructionType.unbox:
                case InstructionType.stobj:
                case InstructionType.box:
                case InstructionType.refanyval:
                case InstructionType.mkrefany:
                    file.Read(out this.Type);
                    break;

                case InstructionType.ldarg:
                case InstructionType.ldarga:
                case InstructionType.starg:
                case InstructionType.ldloc:
                case InstructionType.ldloca:
                case InstructionType.stloc:
                    file.Read(out this.Var);
                    break;

                case InstructionType.br_s:
                case InstructionType.brfalse_s:
                case InstructionType.brtrue_s:
                case InstructionType.beq_s:
                case InstructionType.bge_s:
                case InstructionType.bgt_s:
                case InstructionType.ble_s:
                case InstructionType.blt_s:
                case InstructionType.bne_un_s:
                case InstructionType.bge_un_s:
                case InstructionType.bgt_un_s:
                case InstructionType.ble_un_s:
                case InstructionType.blt_un_s:
                case InstructionType.leave_s:
                    file.Read(out this.ShortBrTarget);
                    break;

                case InstructionType.ldc_i4_s:
                case InstructionType.unaligned_prefix:
                    file.Read(out this.ShortI);
                    break;

                case InstructionType.ldc_r4:
                    file.Read(out this.ShortR);
                    break;

                case InstructionType.ldarg_s:
                case InstructionType.ldarga_s:
                case InstructionType.starg_s:
                case InstructionType.ldloc_s:
                case InstructionType.ldloca_s:
                case InstructionType.stloc_s:
                    file.Read(out this.ShortVar);
                    break;
            }
        }
    }

    public struct MethodDataSectionHeader : ICustomByteReader {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TinyExceptionHandlingClause {
            public ushort Flags;
            public ushort TryOffset;
            public byte TryLength;
            public ushort HandlerOffset;
            public byte HandlerLength;
            public uint ClassToken;
            public uint FilterOffset;

            public ExceptionHandlingClause Expand() => new ExceptionHandlingClause {
                Flags = (ExceptionClauseFlags)this.Flags,
                TryOffset = this.TryOffset,
                TryLength = this.TryLength,
                HandlerOffset = this.HandlerOffset,
                HandlerLength = this.HandlerLength,
                ClassToken = this.ClassToken,
                FilterOffset = this.FilterOffset,
            };
        }

        public MethodDataSectionFlags Kind;
        public uint DataSize;
        public ExceptionHandlingClause[] Clauses;

        public void Read(ByteReader file) {
            file.ReadEnum(out this.Kind);
            file.Read(out this.DataSize);

            if (!this.Kind.FlagSet(MethodDataSectionFlags.FatFormat)) {
                this.DataSize >>= 16;

                this.Clauses = file.ReadArray<TinyExceptionHandlingClause>((this.DataSize - 4) / 12).ToArray(c => c.Expand());
            }
            else {
                file.ReadArray((this.DataSize - 4) / 24, out this.Clauses);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ExceptionHandlingClause {
        public ExceptionClauseFlags Flags;
        public uint TryOffset;
        public uint TryLength;
        public uint HandlerOffset;
        public uint HandlerLength;
        public uint ClassToken;
        public uint FilterOffset;
    }

    [Flags]
    public enum CliRuntimeFlags : uint {
        IlOnly = 0x00000001,
        X86Required = 0x00000002,
        StrongNameSigned = 0x00000008,
        NativeEntryPointer = 0x00000010,
        TrackDebugData = 0x00010000,
        Prefer32Bit = 0x00020000,
    }

    [Flags]
    public enum MethodFlags : ushort {
        TinyFormat = 0x02,
        FatFormat = 0x03,
        MoreSects = 0x08,
        InitLocals = 0x10,
    }

    [Flags]
    public enum MethodDataSectionFlags : uint {
        EhTable = 0x01,
        OptIlTable = 0x02,
        FatFormat = 0x40,
        MoreSects = 0x80,
    }

    [Flags]
    public enum ExceptionClauseFlags : uint {
        Exception = 0x00,
        Filter = 0x01,
        Finally = 0x02,
        Fault = 0x04
    }

    public enum InstructionType : ushort {
        nop = 0x00,
        @break = 0x01,
        ldarg_0 = 0x02,
        ldarg_1 = 0x03,
        ldarg_2 = 0x04,
        ldarg_3 = 0x05,
        ldloc_0 = 0x06,
        ldloc_1 = 0x07,
        ldloc_2 = 0x08,
        ldloc_3 = 0x09,
        stloc_0 = 0x0A,
        stloc_1 = 0x0B,
        stloc_2 = 0x0C,
        stloc_3 = 0x0D,
        ldarg_s = 0x0E,
        ldarga_s = 0x0F,
        starg_s = 0x10,
        ldloc_s = 0x11,
        ldloca_s = 0x12,
        stloc_s = 0x13,
        ldnull = 0x14,
        ldc_i4_m1 = 0x15,
        ldc_i4_0 = 0x16,
        ldc_i4_1 = 0x17,
        ldc_i4_2 = 0x18,
        ldc_i4_3 = 0x19,
        ldc_i4_4 = 0x1A,
        ldc_i4_5 = 0x1B,
        ldc_i4_6 = 0x1C,
        ldc_i4_7 = 0x1D,
        ldc_i4_8 = 0x1E,
        ldc_i4_s = 0x1F,
        ldc_i4 = 0x20,
        ldc_i8 = 0x21,
        ldc_r4 = 0x22,
        ldc_r8 = 0x23,
        dup = 0x25,
        pop = 0x26,
        jmp = 0x27,
        call = 0x28,
        calli = 0x29,
        ret = 0x2A,
        br_s = 0x2B,
        brfalse_s = 0x2C,
        brtrue_s = 0x2D,
        beq_s = 0x2E,
        bge_s = 0x2F,
        bgt_s = 0x30,
        ble_s = 0x31,
        blt_s = 0x32,
        bne_un_s = 0x33,
        bge_un_s = 0x34,
        bgt_un_s = 0x35,
        ble_un_s = 0x36,
        blt_un_s = 0x37,
        br = 0x38,
        brfalse = 0x39,
        brtrue = 0x3A,
        beq = 0x3B,
        bge = 0x3C,
        bgt = 0x3D,
        ble = 0x3E,
        blt = 0x3F,
        bne_un = 0x40,
        bge_un = 0x41,
        bgt_un = 0x42,
        ble_un = 0x43,
        blt_un = 0x44,
        @switch = 0x45,
        ldind_i1 = 0x46,
        ldind_u1 = 0x47,
        ldind_i2 = 0x48,
        ldind_u2 = 0x49,
        ldind_i4 = 0x4A,
        ldind_u4 = 0x4B,
        ldind_i8 = 0x4C,
        ldind_i = 0x4D,
        ldind_r4 = 0x4E,
        ldind_r8 = 0x4F,
        ldind_ref = 0x50,
        stind_ref = 0x51,
        stind_i1 = 0x52,
        stind_i2 = 0x53,
        stind_i4 = 0x54,
        stind_i8 = 0x55,
        stind_r4 = 0x56,
        stind_r8 = 0x57,
        add = 0x58,
        sub = 0x59,
        mul = 0x5A,
        div = 0x5B,
        div_un = 0x5C,
        rem = 0x5D,
        rem_un = 0x5E,
        and = 0x5F,
        or = 0x60,
        xor = 0x61,
        shl = 0x62,
        shr = 0x63,
        shr_un = 0x64,
        neg = 0x65,
        not = 0x66,
        conv_i1 = 0x67,
        conv_i2 = 0x68,
        conv_i4 = 0x69,
        conv_i8 = 0x6A,
        conv_r4 = 0x6B,
        conv_r8 = 0x6C,
        conv_u4 = 0x6D,
        conv_u8 = 0x6E,
        callvirt = 0x6F,
        cpobj = 0x70,
        ldobj = 0x71,
        ldstr = 0x72,
        newobj = 0x73,
        castclass = 0x74,
        isinst = 0x75,
        conv_r_un = 0x76,
        unbox = 0x79,
        @throw = 0x7A,
        ldfld = 0x7B,
        ldflda = 0x7C,
        stfld = 0x7D,
        ldsfld = 0x7E,
        ldsflda = 0x7F,
        stsfld = 0x80,
        stobj = 0x81,
        conv_ovf_i1_un = 0x82,
        conv_ovf_i2_un = 0x83,
        conv_ovf_i4_un = 0x84,
        conv_ovf_i8_un = 0x85,
        conv_ovf_u1_un = 0x86,
        conv_ovf_u2_un = 0x87,
        conv_ovf_u4_un = 0x88,
        conv_ovf_u8_un = 0x89,
        conv_ovf_i_un = 0x8A,
        conv_ovf_u_un = 0x8B,
        box = 0x8C,
        newarr = 0x8D,
        ldlen = 0x8E,
        ldelema = 0x8F,
        ldelem_i1 = 0x90,
        ldelem_u1 = 0x91,
        ldelem_i2 = 0x92,
        ldelem_u2 = 0x93,
        ldelem_i4 = 0x94,
        ldelem_u4 = 0x95,
        ldelem_i8 = 0x96,
        ldelem_i = 0x97,
        ldelem_r4 = 0x98,
        ldelem_r8 = 0x99,
        ldelem_ref = 0x9A,
        stelem_i = 0x9B,
        stelem_i1 = 0x9C,
        stelem_i2 = 0x9D,
        stelem_i4 = 0x9E,
        stelem_i8 = 0x9F,
        stelem_r4 = 0xA0,
        stelem_r8 = 0xA1,
        stelem_ref = 0xA2,
        ldelem = 0xA3,
        stelem = 0xA4,
        unbox_any = 0xA5,
        conv_ovf_i1 = 0xB3,
        conv_ovf_u1 = 0xB4,
        conv_ovf_i2 = 0xB5,
        conv_ovf_u2 = 0xB6,
        conv_ovf_i4 = 0xB7,
        conv_ovf_u4 = 0xB8,
        conv_ovf_i8 = 0xB9,
        conv_ovf_u8 = 0xBA,
        refanyval = 0xC2,
        ckfinite = 0xC3,
        mkrefany = 0xC6,
        ldtoken = 0xD0,
        conv_u2 = 0xD1,
        conv_u1 = 0xD2,
        conv_i = 0xD3,
        conv_ovf_i = 0xD4,
        conv_ovf_u = 0xD5,
        add_ovf = 0xD6,
        add_ovf_un = 0xD7,
        mul_ovf = 0xD8,
        mul_ovf_un = 0xD9,
        sub_ovf = 0xDA,
        sub_ovf_un = 0xDB,
        endfinally = 0xDC,
        leave = 0xDD,
        leave_s = 0xDE,
        stind_i = 0xDF,
        conv_u = 0xE0,
        arglist = 0xFE_00,
        ceq = 0xFE_01,
        cgt = 0xFE_02,
        cgt_un = 0xFE_03,
        clt = 0xFE_04,
        clt_un = 0xFE_05,
        ldftn = 0xFE_06,
        ldvirtftn = 0xFE_07,
        ldarg = 0xFE_09,
        ldarga = 0xFE_0A,
        starg = 0xFE_0B,
        ldloc = 0xFE_0C,
        ldloca = 0xFE_0D,
        stloc = 0xFE_0E,
        localloc = 0xFE_0F,
        endfilter = 0xFE_11,
        unaligned_prefix = 0xFE_12,
        volatile_prefix = 0xFE_13,
        tail_prefix = 0xFE_14,
        initobj = 0xFE_15,
        constrained_prefix = 0xFE_16,
        cpblk = 0xFE_17,
        initblk = 0xFE_18,
        no_prefix = 0xFE_19,
        rethrow = 0xFE_1A,
        @sizeof = 0xFE_1C,
        refanytype = 0xFE_1D,
        readonly_prefix = 0xFE_1E,
    }
}
