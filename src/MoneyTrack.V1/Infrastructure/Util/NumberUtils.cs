using System.Text;

namespace CloudyWing.MoneyTrack.Infrastructure.Util {
    /// <summary>
    /// A utility class for working with numbers, providing various helper methods.
    /// </summary>
    public static class NumberUtils {
        /// <summary>
        /// Returns the Chinese representation of the given integer number.
        /// </summary>
        /// <param name="number">The number to convert to Chinese.</param>
        /// <returns>A string representing the Chinese equivalent of the given number.</returns>
        public static string GetChineseNumber(int number) {
            string[] chineseNumber = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            string[] unit = { "", "十", "百", "千", "萬", "十萬", "百萬", "千萬", "億", "十億", "百億", "千億", "兆", "十兆", "百兆", "千兆" };
            StringBuilder ret = new StringBuilder();
            string inputNumber = number.ToString();
            int idx = inputNumber.Length;
            bool needAppendZero = false;
            foreach (char c in inputNumber) {
                idx--;
                if (c > '0') {
                    if (needAppendZero) {

                        ret.Append(chineseNumber[0]);
                        needAppendZero = false;

                    }

                    ret.Append(chineseNumber[c - '0'] + unit[idx]);

                } else {

                    needAppendZero = true;
                }

            }
            return ret.Length == 0 ? chineseNumber[0] : ret.ToString();

        }
    }
}
