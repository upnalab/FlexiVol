using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEditor;

public static class Windows
{
    private static long MB_OK = 0x00000000L;
	private static int IDOK = 0;
	// private static long MB_YESNO = 0x00000004L;
	// private static long MB_DEFBUTTON2 = 0x00000100L;
	// private static int IDYES = 6;
	// private static int IDNO = 7;
	[DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);

    public static bool Alert(string Message)
    {
        int result = MessageBox(IntPtr.Zero, Message, "Alert", (int)(MB_OK));
        return result == IDOK;
    }
    
    public static bool Error(string Message)
    {
        int result = MessageBox(IntPtr.Zero, Message, "Error", (int)(MB_OK));
        return result == IDOK;
    }
}
