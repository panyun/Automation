using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;

namespace EL.ComandControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //  MainNLP();
        }
        static void MainNLP()
        {
            string inputText = "Please open my music player and play the music I like"; // 分词
                                                                                        //string inputText = "Hi,Open msg"; // 分词
            var sd = Path.Combine(Environment.CurrentDirectory, "Models\\EnglishPOS.nbin");
            var sd1 = Path.Combine(Environment.CurrentDirectory, "Models\\Parser\\tagdict");
            var tokenizer = new EnglishRuleBasedTokenizer(false);
            string[] tokens = tokenizer.Tokenize(inputText); // 词性标注
            var tagger = new EnglishMaximumEntropyPosTagger(sd, sd1);
            string[] tags = tagger.Tag(tokens); // 句法分析
            var parser = new EnglishTreebankParser(Path.Combine(Environment.CurrentDirectory, "Models\\"), true, false);
            var parse = parser.DoParse(string.Join(" ", tokens)); // 遍历句法树，识别操作和意图
            foreach (var node in parse.GetChildren())
            {
                if (node.IsLeaf)
                {
                    string token = node.Label;
                    //string tag = tags[node.LabelPosition - 1]; if (tag.StartsWith("VB"))
                    //{
                    //    if (token.ToLower() == "open")
                    //    {
                    //        Console.WriteLine("打开操作");
                    //    }
                    //    else if (tag.EndsWith("RP"))
                    //    {
                    //        Console.WriteLine("点击操作");
                    //    }
                    //}
                    //else if (tag.StartsWith("NN"))
                    //{
                    //    Console.WriteLine("输入操作");
                    //}
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //定义目标字符串
            string input = txt_Cmd.Text;

            //
            //定义正则表达式模式
            string pattern = @"打开\s+(\w+)";
            //对目标字符串进行匹配
            MatchCollection matches = Regex.Matches(input, pattern);
            string cmd = string.Empty;
            //遍历匹配结果
            foreach (Match match in matches)
            {
                //Console.WriteLine(match.Value); //打印匹配到的文本
                cmd = match.Value;
            }
            if (cmd.StartsWith("打开"))
            {
                cmd = cmd.TrimStart("打开".ToCharArray());
                var exe = GetInstallLocation1(cmd.Trim());
                if (exe != null)
                {
                    System.Diagnostics.Process.Start(exe); // 这里会启动 Notepad++ 程序
                }
            }
        }
        //static void RegexAction()
        //{
        //    // 用户输入的文本
        //    string inputText = "我要打开 Chrome浏览器并 点击用户框";
        //    // 通过正则表达式匹配用户操作类型和目标
        //    string pattern = @"(?<action>打开|点击|输入)\s+(?<target>.+)";
        //    Match match = Regex.Match(inputText, pattern);
        //    if (match.Success)
        //    {
        //        string action = match.Groups["action"].Value;
        //        string target = match.Groups["target"].Value; // 根据操作类型和目标执行相应的操作
        //        switch (action)
        //        {
        //            case "打开":
        //                Console.WriteLine("打开应用程序: " + target);
        //                break;
        //            case "点击":
        //                Console.WriteLine("点击元素: " + target);
        //                break;
        //            case "输入":
        //                Console.WriteLine("在元素 " + target + " 中输入文本"); break;
        //            default:
        //                Console.WriteLine("无法理解用户输入"); break;
        //        }
        //    }
        //    else { Console.WriteLine("无法理解用户输入"); }
        //    Console.ReadLine();
        //}
        private string GetInstallLocation1(string exeName)
        {
            // 指定要查找的目录
            string directory = @"C:\Program Files";
            // 遍历该目录下的所有子目录和文件
            foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
            {
                // 检查文件扩展名是否是 .exe
                if (Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = System.IO.Path.GetFileName(file); // 这里会返回 Report.pdf
                    if (fileName.Contains(exeName))
                    {
                        return file;
                    }

                    Console.WriteLine(file + " is an executable file by extension.");
                }
            }
            return default;
        }
        private string GetInstallLocation(string exeName)
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        string displayName = subkey.GetValue("DisplayName") as string;
                        var installLocation = subkey.GetValue("InstallLocation") as string;
                        Debug.WriteLine("{0} : {1}", displayName, installLocation);
                        //if (displayName != null && displayName.Contains(exeName.Trim()))
                        //{

                        //    Console.WriteLine("{0} : {1}", displayName, installLocation);
                        //    //return subkey.GetValue("InstallLocation") as string;
                        //}

                        //
                    }
                }
            }
            return default;
        }

        private void txt_Cmd_TextChanged(object sender, EventArgs e)
        {
            //var t = sender as TextBox;
            //if (t.Text == "打开")
            //{
            //    MessageBox.Show("Test");
            //    Panel panel= new Panel();
            //}
        }

        //private void NLPMain()
        //{
        //    // 创建Stanford CoreNLP管道
        //    var props = new Properties();

        //    //props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, sentiment");
        //    props.setProperty("annotators", "tokenize,ssplit,pos,lemma,ner");
        //    var pipeline = new StanfordCoreNLP(props);

        //    // 用户输入的文本
        //    string inputText = "打开Chrome浏览器";

        //    // 分析文本并提取操作类型
        //    var document = new CoreDocument(inputText);
        //    pipeline.annotate(document);
        //    var sentences = document.sentences();
        //    if (sentences.size() > 0)
        //    {
        //        var s = sentences.get(0);
        //        //var tokens = sentences.get(0).tokens();
        //        //var firstToken = tokens.get(0).word().ToLower();
        //        //if (firstToken == "打开")
        //        //{
        //        //    var secondToken = tokens.get(1).word().ToLower();
        //        //    if (secondToken.EndsWith("浏览器"))
        //        //    {
        //        //        Console.WriteLine("打开浏览器操作");
        //        //    }
        //        //    else if (secondToken.EndsWith("文档"))
        //        //    {
        //        //        Console.WriteLine("打开文档操作");
        //        //    }
        //        //    // 可以继续添加其他操作类型的判断
        //        //}
        //        //else if (firstToken == "点击")
        //        //{
        //        //    Console.WriteLine("点击操作");
        //        //}
        //        //else if (firstToken == "输入")
        //        //{
        //        //    Console.WriteLine("输入操作");
        //        //}
        //    }
        //}
        //static void MainNLP()
        //{ // 用户输入的文本
        //    string inputText = "我要打开浏览器，你帮我打开一下"; // 初始化OpenNLP组件
        //    var sentenceDetector = new EnglishMaximumEntropySentenceDetector(Path.Combine(Environment.CurrentDirectory, "OpenNLPmodels\\EnglishSD.nbin"));
        //    var tokenizer = new EnglishMaximumEntropyTokenizer(Path.Combine(Environment.CurrentDirectory, "OpenNLPmodels\\EnglishTok.nbin"));
        //    var posTagger = new EnglishMaximumEntropyPosTagger(Path.Combine(Environment.CurrentDirectory, "OpenNLPmodels\\EnglishPOS.nbin"));
        //    //var parser = new EnglishTreebankParser(Path.Combine(Environment.CurrentDirectory, "OpenNLPmodels\\EnglishPCFG.ser.gz1")); 
        //    // 利用OpenNLP进行语义分析
        //    string[] sentences = sentenceDetector.SentenceDetect(inputText);
        //    if (sentences.Length > 0)
        //    {
        //        string[] tokens = tokenizer.Tokenize(sentences[0]);
        //        string[] tags = posTagger.Tag(tokens);
        //        //var tree = parser.DoParse(tokens);
        //        //var action = GetAction(tree); // 输出结果
        //        //Console.WriteLine($"操作类型：{action}");
        //    }
        //}
        //static string GetAction(Parse p)
        //{
        //    if (p.Type.StartsWith("S"))
        //    {
        //        foreach (var child in p.GetChildren())
        //        {
        //            var action = GetAction(child); if (!string.IsNullOrEmpty(action))
        //            { return action; }
        //        }
        //    }
        //    else if (p.Type.StartsWith("VP"))
        //    {
        //        var verb = p.GetChildren().FirstOrDefault(x => x.Type.StartsWith("VB"));
        //        if (verb != null)
        //        {
        //            var action = verb.Text;
        //            if (action.ToLower() == "open")
        //            {
        //                var obj = p.GetChildren().FirstOrDefault(x => x.Type.StartsWith("NP"));
        //                if (obj != null)
        //                {
        //                    var objText = obj.Text;
        //                    if (objText.ToLower().Contains("browser"))
        //                    {
        //                        return "打开浏览器操作";
        //                    }
        //                    else if (objText.ToLower().Contains("document")) { return "打开文档操作"; }
        //                }
        //            }
        //            else if (action.ToLower() == "click") { return "点击操作"; } else if (action.ToLower() == "input") { return "输入操作"; }
        //        }
        //    }
        //    return null;
        //}
    }
}