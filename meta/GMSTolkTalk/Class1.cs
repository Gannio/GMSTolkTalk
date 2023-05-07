using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using DavyKager64;
using System.Speech.Synthesis;


namespace GMSTolkTalk
{
    public class Class1
    {
        static bool debugEnabled = false;
        static bool SpeechSynthFallback = true;
        static SpeechSynthesizer synth = new SpeechSynthesizer();
        static Task synthLastTask = null;
        const string notLoadedText = "Tolk is not yet loaded!";
        const int OUTPUTTYPE_SPEECH = 0;
        const int OUTPUTTYPE_BRAILLE = 1;
        const int OUTPUTTYPE_BOTH = 2;
        

        private static void DebugPrint(string content)
        {
            if (!debugEnabled)
            {
                return;
            }
            else
            {
                Console.WriteLine(content);
            }
        }


        [DllExport("TolkFallbackSet", CallingConvention.Cdecl)]
        public static unsafe double TolkFallbackSet(double value)
        {
            DebugPrint("TolkFallbackSet called");
            //Doesn't need a load check, since it's just shifting a boolean.
            SpeechSynthFallback = value > 0;
            return 0;
        }
        [DllExport("TolkDebugSet", CallingConvention.Cdecl)]
        public static unsafe double TolkDebugSet(double value)
        {
            DebugPrint("TolkDebugSet called");
            //Doesn't need a load check, since it's just shifting a boolean.
            debugEnabled = value > 0;
            return 0;
        }

        [DllExport("TolkTrySAPI", CallingConvention.Cdecl)]
        public static unsafe double TolkTrySAPI(double value)
        {
            DebugPrint("TolkTrySAPI called");
            //Documentation explicitly prefers setting this up before loading, but can be done while loaded.
            Tolk.TrySAPI(value > 0);
            return 0;
        }

        [DllExport("TolkPreferSAPI", CallingConvention.Cdecl)]
        public static unsafe double TolkPreferSAPI(double value)
        {
            DebugPrint("TolkPreferSAPI called");
            //Documentation explicitly prefers setting this up before loading, but can be done while loaded.
            Tolk.PreferSAPI(value > 0);
            return 0;
        }


        [DllExport("TolkPowerOn", CallingConvention.Cdecl)]
        public static unsafe double TolkPowerOn()
        {
            DebugPrint("TolkPowerOn called");
            try
            {
                if (!Tolk.IsLoaded())
                {
                    Tolk.Load();
                    
                }
                if (Tolk.IsLoaded())
                {
                    DebugPrint("Tolk succesfully loaded.");
                    return 1;
                }
            }
            catch
            {
            }
            DebugPrint("Tolk could not be loaded.");
            return 0;
        }

