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
            string inputText = "Please open my music player and play the music I like"; // �ִ�
                                                                                        //string inputText = "Hi,Open msg"; // �ִ�
            var sd = Path.Combine(Environment.CurrentDirectory, "Models\\EnglishPOS.nbin");
            var sd1 = Path.Combine(Environment.CurrentDirectory, "Models\\Parser\\tagdict");
            var tokenizer = new EnglishRuleBasedTokenizer(false);
            string[] tokens = tokenizer.Tokenize(inputText); // ���Ա�ע
            var tagger = new EnglishMaximumEntropyPosTagger(sd, sd1);
            string[] tags = tagger.Tag(tokens); // �䷨����
            var parser = new EnglishTreebankParser(Path.Combine(Environment.CurrentDirectory, "Models\\"), true, false);
            var parse = parser.DoParse(string.Join(" ", tokens)); // �����䷨����ʶ���������ͼ
            foreach (var node in parse.GetChildren())
            {
                if (node.IsLeaf)
                {
                    string token = node.Label;
                    //string tag = tags[node.LabelPosition - 1]; if (tag.StartsWith("VB"))
                    //{
                    //    if (token.ToLower() == "open")
                    //    {
                    //        Console.WriteLine("�򿪲���");
                    //    }
                    //    else if (tag.EndsWith("RP"))
                    //    {
                    //        Console.WriteLine("�������");
                    //    }
                    //}
                    //else if (tag.StartsWith("NN"))
                    //{
                    //    Console.WriteLine("�������");
                    //}
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //����Ŀ���ַ���
            string input = txt_Cmd.Text;

            //
            //����������ʽģʽ
            string pattern = @"��\s+(\w+)";
            //��Ŀ���ַ�������ƥ��
            MatchCollection matches = Regex.Matches(input, pattern);
            string cmd = string.Empty;
            //����ƥ����
            foreach (Match match in matches)
            {
                //Console.WriteLine(match.Value); //��ӡƥ�䵽���ı�
                cmd = match.Value;
            }
            if (cmd.StartsWith("��"))
            {
                cmd = cmd.TrimStart("��".ToCharArray());
                var exe = GetInstallLocation1(cmd.Trim());
                if (exe != null)
                {
                    System.Diagnostics.Process.Start(exe); // ��������� Notepad++ ����
                }
            }
        }
        //static void RegexAction()
        //{
        //    // �û�������ı�
        //    string inputText = "��Ҫ�� Chrome������� ����û���";
        //    // ͨ��������ʽƥ���û��������ͺ�Ŀ��
        //    string pattern = @"(?<action>��|���|����)\s+(?<target>.+)";
        //    Match match = Regex.Match(inputText, pattern);
        //    if (match.Success)
        //    {
        //        string action = match.Groups["action"].Value;
        //        string target = match.Groups["target"].Value; // ���ݲ������ͺ�Ŀ��ִ����Ӧ�Ĳ���
        //        switch (action)
        //        {
        //            case "��":
        //                Console.WriteLine("��Ӧ�ó���: " + target);
        //                break;
        //            case "���":
        //                Console.WriteLine("���Ԫ��: " + target);
        //                break;
        //            case "����":
        //                Console.WriteLine("��Ԫ�� " + target + " �������ı�"); break;
        //            default:
        //                Console.WriteLine("�޷�����û�����"); break;
        //        }
        //    }
        //    else { Console.WriteLine("�޷�����û�����"); }
        //    Console.ReadLine();
        //}
        private string GetInstallLocation1(string exeName)
        {
            // ָ��Ҫ���ҵ�Ŀ¼
            string directory = @"C:\Program Files";
            // ������Ŀ¼�µ�������Ŀ¼���ļ�
            foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
            {
                // ����ļ���չ���Ƿ��� .exe
                if (Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = System.IO.Path.GetFileName(file); // ����᷵�� Report.pdf
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
            //if (t.Text == "��")
            //{
            //    MessageBox.Show("Test");
            //    Panel panel= new Panel();
            //}
        }

        //private void NLPMain()
        //{
        //    // ����Stanford CoreNLP�ܵ�
        //    var props = new Properties();

        //    //props.setProperty("annotators", "tokenize, ssplit, pos, lemma, ner, parse, sentiment");
        //    props.setProperty("annotators", "tokenize,ssplit,pos,lemma,ner");
        //    var pipeline = new StanfordCoreNLP(props);

        //    // �û�������ı�
        //    string inputText = "��Chrome�����";

        //    // �����ı�����ȡ��������
        //    var document = new CoreDocument(inputText);
        //    pipeline.annotate(document);
        //    var sentences = document.sentences();
        //    if (sentences.size() > 0)
        //    {
        //        var s = sentences.get(0);
        //        //var tokens = sentences.get(0).tokens();
        //        //var firstToken = tokens.get(0).word().ToLower();
        //        //if (firstToken == "��")
        //        //{
        //        //    var secondToken = tokens.get(1).word().ToLower();
        //        //    if (secondToken.EndsWith("�����"))
        //        //    {
        //        //        Console.WriteLine("�����������");
        //        //    }
        //        //    else if (secondToken.EndsWith("�ĵ�"))
        //        //    {
        //        //        Console.WriteLine("���ĵ�����");
        //        //    }
        //        //    // ���Լ�����������������͵��ж�
        //        //}
        //        //else if (firstToken == "���")
        //        //{
        //        //    Console.WriteLine("�������");
        //        //}
        //        //else if (firstToken == "����")
        //        //{
        //        //    Console.WriteLine("�������");
        //        //}
        //    }
        //}
        //static void MainNLP()
        //{ // �û�������ı�
        //    string inputText = "��Ҫ�������������Ҵ�һ��"; // ��ʼ��OpenNLP���
        //    var sentenceDetector = new EnglishMaximumEntropySentenceDetector(Path.Combine(Environment.CurrentDirectory, "OpenNLPmodels\\EnglishSD.nbin"));
        //    var tokenizer = new EnglishMaximumEntropyTokenizer(Path.Combine(Environment.CurrentDirectory, "OpenNLPmodels\\EnglishTok.nbin"));
        //    var posTagger = new EnglishMaximumEntropyPosTagger(Path.Combine(Environment.CurrentDirectory, "OpenNLPmodels\\EnglishPOS.nbin"));
        //    //var parser = new EnglishTreebankParser(Path.Combine(Environment.CurrentDirectory, "OpenNLPmodels\\EnglishPCFG.ser.gz1")); 
        //    // ����OpenNLP�����������
        //    string[] sentences = sentenceDetector.SentenceDetect(inputText);
        //    if (sentences.Length > 0)
        //    {
        //        string[] tokens = tokenizer.Tokenize(sentences[0]);
        //        string[] tags = posTagger.Tag(tokens);
        //        //var tree = parser.DoParse(tokens);
        //        //var action = GetAction(tree); // ������
        //        //Console.WriteLine($"�������ͣ�{action}");
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
        //                        return "�����������";
        //                    }
        //                    else if (objText.ToLower().Contains("document")) { return "���ĵ�����"; }
        //                }
        //            }
        //            else if (action.ToLower() == "click") { return "�������"; } else if (action.ToLower() == "input") { return "�������"; }
        //        }
        //    }
        //    return null;
        //}
    }
}