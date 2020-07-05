using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace 古典密码加解密
{

    public partial class MainWindow : Window //前端窗口代码部分
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //正则表达式规则
        string AllowedKeys = @"[1-9]\d*"; //允许密钥为正整数
        string AllowedTexts = @"[A-Za-z ]+";//允许明文、密文、密钥为英文字母
        string AllowedNums = @"1|3|5|7|9|11|15|17|19|21|23|25";//允许乘法密钥为26的互质数
        private void CaesarEncryptBtn_Click(object sender, RoutedEventArgs e)//凯撒加密
        {
            if (Regex.IsMatch(CaesarKey.Text, AllowedKeys) && Regex.IsMatch(CaesarPlain.Text, AllowedTexts))//首先前端用正则表达式检查输入是否符合规范（之后相似部分不注释）
                CaesarEncrypted.Text = Caesar.Encrypt(CaesarPlain.Text, Convert.ToInt32(CaesarKey.Text));//执行加密函数
            else
                MessageBox.Show("偏移量必须是正整数、原文或密文必须由字母和空格组成", "输入的数值有误");//输入不符合规范时提示
        }
        private void CaesarDecryptBtn_Click(object sender, RoutedEventArgs e)//凯撒解密
        {
            if (Regex.IsMatch(CaesarKey.Text, AllowedKeys) && Regex.IsMatch(CaesarEncrypted.Text, AllowedTexts))
                CaesarPlain.Text = Caesar.Decrypt(CaesarEncrypted.Text, Convert.ToInt32(CaesarKey.Text));
            else
                MessageBox.Show("偏移量必须是正整数、原文或密文必须由字母和空格组成", "输入的数值有误");
        }
        private void ForceDecryptBtn_Click(object sender, RoutedEventArgs e)
        {
            string str = "";
            if (Regex.IsMatch(CaesarEncrypted.Text, AllowedTexts))
            {
                for (int i = 1; i < 27; i++)
                {
                    str = str + i + " --> " + Caesar.Decrypt(CaesarEncrypted.Text, i) + Environment.NewLine;//重复执行解密26次，每次偏移量加1，将结果依次输出到原文框
                }
                CaesarPlain.Text = str;
            }
            else
                MessageBox.Show("密文必须由字母和空格组成", "输入的数值有误");
        }//凯撒穷举
        private void VigenereEncryptBtn_Click(object sender, RoutedEventArgs e)//弗吉尼亚加密
        {
            if (Regex.IsMatch(VigenereKey.Text, AllowedTexts) && Regex.IsMatch(VigenerePlain.Text, AllowedTexts))
                VigenereEncrypted.Text = Vigenere.Encrypt(VigenerePlain.Text, VigenereKey.Text);
            else
                MessageBox.Show("原文、密文、密钥必须由字母和空格组成", "输入的数值有误");
        }
        private void VigenereDecryptBtn_Click(object sender, RoutedEventArgs e)//弗吉尼亚解密
        {
            if (Regex.IsMatch(VigenereKey.Text, AllowedTexts) && Regex.IsMatch(VigenereEncrypted.Text, AllowedTexts))
                VigenerePlain.Text = Vigenere.Decrypt(VigenereEncrypted.Text, VigenereKey.Text);
            else
                MessageBox.Show("原文、密文、密钥必须由字母和空格组成", "输入的数值有误");
        }
        private void AffineEncryptBtn_Click(object sender, RoutedEventArgs e)//仿射加密
        {
            if (Regex.IsMatch(AffineKeyA.Text, AllowedNums) && Regex.IsMatch(AffineKeyB.Text, AllowedKeys) && Regex.IsMatch(AffinePlain.Text, AllowedTexts))
                AffineEncrypted.Text = Affine.Encrypt(AffinePlain.Text, Convert.ToInt32(AffineKeyA.Text), Convert.ToInt32(AffineKeyB.Text));
            else
                MessageBox.Show("乘法密钥需为26的互质数，原文或密文必须由字母和空格组成", "输入的数值有误");
        }
        private void AffineDecryptBtn_Click(object sender, RoutedEventArgs e)//仿射解密
        {
            if (Regex.IsMatch(AffineKeyA.Text, AllowedNums) && Regex.IsMatch(AffineKeyB.Text, AllowedKeys) && Regex.IsMatch(AffineEncrypted.Text, AllowedTexts))
                AffinePlain.Text = Affine.Decrypt(AffineEncrypted.Text, Convert.ToInt32(AffineKeyA.Text), Convert.ToInt32(AffineKeyB.Text));
            else
                MessageBox.Show("乘法密钥需为26的互质数，原文或密文必须由字母和空格组成", "输入的数值有误");
        }
    }
    class Caesar //凯撒密码算法部分
    {
        public static string Encrypt(string input, int key)//凯撒加密函数，输入【输入值（原、密文）、密钥】
        {
            string output = string.Empty;
            input = input.ToUpper();//将输入转换为大写，方便后续运算
            foreach (char ch in input)//循环对输入字符串中每一个字符进行计算
            {
                if (ch < 65) continue;//跳过非大写字母范围内的字符
                output += Convert.ToChar((((ch + key) - 65) % 26) + 65);//每个字符直接偏移【偏移量的数值】，模26，运算每一个字符计算后的结果，转换回ASCII代码并循环输出
            }
            return output;
        }
        public static string Decrypt(string input, int key)//凯撒解密函数
        {
            string output = string.Empty;
            input = input.ToUpper();
            foreach (char ch in input)
            {
                if (ch < 65) continue;
                output += Convert.ToChar((((ch + 26 - key) - 65) % 26) + 65);
            }
            return output;
        }
    }
    class Vigenere //弗吉尼亚密码算法部分
    {
        public static string Encrypt(string input, string key)//弗吉尼亚加密函数
        {
            string output = string.Empty;
            int keyi = 0, tmp;//keyi:密钥的下标（位数）、tmp:当前处理的字符
            input = input.ToUpper();
            key = key.ToUpper();
            foreach (char ch in input)
            {
                if (ch < 65)
                    continue;
                tmp = ch - 65 + (key[keyi] - 65);
                if (tmp < 0)
                    tmp += 26;
                output += Convert.ToChar((tmp % 26) + 65);
                if (++keyi == key.Length)//通过密钥位数下标控制的循环，每重复用完一轮密钥所有位后，重新开始新一轮
                    keyi = 0;
            }
            return output;
        }
        public static string Decrypt(string input, string key)//弗吉尼亚解密函数
        {
            string output = string.Empty;
            int keyi = 0, tmp;
            input = input.ToUpper();
            key = key.ToUpper();
            foreach (char t in input)
            {
                if (t < 65)
                    continue;
                tmp = t - 65 - (key[keyi] - 65);
                if (tmp < 0)
                    tmp += 26;
                output += Convert.ToChar((tmp % 26) + 65);
                if (++keyi == key.Length)
                    keyi = 0;
            }
            return output;
        }
    }
    class Affine //仿射密码算法部分
    {
        public static string Encrypt(string input, int a, int b)//仿射加密函数，a、b分别为乘法密钥、加法密钥
        {
            string output = string.Empty;
            input = input.ToUpper();
            foreach (char ch in input)
            {
                if (ch < 65)
                    continue;
                output += Convert.ToChar(((a * (ch - 65) + b) % 26) + 65); //进行加密运算
            }
            return output;
        }
        public static int MultiplicativeInverse(int a) //求乘法逆元函数
        {
            for (int x = 1; x < 27; x++) //使用穷举法求乘法逆元
            {
                if ((a * x) % 26 == 1)
                    return x;
            }
            throw new Exception("No multiplicative inverse found!");//找不到时抛出异常以免程序崩溃
        }
        public static string Decrypt(string input, int a, int b) //仿射解密函数
        {
            string output = string.Empty;
            input = input.ToUpper();
            int aInverse = MultiplicativeInverse(a); //求a的乘法逆元
            foreach (char ch in input)
            {
                int x = ch - 65;
                if (x - b < 0)
                    x += 26;
                output += Convert.ToChar(((aInverse * (x - b)) % 26) + 65); //进行运算
            }
            return output;
        }
    }
}