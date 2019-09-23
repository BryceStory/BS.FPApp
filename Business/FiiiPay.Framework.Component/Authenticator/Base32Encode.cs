using System.Text;

namespace FiiiPay.Framework.Component.Authenticator
{
    public class Base32Encode
    {
        public static string Encode(string code)
        {
            return Encode(Encoding.UTF8.GetBytes(code));
        }
        public static string Encode(byte[] data)
        {
            int inByteSize = 8;
            int outByteSize = 5;
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

            int i = 0, index = 0, digit = 0;
            int current_byte, next_byte;
            StringBuilder result = new StringBuilder((data.Length + 7) * inByteSize / outByteSize);

            while (i < data.Length)
            {
                current_byte = (data[i] >= 0) ? data[i] : (data[i] + 256); // Unsign

                /* Is the current digit going to span a byte boundary? */
                if (index > (inByteSize - outByteSize))
                {
                    if ((i + 1) < data.Length)
                        next_byte = (data[i + 1] >= 0) ? data[i + 1] : (data[i + 1] + 256);
                    else
                        next_byte = 0;

                    digit = current_byte & (0xFF >> index);
                    index = (index + outByteSize) % inByteSize;
                    digit <<= index;
                    digit |= next_byte >> (inByteSize - index);
                    i++;
                }
                else
                {
                    digit = (current_byte >> (inByteSize - (index + outByteSize))) & 0x1F;
                    index = (index + outByteSize) % inByteSize;
                    if (index == 0)
                        i++;
                }
                result.Append(alphabet[digit]);
            }

            return result.ToString();
        }
        public static string Decode(string encodeString)
        {
            int i;
            int index;
            int lookup;
            int offset;
            int digit;
            string en_string = encodeString.ToUpper();
            int[] BASE32LOOOKUP = new int[]{
               0xFF, 0xFF, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, // '0', '1', '2', '3', '4', '5', '6', '7'
               0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // '8', '9', ':', ';', '<', '=', '>', '?'
               0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, // '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G'
               0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, // 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O'
               0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, // 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W'
               0x17, 0x18, 0x19, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 'X', 'Y', 'Z', '[', '\', ']', '^', '_'
               0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, // '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g'
               0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, // 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o'
               0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, // 'p', 'q', 'r', 's', 't', 'u', 'v', 'w'
               0x17, 0x18, 0x19, 0xFF, 0xFF, 0xFF, 0xFF,0xFF
               };
            int stringLen = ((en_string.Length * 5) / 8);
            int[] bytes = new int[stringLen];
            for (var a = 0; a < stringLen; a++)
            {
                bytes[a] = 0;
            }
            for (i = 0, index = 0, offset = 0; i < en_string.Length; i++)
            {
                var charCode0 = (short)'0';
                lookup = (short)en_string[i] - charCode0;
                if ((lookup < 0) || (lookup >= BASE32LOOOKUP.Length))
                {
                    continue;
                }
                digit = BASE32LOOOKUP[lookup];
                if (digit == 0xFF)
                {
                    continue;
                }
                if (index <= 3)
                {
                    index = (index + 5) % 8;
                    if (index == 0)
                    {
                        bytes[offset] = bytes[offset] | digit;
                        offset++;
                        if (offset >= bytes.Length)
                        {
                            break;
                        }
                    }
                    else
                    {
                        bytes[offset] = bytes[offset] | (digit << (8 - index));
                    }
                }
                else
                {
                    index = (index + 5) % 8;
                    bytes[offset] = bytes[offset] | (digit >> index);
                    offset++;
                    if (offset >= bytes.Length)
                    {
                        break;
                    }
                    bytes[offset] = bytes[offset] | (digit << (8 - index));
                    if (bytes[offset] >= 256)
                    {
                        bytes[offset] %= 256;
                    }
                }
            }
            string realkeyString = "";
            for (var a = 0; a < bytes.Length; a++)
            {
                var realkey = (char)bytes[a];
                realkeyString += realkey;
            }
            return realkeyString;
        }
    }
}
