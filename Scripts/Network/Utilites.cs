
namespace Network
{
    public class Utilites
    {
        public static uint PushIntLE(byte[] buffer, int val, uint offset){
            byte[] ss = System.BitConverter.GetBytes(val);
            buffer[offset + 0] = ss[0];
            buffer[offset + 1] = ss[1];
            buffer[offset + 2] = ss[2];
            buffer[offset + 3] = ss[3];

            return 4;
        }

        public static uint PushIntBE(byte[] buffer, int val, uint offset){
            byte[] ss = System.BitConverter.GetBytes(val);
            buffer[offset + 0] = ss[3];
            buffer[offset + 1] = ss[2];
            buffer[offset + 2] = ss[1];
            buffer[offset + 3] = ss[0];

            return 4;
        }

        public static uint PushUShortBE(byte[] buffer, ushort val, uint offset){
            byte[] ss = System.BitConverter.GetBytes(val);
            buffer[offset + 0] = ss[1];
            buffer[offset + 1] = ss[0];

            return 2;
        }

        public static uint PushUShortLE(byte[] buffer, ushort val, uint offset){
            return PushByteLE(buffer, System.BitConverter.GetBytes(val), offset);
        }

        public static uint PushByteLE(byte[] buffer, byte[] val, uint offset){
            for (int i = 0; i < val.Length; ++i){
                buffer[offset] = val[i];
                offset++;
            }

            return (uint)val.Length;
        }


        public static ushort Byte2UShortBE(byte[] buffer){
            return (ushort)(buffer[1] | (buffer[0] << 8));
        }

        public static int Byte2IntBE(byte[] buffer){
            return buffer[3] | (buffer[2] << 8) | (buffer[1] << 16) | (buffer[0] << 24);
        }

        public static ushort Byte2UShortLE(byte[] buffer){
            return (ushort)(buffer[0] | (buffer[1] << 8));
        }

        public static int Byte2IntLE(byte[] buffer){
            return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);
        }
    }
}

