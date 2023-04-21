﻿using System;
using System.Linq;
using System.Text;
using EL.WindowsAPI;

namespace EL
{
    public static class AccessibilityTextResolver
    {
        public static string GetRoleText(AccessibilityRole role)
        {
            var sb = new StringBuilder(1024);
            Oleacc.GetRoleText(role, sb, 1024);
            return sb.ToString();
        }

        public static string GetStateBitText(AccessibilityState state)
        {
            var sb = new StringBuilder(1024);
            Oleacc.GetStateText(state, sb, 1024);
            return sb.ToString();
        }

        public static string GetStateText(AccessibilityState state)
        {
            var allStates = state.GetFlags();
            return String.Join(", ", allStates.Select(s => GetStateBitText((AccessibilityState)s)).ToArray());
        }
    }
}