        [DllExport("TolkPowerOff", CallingConvention.Cdecl)]
        public static unsafe double TolkPowerOff()
        {
            DebugPrint("TolkPowerOff called");
            if (!Tolk.IsLoaded())
            {
                DebugPrint("Tolk was not loaded in the first place.");
                return -1;
            }
            try
            {
                Tolk.Unload();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        [DllExport("TolkGetScreenReader", CallingConvention.Cdecl)]
        public static unsafe byte* TolkGetScreenReader()
        {
            DebugPrint("TolkGetScreenReader called");
            if (!Tolk.IsLoaded())
            {
                DebugPrint(notLoadedText);
                return ToGMSString("NOT LOADED");
            }
            return ToGMSString(GetScreenReader());
        }

        private static string GetScreenReader()
        {
            string name = Tolk.DetectScreenReader();
            if (name != null)
            {
                DebugPrint("The active screen reader driver is: " + name);
                return name;
            }
            else
            {
                DebugPrint("None of the Tolk-supported screen readers are running. Will use fallback of System Speech Synthesis for speech.");
                // Create a new SpeechSynthesizer object
                if (SpeechSynthFallback)
                {
                    return "fallback";
                }
                
                return "null";

            }
        }

        [DllExport("TolkHasSpeech", CallingConvention.Cdecl)]
        public static unsafe double TolkHasSpeech()
        {
            DebugPrint("TolkHasSpeech called");
            if (!Tolk.IsLoaded())
            {
                DebugPrint(notLoadedText);
                return -1;
            }
            if (Tolk.HasSpeech())
            {
                DebugPrint("This screen reader driver supports speech.");
                return 1;
            }
            else
            {
                DebugPrint("This screen reader driver does not support speech.");
                return 0;
            }

            //synth.State == SynthesizerState.Ready
        }

        [DllExport("TolkHasBraille", CallingConvention.Cdecl)]
        public static unsafe double TolkHasBraille()
        {
            DebugPrint("TolkHasBraille called");
            if (!Tolk.IsLoaded())
            {
                DebugPrint(notLoadedText);
                return -1;
            }
            if (Tolk.HasBraille())
            {
                DebugPrint("This screen reader driver supports braille.");
                return 1;
            }
            else
            {
                DebugPrint("This screen reader driver does not support braille.");
                return 0;
            }
        }
        [DllExport("TolkIsSpeaking", CallingConvention.Cdecl)]
        public static unsafe double TolkIsSpeaking()
        {
            DebugPrint("TolkIsSpeaking called");
            if (!Tolk.IsLoaded())
            {
                DebugPrint(notLoadedText);
                return 0;
            }
            if (!SpeechSynthFallback && GetScreenReader() == "fallback")
            {
                if (synth.State != SynthesizerState.Ready)
                {
                    DebugPrint("Fallback is speaking.");
                    return 1.0;
                }
                DebugPrint("Fallback is not speaking.");
                return 0.0;
            }
            else
            {
                if (Tolk.IsSpeaking())
                {
                    DebugPrint("This screen reader driver is speaking.");
                    return 1;
                }
                else
                {
                    DebugPrint("This screen reader driver is not speaking.");
                    return 0;
                }
            }
        }

        [DllExport("TolkSpeak", CallingConvention.Cdecl)]
        public static unsafe double TolkSpeak(char* inputInfo)
        {
            DebugPrint("TolkSpeak called");
            string info = Marshal.PtrToStringAnsi((IntPtr)inputInfo);
            if (!Tolk.IsLoaded())
            {
                DebugPrint(notLoadedText);
                return -1;
            }
            else
            {
                return SpeechOrBraille(info, OUTPUTTYPE_SPEECH);


            }
        }
        [DllExport("TolkBraille", CallingConvention.Cdecl)]
        public static unsafe double TolkBraille(char* inputInfo)
        {
            DebugPrint("TolkBraille called");
            string info = Marshal.PtrToStringAnsi((IntPtr)inputInfo);
            if (!Tolk.IsLoaded())
            {
                DebugPrint(notLoadedText);
                return -1;
            }
            else
            {
                return SpeechOrBraille(info, OUTPUTTYPE_BRAILLE);


            }
        }
        [DllExport("TolkOutput", CallingConvention.Cdecl)]
        public static unsafe double TolkOutput(char* inputInfo)
        {
            DebugPrint("TolkOutput called");
            string info = Marshal.PtrToStringAnsi((IntPtr)inputInfo);
            if (!Tolk.IsLoaded())
            {
                DebugPrint(notLoadedText);
                return -1;
            }
            else
            {
                return SpeechOrBraille(info, OUTPUTTYPE_BOTH);


            }
        }

        private static double SpeechOrBraille(string info, double type)
        {

            string name = Tolk.DetectScreenReader();
            DebugPrint("Text is " + info);
            if (name != null)
            {
                DebugPrint("The active screen reader driver is: " + name);
            }
            else
            {
                DebugPrint("None of the Tolk-supported screen readers are running.");

                if (SpeechSynthFallback)
                {
                    DebugPrint("Falling back to basic System Speech Synthesis.");


                    var pTask = synthLastTask;//Store the current last task.
                    var newTask = Task.Run(() =>
                    {
                        if (pTask != null)//If the last task when we recorded it wasn't null (we aren't the first task) wait until it's finished to keep going.
                        Task.WaitAll(pTask);

                        while (synth.State != SynthesizerState.Ready)
                        {
                            int d = 0;//Delay.
                        }
                        // Create a new SpeechSynthesizer object

                        // Set the output device to the default system audio device
                        synth.SetOutputToDefaultAudioDevice();

                        // Speak the desired string
                        synth.SpeakAsync(info);
                    });
                    synthLastTask = newTask;//Store ourselves as the new last task in the meantime, so all tasks go in order.
                    return 2;//Use 2 to show we had to fall back, so success is indeterminate.
                }
                else
                {
                    DebugPrint("Fallback was disabled. No output will be made.");
                    return 3;//Use 3 to show a fall back, but fallback was disabled.
                }
            }

            var result = false;
            switch (type)
            {
                case OUTPUTTYPE_SPEECH:
                    result = Tolk.Speak(info);
                    break;
                case OUTPUTTYPE_BRAILLE:
                    result = Tolk.Braille(info);
                    break;
                case OUTPUTTYPE_BOTH:
                    result = Tolk.Output(info);
                break;

            }
            if (!result)//"Hello, World!"))
            {
                DebugPrint("Failed to output text");
                return 0;
            }
            return 1;

        }

        [DllExport("TolkSilence", CallingConvention.Cdecl)]
        public static unsafe double TolkSilence()
        {
            DebugPrint("TolkSilence called");
            if (!Tolk.IsLoaded())
            {
                DebugPrint(notLoadedText);
                return -1;
            }
            else
            {
                string name = Tolk.DetectScreenReader();
                if (name != null)
                {
                    DebugPrint("The active screen reader driver is: " + name);
                }
                else
                {
                    DebugPrint("None of the Tolk-supported screen readers are running.");

                    if (SpeechSynthFallback)
                    {
                        
                        DebugPrint("Falling back to basic System Speech Synthesis.");
                        // Speak the desired string
                        synth.SpeakAsyncCancelAll();//There's technically just a single cancel, but afaik the Tolk API lacks such a thing, so it probably cancels all at once.
                        return 2;//Use 2 to show we had to fall back, but it's still a success.
                    }
                    else
                    {
                        DebugPrint("Fallback was disabled. There is nothing to silence?");
                        return 3;//Use 3 to show a fallback, but fallback was disabled.
                    }
                    
                }


            }
            if (!Tolk.Silence())//"Hello, World!"))
            {
                DebugPrint("Failed to silence");
                return 0;
            }
            return 1;
        }

        private static unsafe byte* ToGMSString(string normalString)
        {
            //Small note here since it probably will get someone worried: Unsafe is not necessairly dangerous. It's that it makes things more like C++ where you have to manage your data to avoid memory leaks/errors.
            //In our case, we are simply using it to properly return a string to GMS, as C# can't support pointers otherwise. Strings are passed through this function for that reason.
            //The 'fixed' statement below basically brings us *back* to a managed statement before the return value is sent.
            fixed (byte* unmanagedOutput = new byte[normalString.Length])//(byte*)&output)
            {
                for (var i = 0; i < normalString.Length; i++)
                {
                    unmanagedOutput[i] = (byte)normalString[i];
                }
                /*unmanagedOutput[output.Length] = 0x00;//Set the null terminator.
                unmanagedOutput[output.Length+1] = 0x00;//Set the null terminator.
                unmanagedOutput[output.Length + 2 ] = 0x00;//Set the null terminator.
                unmanagedOutput[output.Length + 3] = 0x00;//Set the null terminator.
                unmanagedOutput[output.Length + 4] = 0x00;//Set the null terminator.
                unmanagedOutput[output.Length + 5] = 0x00;//Set the null terminator.
                unmanagedOutput[output.Length + 6] = 0x00;//Set the null terminator.
                unmanagedOutput[output.Length + 7] = 0x00;//Set the null terminator.*/
                return unmanagedOutput;
            }
        }

        [DllExport("TolkTest", CallingConvention.Cdecl)]
        public static unsafe double TolkTest(char* inputInfo)
        {
            DebugPrint("TolkTest called");
            string info = Marshal.PtrToStringAnsi((IntPtr)inputInfo);
            if (!Tolk.IsLoaded())
            {
                Tolk.Load();
            }
            string name = Tolk.DetectScreenReader();
            if (name != null)
            {
                DebugPrint("The active screen reader driver is: " + name);
            }
            else
            {
                DebugPrint("None of the Tolk-supported screen readers are running. Falling back to System Speech Synthesis.");
                // Create a new SpeechSynthesizer object
                SpeechSynthesizer synth = new SpeechSynthesizer();

                // Set the output device to the default system audio device
                synth.SetOutputToDefaultAudioDevice();

                // Speak the desired string
                synth.Speak(info);

            }
            if (Tolk.HasSpeech())
            {
                DebugPrint("This screen reader driver supports speech");
            }
            if (Tolk.HasBraille())
            {
                DebugPrint("This screen reader driver supports braille");
            }
            DebugPrint("Let's output some text...");
            if (!Tolk.Output(info))//"Hello, World!"))
            {
                DebugPrint("Failed to output text");
            }
            DebugPrint("Finalizing Tolk...");
            Tolk.Unload();

            DebugPrint("Done!");
            return 0;
            //Tolk.Unload();
        }
    }
}
