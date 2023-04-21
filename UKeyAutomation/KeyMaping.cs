using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public static class KeyMaping
    {
        private static Dictionary<string, int> _keyNames = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        private static Dictionary<int, string> KeyList = new Dictionary<int, string>()
        {
            {100 , "ESC"},
            {101 , "F1"},
            {102, "F2"},
            {103 , "F3"},
            {104 , "F4"},
            {105 , "F5"},
            {106 , "F6"},
            {107 , "F7"},
            {108 , "F8"},
            {109, "F9"},
            {110, "F10"},
            {111, "F11"},
            {112, "F12"},

            {200 , "`"},
            {201 , "1"},
            {202, "2"},
            {203 , "3"},
            {204 , "4"},
            {205 , "5"},
            {206 , "6"},
            {207 , "7"},
            {208 , "8"},
            {209, "9"},
            {210, "0"},
            {211, "-"},
            {212, "="},
            {213, "\\"},
            {214, "Backspace"},

            {300 , "Tab"},
            {301 , "q"},
            {302, "w"},
            {303 , "e"},
            {304 , "r"},
            {305 , "t"},
            {306 , "y"},
            {307 , "u"},
            {308 , "i"},
            {309, "o"},
            {310, "p"},
            {311, "["},
            {312, "]"},
            {313, "Enter"},

            {400 , "Capslock"},
            {401 , "a"},
            {402, "s"},
            {403 , "d"},
            {404 , "f"},
            {405 , "g"},
            {406 , "h"},
            {407 , "j"},
            {408 , "k"},
            {409, "l"},
            {410, ";"},
            {411, "'"},

            {500 , "LShift"},
            {501 , "z"},
            {502, "x"},
            {503 , "c"},
            {504 , "v"},
            {505 , "b"},
            {506 , "n"},
            {507 , "m"},
            {508 , ","},
            {509, "."},
            {510, "/"},
            {511, "RShift"},

            {600 , "LCtrl"},
            {601 , "LWin"},
            {602, "LAlt"},
            {603 , "Space"},
            {604 , "RAlt"},
            {605 , "RWin"},
            {606 , "Menu"},
            {607 , "RCtrl"},

            {700 , "Print"},
            {701 , "ScrollLock"},
            {702, "Pause"},
            {703 , "Insert"},
            {704 , "Home"},
            {705 , "PageUp"},
            {706 , "Delete"},
            {707 , "End"},
            {708 , "Down"},
            {709, "AUp"},
            {710, "ALeft"},
            {711, "ADown"},
            {712, "ARight"},

            {700 , "Print"},
            {701 , "ScrollLock"},
            {702, "Pause"},
            {703 , "Insert"},
            {704 , "Home"},
            {705 , "PageUp"},
            {706 , "Delete"},
            {707 , "End"},
            {708 , "Down"},
            {709, "AUp"},
            {710, "ALeft"},
            {711, "ADown"},
            {712, "ARight"},

            {800 , "Num_0"},
            {801 , "Num_1"},
            {802,  "Num_2"},
            {803 , "Num_3"},
            {804 , "Num_4"},
            {805 , "Num_5"},
            {806 , "Num_6"},
            {807 , "Num_7"},
            {808 , "Num_8"},
            {809,  "Num_9"},
            {810,  "Num"},
            {811,  "Num_/"},
            {812, "Num_*"},
            {813, "Num_-"},
            {814, "Num_+"},
            {815, "Num_Enter"},
            {816, "Num_."}
        };
        static KeyMaping()
        {
            foreach (var key in KeyList)
            {
                AddKey((int)key.Key, key.Value);
            }
        }
        private static void AddKey(int code, string name)
        {
            _keyNames.Add(name, code);
        }
    }
}
