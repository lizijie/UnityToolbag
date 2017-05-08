#if UNITY_5
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class TestCase {

    [Test]
    public void TestCaseFormatColor()
    {
        ColorPresets.Reflash();
        int count = ColorPresets.Count();
        string[] keys = ColorPresets.GetColorKeys();
        if (keys == null)
            return;

        string format = "[#{0}]asdfsdfsdf111[-]";
        string format2 = "[{0}]asdfsdfsdf111[-]";
        for (int i = 0; i < count; ++i)
        {
            string key = keys[i];
            string num = key.Remove(0, ColorPresets.COLOR_PREFIX.Length);
            string value = ColorPresets.GetHexString(key);

            string lhs = ColorPresets.FormatText(string.Format(format, num));
            string rhs = string.Format(format2, value);
            Assert.AreEqual(lhs, rhs);
        }
    }

    [Test]
    public void TestCaseColorToHexString()
    {
        Color32 rgb = new Color32(134, 234, 123, 252);
        string str = ColorPresets.ColorToHexString(rgb);
        Color32 rgb2 = ColorPresets.HexStringToColor(str);
        Assert.AreEqual(rgb, rgb2);
    }
}
#endif
